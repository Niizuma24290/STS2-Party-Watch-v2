using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using STS2PartyWatch.Diagnostics;

namespace STS2PartyWatch.Combat;

public sealed class LocalIncomingDamageReader
{
    public IncomingDamageRead Read(ICombatState? combatState)
    {
        if (combatState is null || !combatState.IsLiveCombat())
        {
            return IncomingDamageRead.Hidden;
        }

        var localPlayer = LocalContext.GetMe(combatState);
        var localCreature = localPlayer?.Creature;
        if (localCreature is null || !localCreature.IsAlive)
        {
            return IncomingDamageRead.Hidden;
        }

        return ReadKnown(combatState, localCreature);
    }

    public IncomingDamageRead ReadForLocalCreature(Creature? localCreature)
    {
        var combatState = localCreature?.CombatState;
        if (combatState is null || !combatState.IsLiveCombat())
        {
            return IncomingDamageRead.Hidden;
        }

        if (localCreature is null || !localCreature.IsAlive)
        {
            return IncomingDamageRead.Hidden;
        }

        try
        {
            var localPlayer = LocalContext.GetMe(combatState);
            if (localPlayer is null || localCreature.Player?.NetId != localPlayer.NetId)
            {
                return IncomingDamageRead.Hidden;
            }
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("incoming.local-context.expected", exception);
            return IncomingDamageRead.Unknown;
        }

        return ReadKnown(combatState, localCreature);
    }

    internal IncomingDamageDisplayRead ReadIncomingDamageForLocalCreature(
        Creature? localCreature,
        IncomingDamageDisplayOptions options)
    {
        var combatState = localCreature?.CombatState;
        if (combatState is null || !combatState.IsLiveCombat())
        {
            return IncomingDamageDisplayRead.Hidden;
        }

        if (localCreature is null || !localCreature.IsAlive)
        {
            return IncomingDamageDisplayRead.Hidden;
        }

        try
        {
            var localPlayer = LocalContext.GetMe(combatState);
            if (localPlayer is null || localCreature.Player?.NetId != localPlayer.NetId)
            {
                return IncomingDamageDisplayRead.Hidden;
            }
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("incoming.local-context.display", exception);
            return IncomingDamageDisplayRead.Unknown;
        }

        return ReadIncomingDamageKnown(combatState, localCreature, options);
    }

