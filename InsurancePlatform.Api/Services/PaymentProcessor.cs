using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Api.Services;

public sealed class PaymentProcessor
{
    private readonly InsuranceDbContext _dbContext;
    private readonly IClock _clock;

    public PaymentProcessor(InsuranceDbContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<PaymentTransaction> ProcessAsync(int policyId, decimal amount, string paymentMethod, CancellationToken cancellationToken)
    {
        var policy = await _dbContext.Policies.SingleOrDefaultAsync(candidate => candidate.Id == policyId, cancellationToken)
            ?? throw new InvalidOperationException("Policy was not found.");

        var payment = new PaymentTransaction
        {
            PolicyId = policyId,
            Amount = amount,
            PaymentMethod = paymentMethod.Trim(),
            ProcessedUtc = _clock.UtcNow
        };

        if (amount < policy.PremiumAmount)
        {
            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = "Submitted amount is lower than the required premium.";
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return payment;
        }

        payment.Status = PaymentStatus.Completed;
        policy.Status = PolicyStatus.Active;
        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return payment;
    }
}
