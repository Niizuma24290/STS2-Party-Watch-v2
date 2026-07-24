namespace DamageForecast.Combat;

internal readonly record struct AvailableBlockInput(
    int CurrentBlock,
    int PowerBlock,
    int RelicBlock);

internal static class HpLossEventPolicy
{
    public static int SelectBlock(
        AvailableBlockInput available,
        IncomingDamageDisplayOptions options)
    {
        long selected = 0;
        if (options.IncludeCurrentBlock)
        {
            selected += Math.Max(0, available.CurrentBlock);
        }

        if (options.IncludePowerBlock)
        {
            selected += Math.Max(0, available.PowerBlock);
        }

        if (options.IncludeRelicBlock)
        {
            selected += Math.Max(0, available.RelicBlock);
        }

        return (int)Math.Min(int.MaxValue, selected);
    }

    public static List<UpcomingHpLossEvent> ApplySelectedBlock(
        IReadOnlyList<UpcomingHpLossEvent> sourceEvents,
        int selectedBlock)
    {
        var remainingBlock = Math.Max(0, selectedBlock);
        var hpLossEvents = new List<UpcomingHpLossEvent>(sourceEvents.Count);
        foreach (var entry in sourceEvents
                     .Select((hpLossEvent, index) => (Event: hpLossEvent, Index: index))
                     .OrderBy(entry => entry.Event.NativeExecutionOrder)
                     .ThenBy(entry => entry.Index))
        {
            var sourceEvent = entry.Event;
            if (sourceEvent.DisplayLane == HpLossDisplayLane.Blockable)
            {
                var amount = Math.Max(0, sourceEvent.VerifiedHpLoss);
                var hpLoss = Math.Max(0, amount - remainingBlock);
                remainingBlock = Math.Max(0, remainingBlock - amount);
                hpLossEvents.Add(sourceEvent with { VerifiedHpLoss = hpLoss });
            }
            else
            {
                hpLossEvents.Add(sourceEvent);
            }
        }

        return hpLossEvents;
    }
}