    private static IncomingDamageRead ReadKnown(ICombatState combatState, Creature localCreature)
    {
        if (localCreature.Player is null)
        {
            return IncomingDamageRead.Unknown;
        }

        var raw = 0;
        var foundDamage = false;
        var enemyAttackEvents = new List<BlockableFutureDamageEvent>();
        var enemyAttackOrder = 0;
        var enemySnapshotIndex = 0;

        foreach (var enemy in combatState.Enemies)
        {
            var snapshotIndex = enemySnapshotIndex++;
            if (enemy is null || !enemy.IsAlive || enemy.Monster?.NextMove?.Intents is null)
            {
                continue;
            }

            var attackIntents = enemy.Monster.NextMove.Intents.OfType<AttackIntent>().ToArray();
            if (attackIntents.Length == 0)
            {
                continue;
            }

            var enemyIntentContribution = 0;
            var intentTotals = new int[attackIntents.Length];
            for (var i = 0; i < attackIntents.Length; i++)
            {
                intentTotals[i] = attackIntents[i].GetTotalDamage(new[] { localCreature }, enemy);
                enemyIntentContribution += intentTotals[i];
            }

            var survivalPreview = EnemyPreActionSurvivalPreview.Preview(enemy, snapshotIndex, enemyIntentContribution);
            if (!survivalPreview.WillExecuteCurrentIntent)
            {
                continue;
            }

            for (var i = 0; i < attackIntents.Length; i++)
            {
                foundDamage = true;
                var attackIntent = attackIntents[i];
                var totalDamage = intentTotals[i];
                var attackEvents = ReadEnemyAttackEvents(
                    localCreature.Player,
                    attackIntent,
                    localCreature,
                    enemy,
                    survivalPreview.State.Identity,
                    enemyAttackOrder,
                    totalDamage,
                    out var modificationState);
                if (modificationState != EnemyDamageModificationState.Supported)
                {
                    return ReadKnownWithUnsupportedEnemyDamage(localCreature);
                }

                raw += attackEvents.Sum(e => e.Amount);
                enemyAttackEvents.AddRange(attackEvents);
                enemyAttackOrder++;
            }
        }

        if (HasVerifiedHpLossResultModifier(localCreature.Player, localCreature))
        {
            return ReadKnownWithHpLossResultModifiers(localCreature.Player, localCreature, enemyAttackEvents, foundDamage);
        }

        if (TryReadHandTurnEndDamage(localCreature.Player, localCreature, out var handTurnEndDamage))
        {
            raw += handTurnEndDamage;
            foundDamage = foundDamage || handTurnEndDamage > 0;
        }
        else
        {
            return IncomingDamageRead.Unknown;
        }

        if (VerifiedTurnEndPowerDamageReader.TryRead(localCreature, out _, out var turnEndPowerDamage))
        {
            raw += turnEndPowerDamage;
            foundDamage = foundDamage || turnEndPowerDamage > 0;
        }
        else
        {
            return IncomingDamageRead.Unknown;
        }

        if (!VerifiedFixedTurnEndHpLossReader.TryRead(localCreature.Player, out var directHpLoss))
        {
            return IncomingDamageRead.Unknown;
        }

        if (!foundDamage && directHpLoss <= 0)
        {
            return IncomingDamageRead.Hidden;
        }

        if (!foundDamage)
        {
            return IncomingDamageRead.Known(raw, localCreature.Block, directHpLoss);
        }

        var preAttackBlock = VerifiedPreAttackBlockReader.Read(localCreature.Player, localCreature);
        if (preAttackBlock.State != PreAttackBlockReadState.Known)
        {
            return directHpLoss > 0
                ? IncomingDamageRead.UnknownDirect(directHpLoss)
                : IncomingDamageRead.Unknown;
        }

        return IncomingDamageRead.Known(raw, localCreature.Block + preAttackBlock.Block, directHpLoss);
    }

