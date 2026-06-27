using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace STS2PartyWatch.Combat;

internal static class VerifiedFixedTurnEndHpLossReader
{
    public static bool TryRead(Player player, out int hpLoss)
    {
        hpLoss = 0;

        try
        {
            var handPile = CardPile.Get(PileType.Hand, player);
            if (handPile is null)
            {
                return true;
            }

            var handCount = handPile.Cards.Count;
            foreach (var card in handPile.Cards)
            {
                if (TryGetBeckonHpLoss(card, out var beckonLoss))
                {
                    hpLoss += beckonLoss;
                    continue;
                }

                if (TryGetBadLuckHpLoss(card, out var badLuckLoss))
                {
                    hpLoss += badLuckLoss;
                    continue;
                }

                if (TryGetRegretHpLoss(card, handCount, out var regretLoss))
                {
                    hpLoss += regretLoss;
                }
            }

            return true;
        }
        catch
        {
            hpLoss = 0;
            return false;
        }
    }

    private static bool TryGetBeckonHpLoss(CardModel card, out int hpLoss)
    {
        return TryGetVerifiedFixedHpLoss<Beckon>(card, 6, out hpLoss);
    }

    private static bool TryGetBadLuckHpLoss(CardModel card, out int hpLoss)
    {
        return TryGetVerifiedFixedHpLoss<BadLuck>(card, 13, out hpLoss);
    }

    private static bool TryGetRegretHpLoss(CardModel card, int handCount, out int hpLoss)
    {
        hpLoss = 0;
        if (card is not Regret || !card.HasTurnEndInHandEffect)
        {
            return false;
        }

        hpLoss = Math.Max(0, handCount);
        return true;
    }

    private static bool TryGetVerifiedFixedHpLoss<TCard>(CardModel card, int expectedHpLoss, out int hpLoss)
        where TCard : CardModel
    {
        hpLoss = 0;
        if (card is not TCard || !card.HasTurnEndInHandEffect)
        {
            return false;
        }

        var hpLossVar = card.DynamicVars.Values.OfType<HpLossVar>().SingleOrDefault();
        if (hpLossVar is null || hpLossVar.BaseValue != expectedHpLoss)
        {
            throw new InvalidOperationException($"{typeof(TCard).Name} HpLossVar did not match the verified value.");
        }

        hpLoss = expectedHpLoss;
        return true;
    }
}
