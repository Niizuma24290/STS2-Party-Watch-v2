using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace STS2PartyWatch.Combat;

internal static class ObservedHpLossBudgetTracker
{
    private static readonly Dictionary<ulong, ObservedHpLossBudget> Budgets = new();

    public static void Observe(Player player)
    {
        if (player.Creature is not { } creature)
        {
            return;
        }

        Observe(creature);
    }

    public static void Observe(Creature creature)
    {
        var player = creature.Player;
        if (player is null)
        {
            return;
        }

        var currentHp = Math.Max(0, creature.CurrentHp);
        if (!Budgets.TryGetValue(player.NetId, out var budget))
        {
            Budgets[player.NetId] = new ObservedHpLossBudget(currentHp, 0);
            return;
        }

        if (currentHp < budget.LastObservedHp)
        {
            budget = budget with
            {
                SpentThisWindow = budget.SpentThisWindow + budget.LastObservedHp - currentHp
            };
        }

        Budgets[player.NetId] = budget with { LastObservedHp = currentHp };
    }

    public static void ResetWindow(Player player)
    {
        if (player.Creature is not { } creature)
        {
            Budgets.Remove(player.NetId);
            return;
        }

        Budgets[player.NetId] = new ObservedHpLossBudget(Math.Max(0, creature.CurrentHp), 0);
    }

    public static int GetSpent(Player player)
    {
        if (!Budgets.TryGetValue(player.NetId, out var budget))
        {
            Observe(player);
            return 0;
        }

        return Math.Max(0, budget.SpentThisWindow);
    }

    private readonly record struct ObservedHpLossBudget(int LastObservedHp, int SpentThisWindow);
}