    private static IncomingDamageDisplayRead ReadIncomingDamageKnown(
        ICombatState combatState,
        Creature localCreature,
        IncomingDamageDisplayOptions options)
    {
        var player = localCreature.Player;
        if (player is null)
        {
            return IncomingDamageDisplayRead.Unknown;
        }

        var events = new List<UpcomingHpLossEvent>();
        var foundDamage = false;
        var enemyAttackOrder = 0;
        var enemySnapshotIndex = 0;

        foreach (var enemy in combatState.Enemies)
        {
            var snapshotIndex = enemySnapshotIndex++;
            if (enemy is null || !enemy.IsAlive || enemy.Monster?.NextMove?.Intents is null)
            {
                continue;
            }

            var attackIntents = enemy.Monster.NextMove.Intents.OfType<AttackIntent>().ToArray();
            if (attackIntents.Length == 0)
            {
                continue;
            }

            var enemyIntentContribution = 0;
            var intentTotals = new int[attackIntents.Length];
            for (var i = 0; i < attackIntents.Length; i++)
            {
                intentTotals[i] = attackIntents[i].GetTotalDamage(new[] { localCreature }, enemy);
                enemyIntentContribution += intentTotals[i];
            }

            var survivalPreview = EnemyPreActionSurvivalPreview.Preview(enemy, snapshotIndex, enemyIntentContribution);
            if (!survivalPreview.WillExecuteCurrentIntent)
            {
                continue;
            }

            for (var i = 0; i < attackIntents.Length; i++)
            {
                var attackEvents = ReadEnemyAttackEvents(
                    player,
                    attackIntents[i],
                    localCreature,
                    enemy,
                    survivalPreview.State.Identity,
                    enemyAttackOrder,
                    intentTotals[i],
                    out var modificationState);
                if (modificationState != EnemyDamageModificationState.Supported)
                {
                    return IncomingDamageDisplayRead.Unknown;
                }

                foreach (var attackEvent in attackEvents)
                {
                    foundDamage = foundDamage || attackEvent.Amount > 0;
                    events.Add(new UpcomingHpLossEvent(
                        attackEvent.Source,
                        attackEvent.NativeExecutionOrder,
                        HpLossDisplayLane.Blockable,
                        attackEvent.Amount,
                        attackEvent.IsSingleVerifiedEvent));
                }

                enemyAttackOrder++;
            }
        }

        if (!TryReadOrderedHandTurnEndEvents(player, localCreature, out var handEvents, out _))
        {
            return IncomingDamageDisplayRead.Unknown;
        }

        foreach (var handEvent in handEvents)
        {
            foundDamage = foundDamage || handEvent.Amount > 0;
            events.Add(new UpcomingHpLossEvent(
                handEvent.Source,
                handEvent.NativeExecutionOrder,
                handEvent.DisplayLane,
                handEvent.Amount,
                handEvent.IsSingleVerifiedEvent));
        }

        if (!VerifiedTurnEndPowerDamageReader.TryRead(localCreature, out var powerEvents, out _))
        {
            return IncomingDamageDisplayRead.Unknown;
        }

        foreach (var powerEvent in powerEvents)
        {
            foundDamage = foundDamage || powerEvent.Amount > 0;
            events.Add(new UpcomingHpLossEvent(
                powerEvent.Source,
                powerEvent.NativeExecutionOrder,
                HpLossDisplayLane.Blockable,
                powerEvent.Amount,
                powerEvent.IsSingleVerifiedEvent));
        }

        if (!foundDamage)
        {
            return IncomingDamageDisplayRead.Hidden;
        }

        var selectedBlock = 0;
        if (options.IncludeCurrentBlock)
        {
            selectedBlock += Math.Max(0, localCreature.Block);
        }

        if (options.IncludePowerBlock || options.IncludeRelicBlock)
        {
            var preAttackBlock = VerifiedPreAttackBlockReader.Read(player, localCreature);
            if (preAttackBlock.State != PreAttackBlockReadState.Known)
            {
                return IncomingDamageDisplayRead.Unknown;
            }

            if (options.IncludePowerBlock)
            {
                selectedBlock += preAttackBlock.PowerBlock;
            }

            if (options.IncludeRelicBlock)
            {
                selectedBlock += preAttackBlock.RelicBlock;
            }
        }

        var hpLossEvents = ApplySelectedBlock(events, selectedBlock);
        if (options.IncludePowerHpLossModifiers || options.IncludeRelicHpLossModifiers)
        {
            var modified = VerifiedHpLossResultModifier.Apply(
                player,
                localCreature,
                hpLossEvents,
                ObservedHpLossBudgetTracker.GetSpent(player),
                options.IncludePowerHpLossModifiers,
                options.IncludeRelicHpLossModifiers);
            return modified.State == HpLossResultModificationState.Supported
                ? IncomingDamageDisplayRead.Known(modified.BlockableHpLoss + modified.DirectHpLoss)
                : IncomingDamageDisplayRead.Unknown;
        }

        return IncomingDamageDisplayRead.Known(hpLossEvents.Sum(e => Math.Max(0, e.VerifiedHpLoss)));
    }

