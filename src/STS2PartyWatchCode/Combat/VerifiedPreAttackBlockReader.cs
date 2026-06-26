using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace STS2PartyWatch.Combat;

internal static class VerifiedPreAttackBlockReader
{
    public static PreAttackBlockRead Read(Player player, Creature localCreature)
    {
        try
        {
            var block = 0;
            block += ReadFrostBlock(player);
            block += ReadPlatingBlock(localCreature);
            block += ReadOrichalcumBlock(player, localCreature.Block);
            block += ReadFakeOrichalcumBlock(player, localCreature.Block);
            block += ReadRippleBasinBlock(player, localCreature.CombatState!);
            block += ReadCloakClaspBlock(player);
            return PreAttackBlockRead.Known(block);
        }
        catch
        {
            return PreAttackBlockRead.Unknown;
        }
    }

    private static int ReadFrostBlock(Player player)
    {
        var orbs = player.PlayerCombatState?.OrbQueue?.Orbs;
        if (orbs is null)
        {
            return 0;
        }

        var block = 0;
        foreach (var orb in orbs.OfType<FrostOrb>())
        {
            block += Math.Max(0, (int)orb.PassiveVal);
        }

        return block;
    }

    private static int ReadPlatingBlock(Creature localCreature)
    {
        var plating = localCreature.GetPower<PlatingPower>();
        return plating is null ? 0 : Math.Max(0, plating.Amount);
    }

    private static int ReadOrichalcumBlock(Player player, int currentBlock)
    {
        if (currentBlock > 0)
        {
            return 0;
        }

        var relic = FindRelic<Orichalcum>(player);
        return relic is null ? 0 : ReadBlockVar(relic);
    }

    private static int ReadFakeOrichalcumBlock(Player player, int currentBlock)
    {
        if (currentBlock > 0)
        {
            return 0;
        }

        var relic = FindRelic<FakeOrichalcum>(player);
        return relic is null ? 0 : ReadBlockVar(relic);
    }

    private static int ReadRippleBasinBlock(Player player, ICombatState combatState)
    {
        var relic = FindRelic<RippleBasin>(player);
        if (relic is null || HasPlayedAttackThisTurn(player, combatState))
        {
            return 0;
        }

        return ReadBlockVar(relic);
    }

    private static int ReadCloakClaspBlock(Player player)
    {
        var relic = FindRelic<CloakClasp>(player);
        if (relic is null)
        {
            return 0;
        }

        var handPile = CardPile.Get(PileType.Hand, player);
        return handPile is null ? 0 : handPile.Cards.Count * ReadBlockVar(relic);
    }

    private static bool HasPlayedAttackThisTurn(Player player, ICombatState combatState)
    {
        return CombatManager.Instance.History.CardPlaysFinished.Any(entry =>
            entry.HappenedThisTurn(combatState)
            && entry.CardPlay.Card.Type == CardType.Attack
            && entry.CardPlay.Card.Owner == player);
    }

    private static TRelic? FindRelic<TRelic>(Player player)
        where TRelic : RelicModel
    {
        return player.Relics.OfType<TRelic>().FirstOrDefault();
    }

    private static int ReadBlockVar(RelicModel relic)
    {
        return relic.DynamicVars.Values.OfType<BlockVar>().Select(blockVar => Math.Max(0, (int)blockVar.BaseValue)).FirstOrDefault();
    }
}

internal readonly record struct PreAttackBlockRead(PreAttackBlockReadState State, int Block)
{
    public static PreAttackBlockRead Unknown => new(PreAttackBlockReadState.Unknown, 0);

    public static PreAttackBlockRead Known(int block) => new(PreAttackBlockReadState.Known, block);
}

internal enum PreAttackBlockReadState
{
    Known,
    Unknown
}
