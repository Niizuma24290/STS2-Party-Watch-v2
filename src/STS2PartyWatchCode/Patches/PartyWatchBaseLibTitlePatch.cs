using System.Reflection;
using BaseLib.Config;
using BaseLib.Config.UI;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using STS2PartyWatch.Settings;

namespace STS2PartyWatch.Patches;

[HarmonyPatch]
internal static class PartyWatchBaseLibTitlePatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var target = ResolveTargetMethod();
        if (target is not null)
        {
            yield return target;
        }
    }

    private static MethodBase? ResolveTargetMethod()
    {
        return AccessTools.DeclaredMethod(
            typeof(NModConfigSubmenu),
            "GetModTitle",
            [typeof(ModConfig)]);
    }

    internal static bool HasCompatibleTarget()
    {
        return ResolveTargetMethod() is not null
            && ResolvePageLoadTarget() is not null
            && ResolvePageTitleField() is not null;
    }

    private static void Postfix(ModConfig __0, ref string __result)
    {
        __result = ResolveListTitle(__0.ModId, __result);
    }

    internal static string ResolveListTitle(
        string? modId,
        string fallbackTitle)
    {
        return string.Equals(modId, MainFile.ModId, StringComparison.Ordinal)
            ? PartyWatchConfigText.EnglishProductName
            : fallbackTitle;
    }

    internal static string ResolvePageTitle(
        string? modId,
        PartyWatchConfigLanguage language,
        string fallbackTitle)
    {
        return string.Equals(modId, MainFile.ModId, StringComparison.Ordinal)
            ? PartyWatchConfigText.ProductName(language)
            : fallbackTitle;
    }

    internal static bool RefreshPageTitle(
        Control? optionContainer,
        PartyWatchConfigLanguage language)
    {
        if (FindSubmenu(optionContainer) is not { } submenu
            || ResolvePageTitleField()?.GetValue(submenu) is not MegaRichTextLabel pageTitle)
        {
            return false;
        }

        pageTitle.SetTextAutoSize($"[center]{PartyWatchConfigText.ProductName(language)}[/center]");
        return true;
    }

    private static NModConfigSubmenu? FindSubmenu(Node? node)
    {
        while (node is not null)
        {
            if (node is NModConfigSubmenu submenu)
            {
                return submenu;
            }

            node = node.GetParent();
        }

        return null;
    }

    private static FieldInfo? ResolvePageTitleField()
    {
        return AccessTools.DeclaredField(typeof(NModConfigSubmenu), "_modTitle");
    }

    internal static MethodBase? ResolvePageLoadTarget()
    {
        return AccessTools.DeclaredMethod(
            typeof(NModConfigSubmenu),
            "LoadModConfig",
            [typeof(ModConfig)]);
    }
}

[HarmonyPatch]
internal static class PartyWatchBaseLibPageTitlePatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var target = PartyWatchBaseLibTitlePatch.ResolvePageLoadTarget();
        if (target is not null)
        {
            yield return target;
        }
    }

    private static void Postfix(NModConfigSubmenu __instance, ModConfig __0)
    {
        if (string.Equals(__0.ModId, MainFile.ModId, StringComparison.Ordinal))
        {
            PartyWatchBaseLibTitlePatch.RefreshPageTitle(
                __instance,
                PartyWatchBaseLibConfig.ConfigLanguage);
        }
    }
}