    private static List<UpcomingHpLossEvent> ApplySelectedBlock(
        IReadOnlyList<UpcomingHpLossEvent> sourceEvents,
        int selectedBlock)
    {
        var remainingBlock = Math.Max(0, selectedBlock);
        var hpLossEvents = new List<UpcomingHpLossEvent>(sourceEvents.Count);
        foreach (var sourceEvent in sourceEvents.OrderBy(e => e.NativeExecutionOrder))
        {
            if (sourceEvent.DisplayLane == HpLossDisplayLane.Blockable)
            {
                var amount = Math.Max(0, sourceEvent.VerifiedHpLoss);
                var hpLoss = Math.Max(0, amount - remainingBlock);
                remainingBlock = Math.Max(0, remainingBlock - amount);
                hpLossEvents.Add(new UpcomingHpLossEvent(
                    sourceEvent.Source,
                    sourceEvent.NativeExecutionOrder,
                    HpLossDisplayLane.Blockable,
                    hpLoss,
                    sourceEvent.IsSingleVerifiedEvent));
            }
            else
            {
                hpLossEvents.Add(sourceEvent);
            }
        }

        return hpLossEvents;
    }

    private static IncomingDamageRead ReadKnownWithUnsupportedEnemyDamage(Creature localCreature)
    {
        var player = localCreature.Player;
        if (player is null || HasVerifiedHpLossRelic(player))
        {
            return IncomingDamageRead.Unknown;
        }

        if (HasActiveIntangiblePower(localCreature))
        {
            if (!VerifiedFixedTurnEndHpLossReader.TryReadEvents(player, out var directEvents))
            {
                return IncomingDamageRead.Unknown;
            }

            var modified = VerifiedHpLossResultModifier.Apply(player, localCreature, directEvents, 0);
            return modified.State == HpLossResultModificationState.Supported && modified.DirectHpLoss > 0
                ? IncomingDamageRead.UnknownDirect(modified.DirectHpLoss)
                : IncomingDamageRead.Unknown;
        }

        if (!VerifiedFixedTurnEndHpLossReader.TryRead(player, out var directHpLoss))
        {
            return IncomingDamageRead.Unknown;
        }

        return directHpLoss > 0
            ? IncomingDamageRead.UnknownDirect(directHpLoss)
            : IncomingDamageRead.Unknown;
    }

    private static IncomingDamageRead ReadKnownWithHpLossResultModifiers(
        Player player,
        Creature localCreature,
        IReadOnlyList<BlockableFutureDamageEvent> enemyAttackEvents,
        bool foundEnemyAttack)
    {
        if (!TryReadOrderedHandTurnEndEvents(player, localCreature, out var handEvents, out var handBlockableRaw))
        {
            return IncomingDamageRead.Unknown;
        }

        if (!VerifiedTurnEndPowerDamageReader.TryRead(localCreature, out var powerEvents, out var powerBlockableRaw))
        {
            return IncomingDamageRead.Unknown;
        }

        if (handEvents.Count == 0 && powerEvents.Count == 0 && !foundEnemyAttack)
        {
            return IncomingDamageRead.Hidden;
        }

        var blockableRaw = handBlockableRaw + powerBlockableRaw + enemyAttackEvents.Sum(e => e.Amount);
        var remainingBlock = localCreature.Block;
        if (blockableRaw > 0)
        {
            var preAttackBlock = VerifiedPreAttackBlockReader.Read(player, localCreature);
            if (preAttackBlock.State != PreAttackBlockReadState.Known)
            {
                return IncomingDamageRead.Unknown;
            }

            remainingBlock += preAttackBlock.Block;
        }

        var hpLossEvents = new List<UpcomingHpLossEvent>(handEvents.Count + enemyAttackEvents.Count);
        foreach (var handEvent in handEvents)
        {
            if (handEvent.DisplayLane == HpLossDisplayLane.Blockable)
            {
                var hpLoss = Math.Max(0, handEvent.Amount - remainingBlock);
                remainingBlock = Math.Max(0, remainingBlock - handEvent.Amount);
                hpLossEvents.Add(new UpcomingHpLossEvent(
                    handEvent.Source,
                    handEvent.NativeExecutionOrder,
                    HpLossDisplayLane.Blockable,
                    hpLoss,
                    handEvent.IsSingleVerifiedEvent));
            }
            else
            {
                hpLossEvents.Add(new UpcomingHpLossEvent(
                    handEvent.Source,
                    handEvent.NativeExecutionOrder,
                    HpLossDisplayLane.DirectHpLoss,
                    handEvent.Amount,
                    handEvent.IsSingleVerifiedEvent));
            }
        }

        foreach (var powerEvent in powerEvents)
        {
            var hpLoss = Math.Max(0, powerEvent.Amount - remainingBlock);
            remainingBlock = Math.Max(0, remainingBlock - powerEvent.Amount);
            hpLossEvents.Add(new UpcomingHpLossEvent(
                powerEvent.Source,
                powerEvent.NativeExecutionOrder,
                HpLossDisplayLane.Blockable,
                hpLoss,
                powerEvent.IsSingleVerifiedEvent));
        }

        foreach (var enemyEvent in enemyAttackEvents)
        {
            var hpLoss = Math.Max(0, enemyEvent.Amount - remainingBlock);
            remainingBlock = Math.Max(0, remainingBlock - enemyEvent.Amount);
            hpLossEvents.Add(new UpcomingHpLossEvent(
                enemyEvent.Source,
                enemyEvent.NativeExecutionOrder,
                HpLossDisplayLane.Blockable,
                hpLoss,
                enemyEvent.IsSingleVerifiedEvent));
        }

        var modified = VerifiedHpLossResultModifier.Apply(
            player,
            localCreature,
            hpLossEvents,
            ObservedHpLossBudgetTracker.GetSpent(player));
        if (modified.State == HpLossResultModificationState.Supported)
        {
            return modified.BlockableHpLoss > 0 || modified.DirectHpLoss > 0
                ? IncomingDamageRead.Known(modified.BlockableHpLoss, 0, modified.DirectHpLoss)
                : IncomingDamageRead.Hidden;
        }

        if (modified.State == HpLossResultModificationState.UnsupportedBecauseAggregateEnemyHpLossWithTungstenRod
            && modified.DirectHpLoss > 0)
        {
            return IncomingDamageRead.UnknownDirect(modified.DirectHpLoss);
        }

        return IncomingDamageRead.Unknown;
    }

