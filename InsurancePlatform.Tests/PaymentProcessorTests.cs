using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Domain;
using InsurancePlatform.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Tests;

public sealed class PaymentProcessorTests
{
    [Fact]
    public async Task ProcessAsync_ActivatesPolicyWhenPaymentCoversPremium()
    {
        await using var scope = await TestDbScope.CreateAsync();
        var policy = await SeedPolicyAsync(scope.DbContext, 500m);
        var processor = new PaymentProcessor(scope.DbContext, new FixedClock(new DateTime(2026, 2, 28, 15, 0, 0, DateTimeKind.Utc)));

        var payment = await processor.ProcessAsync(policy.Id, 500m, "Card", CancellationToken.None);
        var storedPolicy = await scope.DbContext.Policies.SingleAsync(candidate => candidate.Id == policy.Id);

        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.Equal(PolicyStatus.Active, storedPolicy.Status);
    }

    [Fact]
    public async Task ProcessAsync_RejectsInsufficientPaymentAndKeepsPolicyPending()
    {
        await using var scope = await TestDbScope.CreateAsync();
        var policy = await SeedPolicyAsync(scope.DbContext, 500m);
        var processor = new PaymentProcessor(scope.DbContext, new FixedClock(new DateTime(2026, 2, 28, 15, 0, 0, DateTimeKind.Utc)));

        var payment = await processor.ProcessAsync(policy.Id, 300m, "ACH", CancellationToken.None);
        var storedPolicy = await scope.DbContext.Policies.SingleAsync(candidate => candidate.Id == policy.Id);

        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal(PolicyStatus.PendingPayment, storedPolicy.Status);
        Assert.NotNull(payment.FailureReason);
    }

    private static async Task<Policy> SeedPolicyAsync(InsuranceDbContext dbContext, decimal premium)
    {
        var user = new UserAccount
        {
            Username = "agent01",
            FullName = "Agent One",
            PasswordHash = "hash",
            CreatedUtc = DateTime.UtcNow
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var customer = new Customer
        {
            CreatedByUserId = user.Id,
            FullName = "Test Customer",
            Email = "customer@example.com",
            PhoneNumber = "555-0100",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Address = "100 Main St",
            CreatedUtc = DateTime.UtcNow
        };
        dbContext.Customers.Add(customer);

        var coverage = new Coverage
        {
            CreatedByUserId = user.Id,
            Name = "Auto",
            Description = "Auto coverage",
            CoverageLimit = 100000m,
            BasePremium = premium,
            CreatedUtc = DateTime.UtcNow
        };
        dbContext.Coverages.Add(coverage);

        await dbContext.SaveChangesAsync();

        var policy = new Policy
        {
            CustomerId = customer.Id,
            CoverageId = coverage.Id,
            CreatedByUserId = user.Id,
            TermInMonths = 12,
            RiskLevel = RiskLevel.Medium,
            PremiumAmount = premium,
            Status = PolicyStatus.PendingPayment,
            CreatedUtc = DateTime.UtcNow
        };

        dbContext.Policies.Add(policy);
        await dbContext.SaveChangesAsync();
        return policy;
    }
}
