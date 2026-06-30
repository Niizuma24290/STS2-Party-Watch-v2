using System;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2PartyWatch.Combat;
using STS2PartyWatch.Forecast;

namespace STS2PartyWatch.Patches;

[HarmonyPatch(typeof(NHealthBar))]
internal static class ForecastRefreshPatch
{
    private const string LabelName = "STS2PartyWatchForecastLabel";
    private const float RightPadding = 42f;
    private static readonly bool UseCompactHudLayout = false;
    private static readonly bool ShowHudBreakdownDetails = false;
    private static readonly float HudLabelWidth = ShowHudBreakdownDetails ? 240f : 96f;
    private static readonly float HudLabelHeight = ShowHudBreakdownDetails ? 84f : 42f;
    private static readonly FieldInfo? CreatureField = typeof(NHealthBar).GetField("_creature", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly LocalIncomingDamageReader Reader = new();
    private static readonly LocalDamageForecast Forecast = new();
    private static readonly List<WeakReference<NHealthBar>> RegisteredBars = new();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NHealthBar.SetCreature))]
    private static void SetCreaturePostfix(NHealthBar __instance)
    {
        Refresh(__instance, null);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NHealthBar.RefreshValues))]
    private static void RefreshValuesPostfix(NHealthBar __instance)
    {
        Refresh(__instance, null);
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetHpBarContainerSizeWithOffsets")]
    private static void ResizePostfix(NHealthBar __instance, Vector2 size)
    {
        Refresh(__instance, size);
    }

    private static void Refresh(NHealthBar bar, Vector2? containerSize)
    {
        if (!TryGetLocalCreature(bar, out var creature) || creature is null)
        {
            HideExisting(bar);
            return;
        }

        RegisterBar(bar);

        var label = GetOrCreateLabel(bar);
        if (label is null)
        {
            return;
        }

        Reposition(bar, label, containerSize);
        ObservedHpLossBudgetTracker.Observe(creature);

        var result = Forecast.Calculate(Reader.ReadForLocalCreature(creature));
        if (result.State != ForecastResultState.KnownDamage || (result.OutDamage <= 0 && result.DirectHpLoss <= 0))
        {
            label.Text = string.Empty;
            label.Hide();
            return;
        }

        label.Text = BuildForecastText(result);
        label.Show();
    }

    private static string BuildForecastText(ForecastResult result)
    {
        var blockablePrediction = result.OutDamage;
        var directHpLossPrediction = result.DirectHpLoss;
        var displayTotalPredictedHpLoss = blockablePrediction + directHpLossPrediction;
        if (displayTotalPredictedHpLoss <= 0)
        {
            return string.Empty;
        }

        if (!ShowHudBreakdownDetails)
        {
            return $"-{displayTotalPredictedHpLoss}";
        }

        var details = BuildForecastDetails(blockablePrediction, directHpLossPrediction);
        if (UseCompactHudLayout)
        {
            return string.IsNullOrEmpty(details)
                ? $"-{displayTotalPredictedHpLoss}"
                : $"-{displayTotalPredictedHpLoss}  {details}";
        }

        return string.IsNullOrEmpty(details)
            ? $"-{displayTotalPredictedHpLoss}"
            : $"-{displayTotalPredictedHpLoss}\n{details}";
    }

    private static string BuildForecastDetails(int blockablePrediction, int directHpLossPrediction)
    {
        var details = new List<string>(2);
        if (blockablePrediction > 0)
        {
            details.Add($"🛡 {blockablePrediction}");
        }

        if (directHpLossPrediction > 0)
        {
            details.Add($"♥ {directHpLossPrediction}");
        }

        return string.Join("   ", details);
    }

    private static bool TryGetLocalCreature(NHealthBar bar, out Creature? creature)
    {
        creature = GetCreature(bar);
        if (creature?.Player is null)
        {
            return false;
        }

        try
        {
            var localPlayer = LocalContext.GetMe(creature.CombatState);
            return localPlayer is not null && creature.Player.NetId == localPlayer.NetId;
        }
        catch
        {
            return false;
        }
    }

    private static Creature? GetCreature(NHealthBar bar)
    {
        return CreatureField?.GetValue(bar) as Creature;
    }

    private static Label? GetOrCreateLabel(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        if (parent is null)
        {
            return null;
        }

        var existing = parent.GetNodeOrNull<Label>(LabelName);
        if (existing is not null)
        {
            return existing;
        }

        var label = new Label
        {
            Name = LabelName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            CustomMinimumSize = new Vector2(HudLabelWidth, HudLabelHeight),
            Size = new Vector2(HudLabelWidth, HudLabelHeight),
            ZIndex = 50,
            Text = string.Empty,
            Visible = false,
        };
        label.AddThemeFontSizeOverride("font_size", 30);
        label.AddThemeColorOverride("font_color", Colors.White);
        label.AddThemeColorOverride("font_shadow_color", Colors.Black);
        label.AddThemeConstantOverride("shadow_offset_x", 2);
        label.AddThemeConstantOverride("shadow_offset_y", 2);

        parent.AddChild(label);
        return label;
    }

    private static Control? GetLabelParent(NHealthBar bar)
    {
        return bar.HpBarContainer?.GetParent() as Control ?? bar;
    }

    private static void Reposition(NHealthBar bar, Label label, Vector2? containerSize)
    {
        var container = bar.HpBarContainer;
        if (container is null)
        {
            return;
        }

        var size = containerSize ?? container.Size;
        label.Position = new Vector2(
            container.Position.X + size.X + RightPadding,
            container.Position.Y + MathF.Max(0f, (size.Y - label.Size.Y) * 0.5f));
    }

    private static void HideExisting(NHealthBar bar)
    {
        var existing = GetLabelParent(bar)?.GetNodeOrNull<Label>(LabelName);
        if (existing is not null)
        {
            existing.Text = string.Empty;
            existing.Hide();
        }
    }

    private static void RegisterBar(NHealthBar bar)
    {
        for (var i = RegisteredBars.Count - 1; i >= 0; i--)
        {
            if (!RegisteredBars[i].TryGetTarget(out var existing) || !GodotObject.IsInstanceValid(existing))
            {
                RegisteredBars.RemoveAt(i);
                continue;
            }

            if (ReferenceEquals(existing, bar))
            {
                return;
            }
        }

        RegisteredBars.Add(new WeakReference<NHealthBar>(bar));
    }

    internal static void RefreshRegisteredBars()
    {
        for (var i = RegisteredBars.Count - 1; i >= 0; i--)
        {
            if (!RegisteredBars[i].TryGetTarget(out var bar) || !GodotObject.IsInstanceValid(bar))
            {
                RegisteredBars.RemoveAt(i);
                continue;
            }

            Refresh(bar, null);
        }
    }
}

