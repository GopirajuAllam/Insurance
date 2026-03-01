using InsurancePlatform.Api.Domain;

namespace InsurancePlatform.Api.Contracts;

public sealed record CreateQuoteRequest(int CustomerId, int CoverageId, int TermInMonths, RiskLevel RiskLevel);

public sealed record QuoteResponse(
    int CustomerId,
    int CoverageId,
    int TermInMonths,
    RiskLevel RiskLevel,
    decimal PremiumAmount);

public sealed record CreatePolicyRequest(int CustomerId, int CoverageId, int TermInMonths, RiskLevel RiskLevel);
