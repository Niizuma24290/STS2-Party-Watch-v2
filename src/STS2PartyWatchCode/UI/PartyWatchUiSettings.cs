using Godot;

namespace STS2PartyWatch.UI;

internal static class PartyWatchUiSettings
{
    public static event Action? Changed;

    private static bool _hudEnabled = true;
    private static bool _showBreakdownDetails;
    private static bool _freezeHudWithinPlayerTurn = true;
    private static PartyWatchHudAnchor _hudAnchor = PartyWatchHudAnchor.HealthBarRight;
    private static float _offsetX = -32f;
    private static float _offsetY;
    private static Color _totalLossColor = Colors.White;
    private static Color _blockableDetailColor = new(0.55f, 0.85f, 1f);
    private static Color _directHpLossDetailColor = new(1f, 0.55f, 0.62f);

    public static bool HudEnabled => _hudEnabled;

    public static bool ShowBreakdownDetails => _showBreakdownDetails;

    public static bool FreezeHudWithinPlayerTurn => _freezeHudWithinPlayerTurn;

    public static PartyWatchHudAnchor HudAnchor => _hudAnchor;

    public static float OffsetX => _offsetX;

    public static float OffsetY => _offsetY;

    public static Color TotalLossColor => _totalLossColor;

    public static Color BlockableDetailColor => _blockableDetailColor;

    public static Color DirectHpLossDetailColor => _directHpLossDetailColor;

    public static void SetHudEnabled(bool value) => Set(ref _hudEnabled, value);

    public static void SetShowBreakdownDetails(bool value) => Set(ref _showBreakdownDetails, value);

    public static void SetFreezeHudWithinPlayerTurn(bool value) => Set(ref _freezeHudWithinPlayerTurn, value);

    public static void SetHudAnchor(PartyWatchHudAnchor value) => Set(ref _hudAnchor, value);

    public static void SetOffsetX(float value) => Set(ref _offsetX, Math.Clamp(value, -320f, 320f));

    public static void SetOffsetY(float value) => Set(ref _offsetY, Math.Clamp(value, -240f, 240f));

    public static void SetTotalLossColor(Color value) => Set(ref _totalLossColor, value);

    public static void SetBlockableDetailColor(Color value) => Set(ref _blockableDetailColor, value);

    public static void SetDirectHpLossDetailColor(Color value) => Set(ref _directHpLossDetailColor, value);

    public static void ResetDefaults()
    {
        _hudEnabled = true;
        _showBreakdownDetails = false;
        _freezeHudWithinPlayerTurn = true;
        _hudAnchor = PartyWatchHudAnchor.HealthBarRight;
        _offsetX = -32f;
        _offsetY = 0f;
        _totalLossColor = Colors.White;
        _blockableDetailColor = new Color(0.55f, 0.85f, 1f);
        _directHpLossDetailColor = new Color(1f, 0.55f, 0.62f);
        Changed?.Invoke();
    }

    private static void Set<T>(ref T field, T value)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        Changed?.Invoke();
    }
}

internal enum PartyWatchHudAnchor
{
    HealthBarRight,
    HealthBarLeft,
    HealthBarAbove,
    HealthBarBelow
}
