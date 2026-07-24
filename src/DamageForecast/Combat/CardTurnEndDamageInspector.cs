using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DamageForecast.Combat;

internal static class CardTurnEndDamageInspector
{
    private const string ChoiceContextTypeName =
        "MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext";
    private const string CreatureTypeName =
        "MegaCrit.Sts2.Core.Entities.Creatures.Creature";
    private const string CardPlayTypeName =
        "MegaCrit.Sts2.Core.Entities.Cards.CardPlay";
    private const string ValuePropTypeName =
        "MegaCrit.Sts2.Core.ValueProps.ValueProp";

    private static readonly Dictionary<short, OpCode> Opcodes = BuildOpcodeMap();
    private static readonly ConcurrentDictionary<Type, TurnEndDamageShapeDescriptor> Cache = new();

    public static bool DoesTurnEndInHandCallDamage(CardModel card)
    {
        if (!card.HasTurnEndInHandEffect)
        {
            return false;
        }

        return Cache.GetOrAdd(card.GetType(), InspectType).DamageCallCount > 0;
    }

    public static bool TryGetVerifiedSingleBlockableDamageVar(CardModel card, out DamageVar? damageVar)
    {
        damageVar = null;

        if (!card.HasTurnEndInHandEffect)
        {
            var noEffect = new TurnEndDamageShapeDescriptor(
                IsOfficialCurrentGameAssembly: card.GetType().Assembly == typeof(CardModel).Assembly,
                MethodDeclaredOnConcreteType: false,
                BodyResolution: TurnEndDamageBodyResolution.NoTurnEndEffect,
                DamageCallCount: 0,
                OverloadKind: TurnEndDamageOverloadKind.None,
                TargetIsSameCardOwnerCreature: false,
                AmountIsSameCardDynamicVarsDamage: false,
                SourceCardIsSameCard: false,
                BetaCardPlayIsNull: false,
                ControlFlowIsSupported: true);
            return VerifiedTurnEndDamagePolicy.ClassifyDescriptor(
                    noEffect,
                    new TurnEndDamageInstanceShape(0, false, false))
                .Decision == TurnEndDamageDecision.NoDamage;
        }

        try
        {
            var descriptor = Cache.GetOrAdd(card.GetType(), InspectType);
            var damageVars = card.DynamicVars.Values.OfType<DamageVar>().ToArray();
            var singleDamageVar = damageVars.Length == 1 ? damageVars[0] : null;
            var instance = new TurnEndDamageInstanceShape(
                damageVars.Length,
                IsSupportedBaseValue(singleDamageVar),
                singleDamageVar?.Props == (ValueProp.Unpowered | ValueProp.Move));
            var result = VerifiedTurnEndDamagePolicy.ClassifyDescriptor(descriptor, instance);

            if (result.Decision == TurnEndDamageDecision.VerifiedBlockable)
            {
                damageVar = singleDamageVar;
                return true;
            }

            return result.Decision == TurnEndDamageDecision.NoDamage;
        }
        catch
        {
            damageVar = null;
            return false;
        }
    }