[HarmonyPatch(typeof(CardPile))]
internal static class ForecastHandChangePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CardPile.InvokeContentsChanged))]
    private static void InvokeContentsChangedPostfix(CardPile __instance)
    {
        if (__instance.Type == PileType.Hand)
        {
            ForecastRefreshPatch.RefreshRegisteredBars();
        }
    }
}

[HarmonyPatch(typeof(Player))]
internal static class ForecastRelicChangePatch
{
    [HarmonyPostfix]
    [HarmonyPatch("AddRelicInternal")]
    private static void AddRelicInternalPostfix()
    {
        ForecastRefreshPatch.RefreshRegisteredBars();
    }

    [HarmonyPostfix]
    [HarmonyPatch("RemoveRelicInternal")]
    private static void RemoveRelicInternalPostfix()
    {
        ForecastRefreshPatch.RefreshRegisteredBars();
    }

    [HarmonyPostfix]
    [HarmonyPatch("MeltRelicInternal")]
    private static void MeltRelicInternalPostfix()
    {
        ForecastRefreshPatch.RefreshRegisteredBars();
    }
}

[HarmonyPatch(typeof(BeatingRemnant))]
internal static class ForecastBeatingRemnantBudgetPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(BeatingRemnant.BeforeSideTurnStart))]
    private static void BeforeSideTurnStartPostfix(BeatingRemnant __instance, object[] __args)
    {
        if (__args.Length < 3 || __args[2] is not IReadOnlyList<Creature> creaturesStartingTurn)
        {
            return;
        }

        var owner = __instance.Owner;
        var ownerCreature = owner?.Creature;
        if (owner is null || ownerCreature is null || !creaturesStartingTurn.Contains(ownerCreature))
        {
            return;
        }

        ObservedHpLossBudgetTracker.ResetWindow(owner);
        ForecastRefreshPatch.RefreshRegisteredBars();
    }
}
