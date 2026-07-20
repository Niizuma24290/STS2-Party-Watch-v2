using Godot;
using HarmonyLib;
using BaseLib.Config;
using MegaCrit.Sts2.Core.Modding;
#if PARTY_WATCH_VISIBILITY_PROFILING
using STS2PartyWatch.Diagnostics;
#endif
using STS2PartyWatch.Settings;

namespace STS2PartyWatch;

[ModInitializer(nameof(Initialize))]
public static class MainFile
{
    internal const string ModId = "sts2-party-watch-v2";
    private static bool _initialized;
    private static bool _initializing;

    public static void Initialize()
    {
        if (_initialized || _initializing)
        {
            return;
        }

        _initializing = true;
        try
        {
            var config = new PartyWatchBaseLibConfig();
            PartyWatchSettingsAdapter.Bind(config);
            ModConfigRegistry.Register(ModId, config);
            PartyWatchSettingsAdapter.ApplyCurrent();

            new Harmony(ModId).PatchAll(typeof(MainFile).Assembly);
#if PARTY_WATCH_VISIBILITY_PROFILING
            Aud0007VisibilityProfiler.Start();
#endif
            GD.Print("[STS2 Party Watch] Loaded");
            Console.WriteLine("[STS2 Party Watch] Loaded");
            _initialized = true;
        }
        finally
        {
            _initializing = false;
        }
    }
}