    private static TurnEndDamageShapeDescriptor InspectType(Type cardType)
    {
        var officialAssembly = cardType.Assembly == typeof(CardModel).Assembly;

        try
        {
            var method = cardType.GetMethod(
                "OnTurnEndInHand",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method is null || method.DeclaringType == typeof(CardModel))
            {
                return Unreadable(officialAssembly, method?.DeclaringType == cardType);
            }

            var methodDeclaredOnConcreteType = method.DeclaringType == cardType;
            var stateMachine = method.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType;
            var bodyResolution = TurnEndDamageBodyResolution.ParsedSync;
            MethodBase bodyMethod = method;
            if (stateMachine is not null)
            {
                var moveNext = stateMachine.GetMethod(
                    "MoveNext",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (moveNext is null)
                {
                    return Unreadable(officialAssembly, methodDeclaredOnConcreteType);
                }

                bodyMethod = moveNext;
                bodyResolution = TurnEndDamageBodyResolution.ParsedAsync;
            }

            if (!TryDecodeInstructions(bodyMethod, out var instructions))
            {
                return Unreadable(officialAssembly, methodDeclaredOnConcreteType);
            }

            var damageCallIndexes = instructions
                .Select((instruction, index) => (instruction, index))
                .Where(pair => IsCreatureDamageCall(pair.instruction))
                .Select(pair => pair.index)
                .ToArray();
            if (damageCallIndexes.Length == 0)
            {
                return new TurnEndDamageShapeDescriptor(
                    officialAssembly,
                    methodDeclaredOnConcreteType,
                    bodyResolution,
                    DamageCallCount: 0,
                    TurnEndDamageOverloadKind.None,
                    TargetIsSameCardOwnerCreature: false,
                    AmountIsSameCardDynamicVarsDamage: false,
                    SourceCardIsSameCard: false,
                    BetaCardPlayIsNull: false,
                    ControlFlowIsSupported: true);
            }

            if (damageCallIndexes.Length != 1)
            {
                return new TurnEndDamageShapeDescriptor(
                    officialAssembly,
                    methodDeclaredOnConcreteType,
                    bodyResolution,
                    damageCallIndexes.Length,
                    TurnEndDamageOverloadKind.Other,
                    TargetIsSameCardOwnerCreature: false,
                    AmountIsSameCardDynamicVarsDamage: false,
                    SourceCardIsSameCard: false,
                    BetaCardPlayIsNull: false,
                    ControlFlowIsSupported: false);
            }

            var callIndex = damageCallIndexes[0];
            var calledMethod = (MethodBase)instructions[callIndex].Operand!;
            var overloadKind = ClassifyDamageOverload(calledMethod);
            var provenance = InspectDamageCallProvenance(
                instructions,
                callIndex,
                cardType,
                bodyResolution == TurnEndDamageBodyResolution.ParsedAsync,
                overloadKind);

            return new TurnEndDamageShapeDescriptor(
                officialAssembly,
                methodDeclaredOnConcreteType,
                bodyResolution,
                DamageCallCount: 1,
                overloadKind,
                provenance.TargetIsSameCardOwnerCreature,
                provenance.AmountIsSameCardDynamicVarsDamage,
                provenance.SourceCardIsSameCard,
                provenance.BetaCardPlayIsNull,
                provenance.ControlFlowIsSupported);
        }
        catch
        {
            return Unreadable(officialAssembly, methodDeclaredOnConcreteType: false);
        }
    }

    private static TurnEndDamageCallProvenance InspectDamageCallProvenance(
        IReadOnlyList<DecodedInstruction> instructions,
        int callIndex,
        Type cardType,
        bool isAsyncBody,
        TurnEndDamageOverloadKind overloadKind)
    {
        var currentCardLocals = FindCurrentCardLocals(instructions, cardType, isAsyncBody);
        var cursor = callIndex - 1;

        var betaCardPlayIsNull = overloadKind != TurnEndDamageOverloadKind.DamageVarBeta;
        if (overloadKind == TurnEndDamageOverloadKind.DamageVarBeta)
        {
            betaCardPlayIsNull = TryConsumeOpcode(instructions, ref cursor, OpCodes.Ldnull);
        }

        var sourceCardIsSameCard =
            TryConsumeCurrentCard(instructions, ref cursor, currentCardLocals, cardType, isAsyncBody);
        var amountIsSameCardDynamicVarsDamage =
            sourceCardIsSameCard
            && TryConsumeCall(
                instructions,
                ref cursor,
                typeof(DynamicVarSet),
                "get_Damage")
            && TryConsumeCall(
                instructions,
                ref cursor,
                typeof(CardModel),
                "get_DynamicVars")
            && TryConsumeCurrentCard(
                instructions,
                ref cursor,
                currentCardLocals,
                cardType,
                isAsyncBody);
        var targetIsSameCardOwnerCreature =
            amountIsSameCardDynamicVarsDamage
            && TryConsumeCallByName(
                instructions,
                ref cursor,
                "MegaCrit.Sts2.Core.Entities.Players.Player",
                "get_Creature")
            && TryConsumeCall(
                instructions,
                ref cursor,
                typeof(CardModel),
                "get_Owner")
            && TryConsumeCurrentCard(
                instructions,
                ref cursor,
                currentCardLocals,
                cardType,
                isAsyncBody);

        var sliceStart = targetIsSameCardOwnerCreature ? cursor + 1 : callIndex;
        var controlFlowIsSupported =
            targetIsSameCardOwnerCreature
            && amountIsSameCardDynamicVarsDamage
            && sourceCardIsSameCard
            && betaCardPlayIsNull
            && !HasUnsupportedControlFlow(instructions, sliceStart, callIndex, isAsyncBody);

        return new TurnEndDamageCallProvenance(
            targetIsSameCardOwnerCreature,
            amountIsSameCardDynamicVarsDamage,
            sourceCardIsSameCard,
            betaCardPlayIsNull,
            controlFlowIsSupported);
    }

    private static HashSet<int> FindCurrentCardLocals(
        IReadOnlyList<DecodedInstruction> instructions,
        Type cardType,
        bool isAsyncBody)
    {
        var locals = new HashSet<int>();
        if (!isAsyncBody)
        {
            return locals;
        }

        for (var index = 0; index + 2 < instructions.Count; index++)
        {
            if (instructions[index].Opcode != OpCodes.Ldarg_0
                || instructions[index + 1].Opcode != OpCodes.Ldfld
                || instructions[index + 1].Operand is not FieldInfo field
                || field.Name != "<>4__this"
                || field.FieldType != cardType
                || !TryGetStoredLocalIndex(instructions[index + 2], out var localIndex))
            {
                continue;
            }

            locals.Add(localIndex);
        }

        return locals;
    }

    private static bool TryConsumeCurrentCard(
        IReadOnlyList<DecodedInstruction> instructions,
        ref int cursor,
        IReadOnlySet<int> currentCardLocals,
        Type cardType,
        bool isAsyncBody)
    {
        if (cursor < 0)
        {
            return false;
        }

        if (TryGetLoadedLocalIndex(instructions[cursor], out var localIndex)
            && currentCardLocals.Contains(localIndex))
        {
            cursor--;
            return true;
        }

        if (!isAsyncBody && instructions[cursor].Opcode == OpCodes.Ldarg_0)
        {
            cursor--;
            return true;
        }

        if (isAsyncBody
            && instructions[cursor].Opcode == OpCodes.Ldfld
            && instructions[cursor].Operand is FieldInfo field
            && field.Name == "<>4__this"
            && field.FieldType == cardType
            && cursor > 0
            && instructions[cursor - 1].Opcode == OpCodes.Ldarg_0)
        {
            cursor -= 2;
            return true;
        }

        return false;
    }

    private static bool TryConsumeCall(
        IReadOnlyList<DecodedInstruction> instructions,
        ref int cursor,
        Type declaringType,
        string methodName)
    {
        if (cursor < 0
            || !IsCallOpcode(instructions[cursor].Opcode)
            || instructions[cursor].Operand is not MethodBase method
            || method.DeclaringType != declaringType
            || method.Name != methodName)
        {
            return false;
        }

        cursor--;
        return true;
    }

    private static bool TryConsumeCallByName(
        IReadOnlyList<DecodedInstruction> instructions,
        ref int cursor,
        string declaringTypeName,
        string methodName)
    {
        if (cursor < 0
            || !IsCallOpcode(instructions[cursor].Opcode)
            || instructions[cursor].Operand is not MethodBase method
            || method.DeclaringType?.FullName != declaringTypeName
            || method.Name != methodName)
        {
            return false;
        }

        cursor--;
        return true;
    }

    private static bool TryConsumeOpcode(
        IReadOnlyList<DecodedInstruction> instructions,
        ref int cursor,
        OpCode opcode)
    {
        if (cursor < 0 || instructions[cursor].Opcode != opcode)
        {
            return false;
        }

        cursor--;
        return true;
    }

    private static bool HasUnsupportedControlFlow(
        IReadOnlyList<DecodedInstruction> instructions,
        int sliceStartIndex,
        int callIndex,
        bool isAsyncBody)
    {
        var sliceStartOffset = instructions[sliceStartIndex].Offset;
        var callOffset = instructions[callIndex].Offset;
        var asyncStateLocals = FindAsyncStateLocals(instructions, isAsyncBody);

        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (index >= sliceStartIndex
                && index <= callIndex
                && instruction.Opcode.FlowControl is FlowControl.Branch or FlowControl.Cond_Branch)
            {
                return true;
            }

            if (instruction.Opcode.OperandType is not (
                    OperandType.ShortInlineBrTarget
                    or OperandType.InlineBrTarget
                    or OperandType.InlineSwitch))
            {
                continue;
            }

            foreach (var target in ReadBranchTargets(instruction.Operand))
            {
                if ((index < sliceStartIndex || index > callIndex)
                    && target >= sliceStartOffset
                    && target <= callOffset)
                {
                    return true;
                }

                if (index < sliceStartIndex
                    && target > callOffset
                    && !IsCanonicalAsyncResumeBranch(
                        instructions,
                        index,
                        asyncStateLocals,
                        isAsyncBody))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static HashSet<int> FindAsyncStateLocals(
        IReadOnlyList<DecodedInstruction> instructions,
        bool isAsyncBody)
    {
        var locals = new HashSet<int>();
        if (!isAsyncBody)
        {
            return locals;
        }

        for (var index = 0; index + 2 < instructions.Count; index++)
        {
            if (instructions[index].Opcode != OpCodes.Ldarg_0
                || instructions[index + 1].Opcode != OpCodes.Ldfld
                || instructions[index + 1].Operand is not FieldInfo field
                || field.Name != "<>1__state"
                || field.FieldType != typeof(int)
                || !TryGetStoredLocalIndex(instructions[index + 2], out var localIndex))
            {
                continue;
            }

            locals.Add(localIndex);
        }

        return locals;
    }

    private static bool IsCanonicalAsyncResumeBranch(
        IReadOnlyList<DecodedInstruction> instructions,
        int branchIndex,
        IReadOnlySet<int> asyncStateLocals,
        bool isAsyncBody)
    {
        if (!isAsyncBody
            || branchIndex <= 0
            || (instructions[branchIndex].Opcode != OpCodes.Brfalse
                && instructions[branchIndex].Opcode != OpCodes.Brfalse_S)
            || !TryGetLoadedLocalIndex(instructions[branchIndex - 1], out var localIndex))
        {
            return false;
        }

        return asyncStateLocals.Contains(localIndex);
    }

    private static IEnumerable<int> ReadBranchTargets(object? operand)
    {
        if (operand is int target)
        {
            yield return target;
        }
        else if (operand is int[] targets)
        {
            foreach (var item in targets)
            {
                yield return item;
            }
        }
    }

    private static TurnEndDamageOverloadKind ClassifyDamageOverload(MethodBase method)
    {
        var parameters = method.GetParameters();
        if (parameters.Length is 4 or 5
            && parameters[0].ParameterType.FullName == ChoiceContextTypeName
            && parameters[1].ParameterType.FullName == CreatureTypeName
            && parameters[2].ParameterType == typeof(DamageVar)
            && parameters[3].ParameterType == typeof(CardModel))
        {
            if (parameters.Length == 4)
            {
                return TurnEndDamageOverloadKind.DamageVarStable;
            }

            return parameters[4].ParameterType.FullName == CardPlayTypeName
                ? TurnEndDamageOverloadKind.DamageVarBeta
                : TurnEndDamageOverloadKind.Other;
        }

        if (parameters.Length is 5 or 6
            && parameters[0].ParameterType.FullName == ChoiceContextTypeName
            && parameters[1].ParameterType.FullName == CreatureTypeName
            && parameters[2].ParameterType == typeof(decimal)
            && parameters[3].ParameterType.FullName == ValuePropTypeName
            && parameters[4].ParameterType == typeof(CardModel))
        {
            return TurnEndDamageOverloadKind.Decimal;
        }

        return TurnEndDamageOverloadKind.Other;
    }

    private static bool IsCreatureDamageCall(DecodedInstruction instruction)
    {
        return IsCallOpcode(instruction.Opcode)
            && instruction.Operand is MethodBase method
            && method.DeclaringType == typeof(CreatureCmd)
            && method.Name == nameof(CreatureCmd.Damage);
    }

    private static bool IsCallOpcode(OpCode opcode)
    {
        return opcode == OpCodes.Call || opcode == OpCodes.Callvirt;
    }

    private static bool IsSupportedBaseValue(DamageVar? damageVar)
    {
        if (damageVar is null)
        {
            return false;
        }

        var baseValue = damageVar.BaseValue;
        return baseValue >= 0
            && baseValue <= int.MaxValue
            && decimal.Truncate(baseValue) == baseValue;
    }

    private static TurnEndDamageShapeDescriptor Unreadable(
        bool officialAssembly,
        bool methodDeclaredOnConcreteType)
    {
        return new TurnEndDamageShapeDescriptor(
            officialAssembly,
            methodDeclaredOnConcreteType,
            TurnEndDamageBodyResolution.Unreadable,
            DamageCallCount: -1,
            TurnEndDamageOverloadKind.Other,
            TargetIsSameCardOwnerCreature: false,
            AmountIsSameCardDynamicVarsDamage: false,
            SourceCardIsSameCard: false,
            BetaCardPlayIsNull: false,
            ControlFlowIsSupported: false);
    }

    private static bool TryDecodeInstructions(
        MethodBase method,
        out List<DecodedInstruction> instructions)
    {
        instructions = new List<DecodedInstruction>();
        var il = method.GetMethodBody()?.GetILAsByteArray();
        if (il is null || il.Length == 0)
        {
            return false;
        }

        var module = method.Module;
        var typeArguments = method.DeclaringType?.GetGenericArguments() ?? Type.EmptyTypes;
        var methodArguments = method.IsGenericMethod ? method.GetGenericArguments() : Type.EmptyTypes;
        for (var index = 0; index < il.Length;)
        {
            var offset = index;
            if (!TryReadOpcode(il, ref index, out var opcode)
                || !TryReadOperand(
                    il,
                    ref index,
                    opcode.OperandType,
                    module,
                    typeArguments,
                    methodArguments,
                    out var operand))
            {
                instructions.Clear();
                return false;
            }

            instructions.Add(new DecodedInstruction(offset, opcode, operand));
        }

        return true;
    }

    private static bool TryReadOpcode(byte[] il, ref int index, out OpCode opcode)
    {
        if (index >= il.Length)
        {
            opcode = default;
            return false;
        }

        var value = (short)il[index++];
        if (value == 0xFE)
        {
            if (index >= il.Length)
            {
                opcode = default;
                return false;
            }

            value = (short)(0xFE00 | il[index++]);
        }

        return Opcodes.TryGetValue(value, out opcode);
    }

    private static bool TryReadOperand(
        byte[] il,
        ref int index,
        OperandType operandType,
        Module module,
        Type[] typeArguments,
        Type[] methodArguments,
        out object? operand)
    {
        operand = null;
        try
        {
            switch (operandType)
            {
                case OperandType.InlineNone:
                    return true;
                case OperandType.ShortInlineBrTarget:
                    if (!HasBytes(il, index, 1))
                    {
                        return false;
                    }

                    var shortDelta = unchecked((sbyte)il[index++]);
                    operand = index + shortDelta;
                    return true;
                case OperandType.InlineBrTarget:
                    if (!HasBytes(il, index, 4))
                    {
                        return false;
                    }

                    var branchDelta = BitConverter.ToInt32(il, index);
                    index += 4;
                    operand = index + branchDelta;
                    return true;
                case OperandType.InlineSwitch:
                    if (!HasBytes(il, index, 4))
                    {
                        return false;
                    }

                    var count = BitConverter.ToInt32(il, index);
                    index += 4;
                    if (count < 0 || !HasBytes(il, index, count * 4))
                    {
                        return false;
                    }

                    var switchBase = index + count * 4;
                    var targets = new int[count];
                    for (var item = 0; item < count; item++)
                    {
                        targets[item] = switchBase + BitConverter.ToInt32(il, index);
                        index += 4;
                    }

                    operand = targets;
                    return true;
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    if (!HasBytes(il, index, 1))
                    {
                        return false;
                    }

                    operand = (int)il[index++];
                    return true;
                case OperandType.InlineVar:
                    if (!HasBytes(il, index, 2))
                    {
                        return false;
                    }

                    operand = (int)BitConverter.ToUInt16(il, index);
                    index += 2;
                    return true;
                case OperandType.InlineI:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                    if (!HasBytes(il, index, 4))
                    {
                        return false;
                    }

                    operand = BitConverter.ToInt32(il, index);
                    index += 4;
                    return true;
                case OperandType.InlineField:
                    if (!HasBytes(il, index, 4))
                    {
                        return false;
                    }

                    var fieldToken = BitConverter.ToInt32(il, index);
                    index += 4;
                    operand = module.ResolveField(fieldToken, typeArguments, methodArguments);
                    return operand is not null;
                case OperandType.InlineMethod:
                    if (!HasBytes(il, index, 4))
                    {
                        return false;
                    }

                    var methodToken = BitConverter.ToInt32(il, index);
                    index += 4;
                    operand = module.ResolveMethod(methodToken, typeArguments, methodArguments);
                    return operand is not null;
                case OperandType.ShortInlineR:
                    if (!HasBytes(il, index, 4))
                    {
                        return false;
                    }

                    operand = BitConverter.ToSingle(il, index);
                    index += 4;
                    return true;
                case OperandType.InlineI8:
                    if (!HasBytes(il, index, 8))
                    {
                        return false;
                    }

                    operand = BitConverter.ToInt64(il, index);
                    index += 8;
                    return true;
                case OperandType.InlineR:
                    if (!HasBytes(il, index, 8))
                    {
                        return false;
                    }

                    operand = BitConverter.ToDouble(il, index);
                    index += 8;
                    return true;
                default:
                    return false;
            }
        }
        catch
        {
            operand = null;
            return false;
        }
    }

    private static bool HasBytes(byte[] il, int index, int count)
    {
        return count >= 0 && index >= 0 && index <= il.Length - count;
    }

    private static bool TryGetLoadedLocalIndex(DecodedInstruction instruction, out int localIndex)
    {
        if (instruction.Opcode == OpCodes.Ldloc_0)
        {
            localIndex = 0;
            return true;
        }

        if (instruction.Opcode == OpCodes.Ldloc_1)
        {
            localIndex = 1;
            return true;
        }

        if (instruction.Opcode == OpCodes.Ldloc_2)
        {
            localIndex = 2;
            return true;
        }

        if (instruction.Opcode == OpCodes.Ldloc_3)
        {
            localIndex = 3;
            return true;
        }

        if ((instruction.Opcode == OpCodes.Ldloc || instruction.Opcode == OpCodes.Ldloc_S)
            && instruction.Operand is int index)
        {
            localIndex = index;
            return true;
        }

        localIndex = -1;
        return false;
    }

    private static bool TryGetStoredLocalIndex(DecodedInstruction instruction, out int localIndex)
    {
        if (instruction.Opcode == OpCodes.Stloc_0)
        {
            localIndex = 0;
            return true;
        }

        if (instruction.Opcode == OpCodes.Stloc_1)
        {
            localIndex = 1;
            return true;
        }

        if (instruction.Opcode == OpCodes.Stloc_2)
        {
            localIndex = 2;
            return true;
        }

        if (instruction.Opcode == OpCodes.Stloc_3)
        {
            localIndex = 3;
            return true;
        }

        if ((instruction.Opcode == OpCodes.Stloc || instruction.Opcode == OpCodes.Stloc_S)
            && instruction.Operand is int index)
        {
            localIndex = index;
            return true;
        }

        localIndex = -1;
        return false;
    }

    private static Dictionary<short, OpCode> BuildOpcodeMap()
    {
        var map = new Dictionary<short, OpCode>();
        foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.GetValue(null) is OpCode opcode)
            {
                map[opcode.Value] = opcode;
            }
        }

        return map;
    }

    private readonly record struct DecodedInstruction(int Offset, OpCode Opcode, object? Operand);

    private readonly record struct TurnEndDamageCallProvenance(
        bool TargetIsSameCardOwnerCreature,
        bool AmountIsSameCardDynamicVarsDamage,
        bool SourceCardIsSameCard,
        bool BetaCardPlayIsNull,
        bool ControlFlowIsSupported);
}
