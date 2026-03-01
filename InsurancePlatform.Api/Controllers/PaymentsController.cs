using InsurancePlatform.Api.Contracts;
using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Security;
using InsurancePlatform.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Api.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
    private readonly InsuranceDbContext _dbContext;
    private readonly PaymentProcessor _paymentProcessor;

    public PaymentsController(InsuranceDbContext dbContext, PaymentProcessor paymentProcessor)
    {
        _dbContext = dbContext;
        _paymentProcessor = paymentProcessor;
    }

    [HttpPost]
    public async Task<IActionResult> Process(ProcessPaymentRequest request, CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUser();
        var policyExists = await _dbContext.Policies
            .AnyAsync(candidate => candidate.Id == request.PolicyId && candidate.CreatedByUserId == user.Id, cancellationToken);

        if (!policyExists)
        {
            return NotFound(new { error = "Policy was not found." });
        }

        try
        {
            var payment = await _paymentProcessor.ProcessAsync(
                request.PolicyId,
                request.Amount,
                request.PaymentMethod,
                cancellationToken);

            return payment.Status == Domain.PaymentStatus.Completed
                ? Ok(payment)
                : BadRequest(payment);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}
