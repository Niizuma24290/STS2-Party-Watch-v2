namespace STS2PartyWatch.Combat;

internal static class VerifiedTurnEndDamagePolicy
{
    public static bool IsVerifiedSingleBlockableDamageShape(
        Type runtimeCardType,
        Type verifiedCardType,
        int damageVarCount,
        bool singleDamageVarIsUnblockable)
    {
        return runtimeCardType == verifiedCardType
            && damageVarCount == 1
            && !singleDamageVarIsUnblockable;
    }
}
