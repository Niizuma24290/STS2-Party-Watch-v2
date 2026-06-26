using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2PartyWatch.UI;

namespace STS2PartyWatch.Patches;

[HarmonyPatch(typeof(NCombatUi))]
internal static class ForecastRefreshPatch
{
    private const string ControllerName = "STS2PartyWatchForecastHud";

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCombatUi.Activate))]
    private static void ActivatePostfix(NCombatUi __instance, CombatState state)
    {
        var controller = GetOrCreateController(__instance);
        controller.SetCombatState(state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(NCombatUi.Deactivate))]
    private static void DeactivatePrefix(NCombatUi __instance)
    {
        var existing = __instance.GetNodeOrNull<ForecastHudController>(ControllerName);
        existing?.Clear();
    }

    private static ForecastHudController GetOrCreateController(NCombatUi ui)
    {
        var existing = ui.GetNodeOrNull<ForecastHudController>(ControllerName);
        if (existing is not null)
        {
            return existing;
        }

        var controller = ForecastHudController.Create(null!);
        controller.Name = ControllerName;
        ui.AddChild(controller);
        return controller;
    }
}
