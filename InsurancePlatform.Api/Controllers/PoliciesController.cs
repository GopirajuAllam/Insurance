using InsurancePlatform.Api.Contracts;
using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Domain;
using InsurancePlatform.Api.Security;
using InsurancePlatform.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Api.Controllers;

[ApiController]
[Route("api/policies")]
public sealed class PoliciesController : ControllerBase
{
    private readonly InsuranceDbContext _dbContext;
    private readonly PremiumCalculator _premiumCalculator;

    public PoliciesController(InsuranceDbContext dbContext, PremiumCalculator premiumCalculator)
    {
        _dbContext = dbContext;
        _premiumCalculator = premiumCalculator;
    }

    [HttpPost("quote")]
    public async Task<IActionResult> Quote(CreateQuoteRequest request, CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUser();
        var customer = await _dbContext.Customers
            .SingleOrDefaultAsync(candidate => candidate.Id == request.CustomerId && candidate.CreatedByUserId == user.Id, cancellationToken);
        var coverage = await _dbContext.Coverages
            .SingleOrDefaultAsync(candidate => candidate.Id == request.CoverageId && candidate.CreatedByUserId == user.Id, cancellationToken);

        if (customer is null || coverage is null)
        {
            return NotFound(new { error = "Customer or coverage was not found." });
        }

        try
        {
            var quote = _premiumCalculator.Calculate(customer, coverage, request);
            return Ok(quote);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePolicyRequest request, CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUser();
        var customer = await _dbContext.Customers
            .SingleOrDefaultAsync(candidate => candidate.Id == request.CustomerId && candidate.CreatedByUserId == user.Id, cancellationToken);
        var coverage = await _dbContext.Coverages
            .SingleOrDefaultAsync(candidate => candidate.Id == request.CoverageId && candidate.CreatedByUserId == user.Id, cancellationToken);

        if (customer is null || coverage is null)
        {
            return NotFound(new { error = "Customer or coverage was not found." });
        }

        try
        {
            var quote = _premiumCalculator.Calculate(
                customer,
                coverage,
                new CreateQuoteRequest(request.CustomerId, request.CoverageId, request.TermInMonths, request.RiskLevel));

            var policy = new Policy
            {
                CustomerId = customer.Id,
                CoverageId = coverage.Id,
                CreatedByUserId = user.Id,
                TermInMonths = request.TermInMonths,
                RiskLevel = request.RiskLevel,
                PremiumAmount = quote.PremiumAmount,
                Status = PolicyStatus.PendingPayment,
                CreatedUtc = DateTime.UtcNow
            };

            _dbContext.Policies.Add(policy);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = policy.Id }, policy);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUser();
        var policy = await _dbContext.Policies
            .Include(candidate => candidate.Customer)
            .Include(candidate => candidate.Coverage)
            .Include(candidate => candidate.Payments)
            .SingleOrDefaultAsync(candidate => candidate.Id == id && candidate.CreatedByUserId == user.Id, cancellationToken);

        return policy is null ? NotFound() : Ok(policy);
    }
}
