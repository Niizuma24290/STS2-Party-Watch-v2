using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DamageForecast.Combat;

internal static class ObservedHpLossBudgetTracker
{
    private static readonly Dictionary<ulong, HpLossBudgetState> Budgets = new();

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

        ObserveIdentity(player.NetId, creature.CurrentHp);
    }

    public static void ResetWindow(Player player)
    {
        if (player.Creature is not { } creature)
        {
            Budgets.Remove(player.NetId);
            return;
        }

        Budgets[player.NetId] = HpLossBudgetPolicy.Reset(creature.CurrentHp);
    }

    public static int GetSpent(Player player)
    {
        if (!Budgets.TryGetValue(player.NetId, out var budget))
        {
            Observe(player);
            return 0;
        }

        return HpLossBudgetPolicy.GetSpent(budget);
    }

    public static void Clear()
    {
        Budgets.Clear();
    }

    internal static void ObserveIdentity(ulong playerIdentity, int currentHp)
    {
        Budgets.TryGetValue(playerIdentity, out var previous);
        Budgets[playerIdentity] = HpLossBudgetPolicy.Observe(
            Budgets.ContainsKey(playerIdentity) ? previous : null,
            currentHp);
    }

    internal static int GetSpentForIdentity(ulong playerIdentity) =>
        Budgets.TryGetValue(playerIdentity, out var budget)
            ? HpLossBudgetPolicy.GetSpent(budget)
            : 0;
}

internal static class HpLossBudgetPolicy
{
    public static HpLossBudgetState Observe(HpLossBudgetState? previous, int currentHp)
    {
        currentHp = Math.Max(0, currentHp);
        if (previous is not { } budget)
        {
            return new(currentHp, 0);
        }

        var spent = budget.SpentThisWindow;
        if (currentHp < budget.LastObservedHp)
        {
            spent += budget.LastObservedHp - currentHp;
        }

        return new(currentHp, spent);
    }

    public static HpLossBudgetState Reset(int currentHp) =>
        new(Math.Max(0, currentHp), 0);

    public static int GetSpent(HpLossBudgetState state) =>
        Math.Max(0, state.SpentThisWindow);
}

internal readonly record struct HpLossBudgetState(int LastObservedHp, int SpentThisWindow);
