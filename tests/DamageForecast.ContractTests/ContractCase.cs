internal sealed record ContractCase(
    string Id,
    string Category,
    string Name,
    Action<ContractAssert> Execute,
    string? SkipReason = null);
