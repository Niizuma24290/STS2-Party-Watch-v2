using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace STS2PartyWatch.UI;

internal static class PartyWatchHudVisibilityPolicy
{
    public static bool ShouldRenderHud(NHealthBar bar, Creature creature)
    {
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
            || combatState.Players.Count != 1
            || creature.Player is null
            || !creature.IsAlive)
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

        var tree = bar.GetTree();
        return tree?.Root is not null && !HasBlockingUi(tree.Root, bar);
    }

    private static bool HasBlockingUi(Node node, Node healthBar)
    {
        var skipCurrentNode = ReferenceEquals(node, healthBar) || IsAncestorOf(node, healthBar);
        if (!skipCurrentNode
            && node is CanvasItem canvasItem
            && canvasItem.IsVisibleInTree()
            && IsBlockingUiNode(node))
        {
            return true;
        }

        foreach (var child in node.GetChildren())
        {
            if (child is Node childNode && HasBlockingUi(childNode, healthBar))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsBlockingUiNode(Node node)
    {
        var typeName = node.GetType().FullName ?? string.Empty;
        if (typeName.StartsWith("MegaCrit.Sts2.Core.Nodes.Screens.", StringComparison.Ordinal)
            && !typeName.Contains(".Combat", StringComparison.Ordinal))
        {
            return true;
        }

        if (typeName.StartsWith("MegaCrit.Sts2.Core.Nodes.CommonUi.", StringComparison.Ordinal)
            && (typeName.Contains("Modal", StringComparison.Ordinal)
                || typeName.Contains("Popup", StringComparison.Ordinal)
                || typeName.Contains("Overlay", StringComparison.Ordinal)))
        {
            return true;
        }

        return false;
    }

    private static bool IsAncestorOf(Node possibleAncestor, Node node)
    {
        for (var current = node.GetParent(); current is not null; current = current.GetParent())
        {
            if (ReferenceEquals(current, possibleAncestor))
            {
                return true;
            }
        }

        return false;
    }
}
