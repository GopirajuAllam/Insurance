using InsurancePlatform.Api.Contracts;
using InsurancePlatform.Api.Domain;

namespace InsurancePlatform.Api.Services;

public sealed class PremiumCalculator
{
    private readonly IClock _clock;

    public PremiumCalculator(IClock clock)
    {
        _clock = clock;
    }

    public QuoteResponse Calculate(Customer customer, Coverage coverage, CreateQuoteRequest request)
    {
        if (request.TermInMonths <= 0)
        {
            throw new InvalidOperationException("Policy term must be greater than zero.");
        }

        var age = GetAge(customer.DateOfBirth, DateOnly.FromDateTime(_clock.UtcNow));
        var ageFactor = age switch
        {
            < 25 => 1.35m,
            < 40 => 1.10m,
            < 60 => 1.00m,
            _ => 1.25m
        };

        var riskFactor = request.RiskLevel switch
        {
            RiskLevel.Low => 0.90m,
            RiskLevel.Medium => 1.00m,
            RiskLevel.High => 1.35m,
            _ => throw new InvalidOperationException("Unsupported risk level.")
        };

        var termFactor = request.TermInMonths / 12m;
        var premium = decimal.Round(coverage.BasePremium * ageFactor * riskFactor * termFactor, 2, MidpointRounding.AwayFromZero);

        return new QuoteResponse(
            request.CustomerId,
            request.CoverageId,
            request.TermInMonths,
            request.RiskLevel,
            premium);
    }

    private static int GetAge(DateOnly birthday, DateOnly today)
    {
        var age = today.Year - birthday.Year;
        if (birthday > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
