using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace STS2PartyWatch.Combat;

internal static class CardTurnEndDamageInspector
{
    private static readonly Dictionary<short, OpCode> Opcodes = BuildOpcodeMap();
    private static readonly Dictionary<Type, bool> Cache = new();

    public static bool DoesTurnEndInHandCallDamage(CardModel card)
    {
        if (!card.HasTurnEndInHandEffect)
        {
            return false;
        }

        var cardType = card.GetType();
        if (Cache.TryGetValue(cardType, out var cached))
        {
            return cached;
        }

        var result = DoesTurnEndInHandCallDamage(cardType);
        Cache[cardType] = result;
        return result;
    }

    private static bool DoesTurnEndInHandCallDamage(Type cardType)
    {
        var method = cardType.GetMethod("OnTurnEndInHand", BindingFlags.Instance | BindingFlags.NonPublic);
        if (method is null || method.DeclaringType == typeof(CardModel))
        {
            return false;
        }

        var stateMachine = method.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType;
        if (stateMachine is not null)
        {
            var moveNext = stateMachine.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return moveNext is not null && CallsCreatureDamage(moveNext);
        }

        return CallsCreatureDamage(method);
    }

    private static bool CallsCreatureDamage(MethodBase method)
    {
        var il = method.GetMethodBody()?.GetILAsByteArray();
        if (il is null || il.Length == 0)
        {
            return false;
        }

        var module = method.Module;
        for (var index = 0; index < il.Length;)
        {
            if (!TryReadOpcode(il, ref index, out var opcode))
            {
                return false;
            }

            if (opcode.OperandType == OperandType.InlineMethod)
            {
                var token = BitConverter.ToInt32(il, index);
                if (IsCreatureDamageCall(module, token))
                {
                    return true;
                }
            }

            if (!TrySkipOperand(il, ref index, opcode.OperandType))
            {
                return false;
            }
        }

        return false;
    }

    private static bool TryReadOpcode(byte[] il, ref int index, out OpCode opcode)
    {
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

        return Opcodes.TryGetValue((short)value, out opcode);
    }

    private static bool IsCreatureDamageCall(Module module, int metadataToken)
    {
        try
        {
            var called = module.ResolveMethod(metadataToken);
            return called?.DeclaringType == typeof(CreatureCmd) && called.Name == nameof(CreatureCmd.Damage);
        }
        catch
        {
            return false;
        }
    }

    private static bool TrySkipOperand(byte[] il, ref int index, OperandType operandType)
    {
        var size = operandType switch
        {
            OperandType.InlineNone => 0,
            OperandType.ShortInlineBrTarget => 1,
            OperandType.ShortInlineI => 1,
            OperandType.ShortInlineVar => 1,
            OperandType.InlineVar => 2,
            OperandType.InlineBrTarget => 4,
            OperandType.InlineField => 4,
            OperandType.InlineI => 4,
            OperandType.InlineMethod => 4,
            OperandType.InlineSig => 4,
            OperandType.InlineString => 4,
            OperandType.InlineSwitch => ReadSwitchOperandSize(il, index),
            OperandType.InlineTok => 4,
            OperandType.InlineType => 4,
            OperandType.ShortInlineR => 4,
            OperandType.InlineI8 => 8,
            OperandType.InlineR => 8,
            _ => 0
        };

        if (size < 0 || index + size > il.Length)
        {
            return false;
        }

        index += size;
        return true;
    }

    private static int ReadSwitchOperandSize(byte[] il, int index)
    {
        if (index + 4 > il.Length)
        {
            return -1;
        }

        var count = BitConverter.ToInt32(il, index);
        return count < 0 ? -1 : 4 + count * 4;
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
}
