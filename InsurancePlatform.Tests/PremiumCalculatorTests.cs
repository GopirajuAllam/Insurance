using InsurancePlatform.Api.Contracts;
using InsurancePlatform.Api.Domain;
using InsurancePlatform.Api.Services;

namespace InsurancePlatform.Tests;

public sealed class PremiumCalculatorTests
{
    [Fact]
    public void Calculate_AdjustsPremiumForAgeRiskAndTerm()
    {
        var calculator = new PremiumCalculator(new FixedClock(new DateTime(2026, 2, 28, 12, 0, 0, DateTimeKind.Utc)));
        var customer = new Customer
        {
            DateOfBirth = new DateOnly(1995, 6, 15)
        };
        var coverage = new Coverage
        {
            BasePremium = 1200m
        };

        var quote = calculator.Calculate(customer, coverage, new CreateQuoteRequest(1, 2, 6, RiskLevel.High));

        Assert.Equal(891.00m, quote.PremiumAmount);
    }
}
