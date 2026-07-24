using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DamageForecast.Combat;

internal static class VerifiedTurnEndPowerDamageReader
{
    private const int FirstPowerTurnEndOrder = 500_000;

    public static bool TryRead(Creature localCreature, out List<VerifiedTurnEndPowerDamageEvent> events, out int blockableRaw)
    {
        events = new List<VerifiedTurnEndPowerDamageEvent>();
        blockableRaw = 0;

        try
        {
            AddPowerEvent<ConstrictPower>(localCreature, "ConstrictPower", FirstPowerTurnEndOrder, events, ref blockableRaw);
            AddPowerEvent<DisintegrationPower>(localCreature, "DisintegrationPower", FirstPowerTurnEndOrder + 1, events, ref blockableRaw);
            return true;
        }
        catch
        {
            events.Clear();
            blockableRaw = 0;
            return false;
        }
    }

    private static void AddPowerEvent<TPower>(
        Creature localCreature,
        string source,
        int nativeExecutionOrder,
        List<VerifiedTurnEndPowerDamageEvent> events,
        ref int blockableRaw)
        where TPower : PowerModel
    {
        var power = localCreature.GetPower<TPower>();
        if (power is null || power.Amount <= 0)
        {
            return;
        }

        var damage = GetModifiedPowerDamage(localCreature, power.Amount);
        blockableRaw += damage;
        events.Add(new VerifiedTurnEndPowerDamageEvent(source, nativeExecutionOrder, damage, true));
    }

    private static int GetModifiedPowerDamage(Creature localCreature, int baseDamage)
    {
        var modified = HookDamageCompat.ModifyDamage(
            localCreature.CombatState!.RunState,
            localCreature.CombatState,
            localCreature,
            localCreature,
            baseDamage,
            DamageProps.nonCardUnpowered,
            null,
            ModifyDamageHookType.All,
            CardPreviewMode.None);

        return Math.Max(0, (int)modified);
    }
}

internal readonly record struct VerifiedTurnEndPowerDamageEvent(
    string Source,
    int NativeExecutionOrder,
    int Amount,
    bool IsSingleVerifiedEvent);
