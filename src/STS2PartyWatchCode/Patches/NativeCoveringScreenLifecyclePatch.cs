using System.Reflection;
#if PARTY_WATCH_VISIBILITY_PROFILING
using System.Diagnostics;
using STS2PartyWatch.Diagnostics;
#endif
using Godot;
using HarmonyLib;
using STS2PartyWatch.UI;

namespace STS2PartyWatch.Patches;

[HarmonyPatch]
internal static class NativeCoveringScreenLifecyclePatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var seen = new HashSet<MethodBase>();
        foreach (var typeName in PartyWatchNativeCoveringScreenTracker.CoveringScreenTypeNames)
        {
            var type = AccessTools.TypeByName(typeName);
            if (type is null)
            {
                continue;
            }

            var ready = AccessTools.DeclaredMethod(type, "_Ready");
            if (ready is not null && seen.Add(ready))
            {
                yield return ready;
            }

            var enter = AccessTools.DeclaredMethod(type, "_EnterTree");
            if (enter is not null && seen.Add(enter))
            {
                yield return enter;
            }

            var exit = AccessTools.DeclaredMethod(type, "_ExitTree");
            if (exit is not null && seen.Add(exit))
            {
                yield return exit;
            }
        }
    }

    private static void Postfix(Node __instance, MethodBase __originalMethod)
    {
        if (__originalMethod.Name is "_Ready" or "_EnterTree")
        {
            PartyWatchNativeCoveringScreenTracker.MarkOpened(__instance);
        }
        else if (__originalMethod.Name == "_ExitTree")
        {
            PartyWatchNativeCoveringScreenTracker.MarkClosed(__instance);
        }

        ForecastRefreshPatch.RefreshRegisteredBars();
    }
}

#if !PARTY_WATCH_DISABLE_GLOBAL_VISIBILITY_PATCHES
[HarmonyPatch(typeof(CanvasItem))]
internal static class NativeCoveringScreenVisibilityPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CanvasItem.Show))]
    private static void ShowPostfix(CanvasItem __instance)
    {
        UpdateCoveringScreenVisibility(
            __instance,
            visible: true
#if PARTY_WATCH_VISIBILITY_PROFILING
            , VisibilityPostfixKind.Show
#endif
        );
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CanvasItem.Hide))]
    private static void HidePostfix(CanvasItem __instance)
    {
        UpdateCoveringScreenVisibility(
            __instance,
            visible: false
#if PARTY_WATCH_VISIBILITY_PROFILING
            , VisibilityPostfixKind.Hide
#endif
        );
    }

    [HarmonyPostfix]
    [HarmonyPatch("set_Visible")]
    private static void SetVisiblePostfix(CanvasItem __instance, bool value)
    {
        UpdateCoveringScreenVisibility(
            __instance,
            value
#if PARTY_WATCH_VISIBILITY_PROFILING
            , VisibilityPostfixKind.SetVisible
#endif
        );
    }

    private static void UpdateCoveringScreenVisibility(
        CanvasItem instance,
        bool visible
#if PARTY_WATCH_VISIBILITY_PROFILING
        , VisibilityPostfixKind kind
#endif
    )
    {
#if PARTY_WATCH_VISIBILITY_PROFILING
        var callbackStartedAt = Stopwatch.GetTimestamp();
        var typeCheckStartedAt = Stopwatch.GetTimestamp();
        var matched = PartyWatchNativeCoveringScreenTracker.IsNativeCombatCoveringScreen(instance);
        var typeCheckTicks = Stopwatch.GetTimestamp() - typeCheckStartedAt;
        var transitioned = false;
        var refreshed = false;
        var refreshTicks = 0L;
        var barsVisited = 0;

        if (matched)
        {
            var wasTrackedOpen = PartyWatchNativeCoveringScreenTracker.IsTrackedOpenForVisibilityProfiling(instance);
            if (visible)
            {
                PartyWatchNativeCoveringScreenTracker.MarkOpened(instance);
                transitioned = !wasTrackedOpen;
            }
            else
            {
                PartyWatchNativeCoveringScreenTracker.MarkClosed(instance);
                transitioned = wasTrackedOpen;
            }

            var refreshStartedAt = Stopwatch.GetTimestamp();
            ForecastRefreshPatch.RefreshRegisteredBars();
            refreshTicks = Stopwatch.GetTimestamp() - refreshStartedAt;
            barsVisited = ForecastRefreshPatch.LastRegisteredBarsVisitedForVisibilityProfiling;
            refreshed = true;
        }

        Aud0007VisibilityProfiler.Record(
            kind,
            matched,
            transitioned,
            typeCheckTicks,
            Stopwatch.GetTimestamp() - callbackStartedAt,
            refreshed,
            refreshTicks,
            barsVisited);

        if (matched && !visible && transitioned)
        {
            Aud0007VisibilityProfiler.Dump("cover-close", reset: false);
        }
#else
        if (!PartyWatchNativeCoveringScreenTracker.IsNativeCombatCoveringScreen(instance))
        {
            return;
        }

        if (visible)
        {
            PartyWatchNativeCoveringScreenTracker.MarkOpened(instance);
        }
        else
        {
            PartyWatchNativeCoveringScreenTracker.MarkClosed(instance);
        }

        ForecastRefreshPatch.RefreshRegisteredBars();
#endif
    }
}
#endif
