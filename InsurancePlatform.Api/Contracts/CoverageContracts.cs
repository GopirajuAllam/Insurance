namespace InsurancePlatform.Api.Contracts;

public sealed record CreateCoverageRequest(
    string Name,
    string Description,
    decimal CoverageLimit,
    decimal BasePremium);