    private static bool HasVerifiedHpLossResultModifier(Player player, Creature localCreature)
    {
        return HasActiveIntangiblePower(localCreature) || HasVerifiedHpLossRelic(player);
    }

    private static bool HasActiveIntangiblePower(Creature localCreature)
    {
        return localCreature.GetPower<IntangiblePower>()?.Amount > 0;
    }

    private static bool HasVerifiedHpLossRelic(Player player)
    {
        return player.Relics.Any(relic => !relic.IsMelted && (relic is TungstenRod || relic is BeatingRemnant));
    }

    private static IReadOnlyList<BlockableFutureDamageEvent> ReadEnemyAttackEvents(
        Player player,
        AttackIntent attackIntent,
        Creature localCreature,
        Creature enemy,
        EnemyInstanceIdentity enemyIdentity,
        int enemyAttackOrder,
        int totalDamage,
        out EnemyDamageModificationState modificationState)
    {
        var events = new List<BlockableFutureDamageEvent>();
        var orderBase = 1_000_000 + (enemyAttackOrder * 1_000);
        var repeats = attackIntent.Repeats;
        if (repeats > 0)
        {
            var singleDamage = attackIntent.GetSingleDamage(new[] { localCreature }, enemy);
            if (singleDamage >= 0 && singleDamage * repeats == totalDamage)
            {
                for (var i = 0; i < repeats; i++)
                {
                    var modified = VerifiedEnemyDamageModifier.ApplyDiamondDiadem(
                        player,
                        localCreature,
                        attackIntent,
                        enemy,
                        singleDamage,
                        true);
                    if (modified.State != EnemyDamageModificationState.Supported)
                    {
                        modificationState = modified.State;
                        return events;
                    }

                    events.Add(new BlockableFutureDamageEvent(
                        $"EnemyAttackIntent[{enemyIdentity.StableIdentity}:{enemyIdentity.SnapshotIndex}:{enemyAttackOrder}:{i}]",
                        orderBase + i,
                        modified.Amount,
                        true));
                }

                modificationState = EnemyDamageModificationState.Supported;
                return events;
            }
        }

        var aggregateModified = VerifiedEnemyDamageModifier.ApplyDiamondDiadem(
            player,
            localCreature,
            attackIntent,
            enemy,
            totalDamage,
            false);
        if (aggregateModified.State != EnemyDamageModificationState.Supported)
        {
            modificationState = aggregateModified.State;
            return events;
        }

        events.Add(new BlockableFutureDamageEvent(
            $"EnemyAttackIntent[{enemyIdentity.StableIdentity}:{enemyIdentity.SnapshotIndex}:{enemyAttackOrder}]",
            orderBase,
            aggregateModified.Amount,
            false));
        modificationState = EnemyDamageModificationState.Supported;
        return events;
    }

