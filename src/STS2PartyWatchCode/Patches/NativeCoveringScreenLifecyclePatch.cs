using System.Reflection;
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
