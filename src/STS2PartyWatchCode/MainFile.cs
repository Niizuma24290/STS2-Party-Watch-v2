using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace STS2PartyWatch;

[ModInitializer(nameof(Initialize))]
public static class MainFile
{
    private const string HarmonyId = "sts2-party-watch-v2";
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        GD.Print("[STS2 Party Watch] Loaded");
        Console.WriteLine("[STS2 Party Watch] Loaded");
        new Harmony(HarmonyId).PatchAll(typeof(MainFile).Assembly);
    }
}
