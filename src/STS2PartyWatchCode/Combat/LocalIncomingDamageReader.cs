using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

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

        var raw = 0;
        var foundAttack = false;

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

                foundAttack = true;
                raw += attackIntent.GetTotalDamage(new[] { localCreature }, enemy);
            }
        }

        return foundAttack
            ? IncomingDamageRead.Known(raw, localCreature.Block)
            : IncomingDamageRead.Hidden;
    }
}
