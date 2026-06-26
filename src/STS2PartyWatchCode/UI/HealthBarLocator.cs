using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace STS2PartyWatch.UI;

internal static class HealthBarLocator
{
    private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    public static bool TryGetGlobalRect(NCreature creatureNode, out Rect2 rect)
    {
        rect = default;

        var stateDisplay = typeof(NCreature)
            .GetField("_stateDisplay", InstanceFlags)?
            .GetValue(creatureNode);
        if (stateDisplay is null)
        {
            return false;
        }

        var healthBar = stateDisplay.GetType()
            .GetField("_healthBar", InstanceFlags)?
            .GetValue(stateDisplay) as Control;
        if (healthBar is null)
        {
            return false;
        }

        rect = healthBar.GetGlobalRect();
        return rect.Size.X > 0 && rect.Size.Y > 0;
    }
}
