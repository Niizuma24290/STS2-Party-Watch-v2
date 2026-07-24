using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DamageForecast.Combat;

internal static class HookDamageCompat
{
    private static readonly MethodInfo ModifyDamageMethod = ResolveModifyDamage();
    private static readonly bool HasCardPlayArgument = ModifyDamageMethod.GetParameters().Length == 11;

    public static decimal ModifyDamage(
        object runState,
        ICombatState combatState,
        Creature source,
        Creature target,
        decimal amount,
        ValueProp props,
        CardModel? card,
        ModifyDamageHookType hookType,
        CardPreviewMode previewMode)
    {
        var args = HasCardPlayArgument
            ? new object?[] { runState, combatState, source, target, amount, props, card, null, hookType, previewMode, null }
            : new object?[] { runState, combatState, source, target, amount, props, card, hookType, previewMode, null };

        return (decimal)ModifyDamageMethod.Invoke(null, args)!;
    }

    private static MethodInfo ResolveModifyDamage()
    {
        return typeof(Hook)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.Name == nameof(Hook.ModifyDamage))
            .FirstOrDefault(IsSupportedSignature)
            ?? throw new MissingMethodException(typeof(Hook).FullName, nameof(Hook.ModifyDamage));
    }

    private static bool IsSupportedSignature(MethodInfo method)
    {
        var parameters = method.GetParameters();
        if (parameters.Length is not (10 or 11))
        {
            return false;
        }

        var hookTypeIndex = parameters.Length == 11 ? 8 : 7;
        return parameters[hookTypeIndex].ParameterType == typeof(ModifyDamageHookType)
            && parameters[hookTypeIndex + 1].ParameterType == typeof(CardPreviewMode);
    }
}
