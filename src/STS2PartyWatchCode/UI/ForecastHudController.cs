using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2PartyWatch.Combat;
using STS2PartyWatch.Forecast;

namespace STS2PartyWatch.UI;

public sealed partial class ForecastHudController : Control
{
    private const double RefreshIntervalSeconds = 0.1;
    private readonly LocalIncomingDamageReader _reader = new();
    private readonly LocalDamageForecast _forecast = new();
    private readonly ForecastHudView _view = new();
    private ICombatState? _combatState;
    private double _elapsed;

    public static ForecastHudController Create(ICombatState combatState)
    {
        var controller = new ForecastHudController();
        controller.SetCombatState(combatState);
        return controller;
    }

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        SetAnchorsPreset(LayoutPreset.TopLeft);
        AddChild(_view);
        Hide();
    }

    public override void _ExitTree()
    {
        _combatState = null;
    }

    public override void _Process(double delta)
    {
        _elapsed += delta;
        if (_elapsed < RefreshIntervalSeconds)
        {
            return;
        }

        _elapsed = 0;
        Refresh();
    }

    public void SetCombatState(ICombatState combatState)
    {
        _combatState = combatState;
        Refresh();
    }

    public void Clear()
    {
        _combatState = null;
        _view.Apply(ForecastResult.Hidden);
        Hide();
    }

    private void Refresh()
    {
        if (_combatState is null)
        {
            Clear();
            return;
        }

        var result = _forecast.Calculate(_reader.Read(_combatState));
        if (result.State != ForecastResultState.KnownDamage)
        {
            _view.Apply(result);
            Hide();
            return;
        }

        PositionNearLocalHealthBar();
        _view.Apply(result);
        Show();
    }

    private void PositionNearLocalHealthBar()
    {
        var room = NCombatRoom.Instance;
        var localCreature = LocalContext.GetMe(_combatState!)?.Creature;
        if (room is null || localCreature is null)
        {
            Position = new Vector2(220, 96);
            return;
        }

        var creatureNode = room.GetCreatureNode(localCreature);
        if (creatureNode is null)
        {
            Position = new Vector2(220, 96);
            return;
        }

        var anchor = HealthBarLocator.TryGetGlobalRect(creatureNode, out var healthBarRect)
            ? new Vector2(healthBarRect.Position.X + healthBarRect.Size.X + 42, healthBarRect.Position.Y - 4)
            : new Vector2(creatureNode.GlobalPosition.X + 300, creatureNode.GlobalPosition.Y + 220);

        GlobalPosition = anchor;
    }
}
