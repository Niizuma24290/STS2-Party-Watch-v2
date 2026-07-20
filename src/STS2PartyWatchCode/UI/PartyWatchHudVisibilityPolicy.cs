using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace STS2PartyWatch.UI;

internal static class PartyWatchHudVisibilityPolicy
{
    public static bool ShouldRenderHud(NHealthBar bar, Creature creature, out bool temporarilyCovered)
    {
        temporarilyCovered = false;
        if (!PartyWatchUiSettings.HudEnabled
            || !GodotObject.IsInstanceValid(bar)
            || !bar.IsInsideTree()
            || bar.HpBarContainer is null
            || !bar.HpBarContainer.IsVisibleInTree())
        {
            return false;
        }

        var combatState = creature.CombatState;
        if (combatState is null
            || !combatState.IsLiveCombat()
            || combatState.Players.Count <= 0
            || creature.Player is null
            || !creature.IsAlive)
        {
            return false;
        }

        if (combatState.Players.Count > 1 && !PartyWatchUiSettings.ShowLocalHudInMultiplayer)
        {
            return false;
        }

        try
        {
            var localPlayer = LocalContext.GetMe(combatState);
            if (localPlayer is null || creature.Player.NetId != localPlayer.NetId)
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

        temporarilyCovered = PartyWatchNativeCoveringScreenTracker.HasNativeCombatCoveringScreenOpen();
        return !temporarilyCovered;
    }
}
