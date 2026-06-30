using Godot;

namespace STS2PartyWatch.UI;

internal static class PartyWatchNativeCoveringScreenTracker
{
    internal static readonly string[] CoveringScreenTypeNames =
    [
        "MegaCrit.Sts2.Core.Nodes.CommonUi.NSettingsScreenPopup",
        "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardGridSelectionScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NChooseACardSelectionScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCombatPileCardSelectScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NDeckCardSelectScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NDeckEnchantSelectScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NDeckTransformSelectScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NDeckUpgradeSelectScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NSimpleCardSelectScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen.NGameOverScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.Map.NMapScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.NDeckViewScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.NRewardsScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.PauseMenu.NPauseMenu",
        "MegaCrit.Sts2.Core.Nodes.Screens.Settings.NSettingsScreen",
        "MegaCrit.Sts2.Core.Nodes.Screens.Shops.NFakeMerchantInventory",
        "MegaCrit.Sts2.Core.Nodes.Screens.Shops.NMerchantInventory",
    ];

    private static readonly List<WeakReference<Node>> ActiveScreens = new();

    public static void MarkOpened(Node node)
    {
        Cleanup();
        if (ActiveScreens.Any(reference => reference.TryGetTarget(out var existing) && ReferenceEquals(existing, node)))
        {
            return;
        }

        ActiveScreens.Add(new WeakReference<Node>(node));
    }

    public static void MarkClosed(Node node)
    {
        for (var i = ActiveScreens.Count - 1; i >= 0; i--)
        {
            if (!ActiveScreens[i].TryGetTarget(out var existing)
                || !GodotObject.IsInstanceValid(existing)
                || ReferenceEquals(existing, node))
            {
                ActiveScreens.RemoveAt(i);
            }
        }
    }

    public static bool HasNativeCombatCoveringScreenOpen()
    {
        Cleanup();
        return ActiveScreens.Any(reference =>
            reference.TryGetTarget(out var node)
            && GodotObject.IsInstanceValid(node)
            && node.IsInsideTree()
            && (node is not CanvasItem canvasItem || canvasItem.IsVisibleInTree()));
    }

    private static void Cleanup()
    {
        for (var i = ActiveScreens.Count - 1; i >= 0; i--)
        {
            if (!ActiveScreens[i].TryGetTarget(out var node)
                || !GodotObject.IsInstanceValid(node)
                || !node.IsInsideTree())
            {
                ActiveScreens.RemoveAt(i);
            }
        }
    }
}
