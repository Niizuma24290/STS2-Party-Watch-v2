using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace STS2PartyWatch.Combat;

public sealed class LocalIncomingDamageReader
{
    public IncomingDamageRead Read(ICombatState? combatState)
    {
        if (combatState is null || !combatState.IsLiveCombat())
        {
            return IncomingDamageRead.Hidden;
        }

        if (combatState.Players.Count != 1)
        {
            return IncomingDamageRead.Unknown;
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

        if (combatState.Players.Count != 1)
        {
            return IncomingDamageRead.Unknown;
        }

        if (localCreature is null || !localCreature.IsAlive)
        {
            return IncomingDamageRead.Hidden;
        }

        return ReadKnown(combatState, localCreature);
    }

    private static IncomingDamageRead ReadKnown(ICombatState combatState, Creature localCreature)
    {
        var raw = 0;
        var foundDamage = false;

        foreach (var enemy in combatState.Enemies)
        {
            if (enemy is null || !enemy.IsAlive || enemy.Monster?.NextMove?.Intents is null)
            {
                continue;
            }

            foreach (var intent in enemy.Monster.NextMove.Intents)
            {
                if (intent is not AttackIntent attackIntent)
                {
                    continue;
                }

                foundDamage = true;
                raw += attackIntent.GetTotalDamage(new[] { localCreature }, enemy);
            }
        }

        if (localCreature.Player is not null && TryReadHandTurnEndDamage(localCreature.Player, localCreature, out var handTurnEndDamage))
        {
            raw += handTurnEndDamage;
            foundDamage = foundDamage || handTurnEndDamage > 0;
        }

        if (!foundDamage)
        {
            return IncomingDamageRead.Hidden;
        }

        if (localCreature.Player is null)
        {
            return IncomingDamageRead.Unknown;
        }

        var preAttackBlock = VerifiedPreAttackBlockReader.Read(localCreature.Player, localCreature);
        if (preAttackBlock.State != PreAttackBlockReadState.Known)
        {
            return IncomingDamageRead.Unknown;
        }

        return IncomingDamageRead.Known(raw, localCreature.Block + preAttackBlock.Block);
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
                if (!CardTurnEndDamageInspector.DoesTurnEndInHandCallDamage(card))
                {
                    continue;
                }

                foreach (var damageVar in card.DynamicVars.Values.OfType<DamageVar>())
                {
                    if (damageVar.Props.HasFlag(ValueProp.Unblockable))
                    {
                        continue;
                    }

                    damage += GetModifiedIncomingCardDamage(player, localCreature, card, damageVar);
                }
            }

            return true;
        }
        catch
        {
            damage = 0;
            return false;
        }
    }

    private static int GetModifiedIncomingCardDamage(Player player, Creature localCreature, CardModel card, DamageVar damageVar)
    {
        var modified = Hook.ModifyDamage(
            player.RunState,
            localCreature.CombatState,
            localCreature,
            localCreature,
            damageVar.BaseValue,
            damageVar.Props,
            card,
            ModifyDamageHookType.All,
            CardPreviewMode.None,
            out _);

        return Math.Max(0, (int)modified);
    }
}