    private static bool TryReadOrderedHandTurnEndEvents(
        Player player,
        Creature localCreature,
        out List<HandTurnEndHpLossEvent> events,
        out int blockableRaw)
    {
        events = new List<HandTurnEndHpLossEvent>();
        blockableRaw = 0;
        var handPile = CardPile.Get(PileType.Hand, player);
        if (handPile is null)
        {
            return true;
        }

        try
        {
            var handCount = handPile.Cards.Count;
            for (var i = 0; i < handPile.Cards.Count; i++)
            {
                var card = handPile.Cards[i];
                if (!CardTurnEndDamageInspector.TryGetVerifiedSingleBlockableDamageVar(card, out var damageVar))
                {
                    return false;
                }

                if (damageVar is not null)
                {
                    var damage = GetModifiedIncomingCardDamage(player, localCreature, card, damageVar);
                    blockableRaw += damage;
                    events.Add(new HandTurnEndHpLossEvent(card.GetType().Name, i, HpLossDisplayLane.Blockable, damage, true));
                }
                else if (VerifiedFixedTurnEndHpLossReader.TryReadEvent(card, handCount, i, out var directHpLossEvent))
                {
                    events.Add(new HandTurnEndHpLossEvent(
                        directHpLossEvent.Source,
                        directHpLossEvent.NativeExecutionOrder,
                        directHpLossEvent.DisplayLane,
                        directHpLossEvent.VerifiedHpLoss,
                        directHpLossEvent.IsSingleVerifiedEvent));
                }
            }

            return true;
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("incoming.hand-events", exception);
            events.Clear();
            blockableRaw = 0;
            return false;
        }
    }

    private static bool TryReadHandTurnEndDamage(Player player, Creature localCreature, out int damage)
    {
        damage = 0;
        var handPile = CardPile.Get(PileType.Hand, player);
        if (handPile is null)
        {
            return true;
        }

        try
        {
            foreach (var card in handPile.Cards)
            {
                if (!CardTurnEndDamageInspector.TryGetVerifiedSingleBlockableDamageVar(card, out var damageVar))
                {
                    return false;
                }

                if (damageVar is not null)
                {
                    damage += GetModifiedIncomingCardDamage(player, localCreature, card, damageVar);
                }
            }

            return true;
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("incoming.hand-total", exception);
            damage = 0;
            return false;
        }
    }

    private static int GetModifiedIncomingCardDamage(Player player, Creature localCreature, CardModel card, DamageVar damageVar)
    {
        var modified = HookDamageCompat.ModifyDamage(
            player.RunState,
            localCreature.CombatState!,
            localCreature,
            localCreature,
            damageVar.BaseValue,
            damageVar.Props,
            card,
            ModifyDamageHookType.All,
            CardPreviewMode.None);

        return Math.Max(0, (int)modified);
    }

    private readonly record struct HandTurnEndHpLossEvent(
        string Source,
        int NativeExecutionOrder,
        HpLossDisplayLane DisplayLane,
        int Amount,
        bool IsSingleVerifiedEvent);

    private readonly record struct BlockableFutureDamageEvent(
        string Source,
        int NativeExecutionOrder,
        int Amount,
        bool IsSingleVerifiedEvent);

}
