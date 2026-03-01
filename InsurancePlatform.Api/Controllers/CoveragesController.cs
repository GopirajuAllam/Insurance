using InsurancePlatform.Api.Contracts;
using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Domain;
using InsurancePlatform.Api.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Api.Controllers;

[ApiController]
[Route("api/coverages")]
public sealed class CoveragesController : ControllerBase
{
    private readonly InsuranceDbContext _dbContext;

    public CoveragesController(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCoverageRequest request, CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUser();
        var coverage = new Coverage
        {
            CreatedByUserId = user.Id,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            CoverageLimit = request.CoverageLimit,
            BasePremium = request.BasePremium,
            CreatedUtc = DateTime.UtcNow
        };

        _dbContext.Coverages.Add(coverage);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Created($"/api/coverages/{coverage.Id}", coverage);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUser();
        var coverages = await _dbContext.Coverages
            .Where(coverage => coverage.CreatedByUserId == user.Id)
            .OrderBy(coverage => coverage.Name)
            .ToListAsync(cancellationToken);

        return Ok(coverages);
    }
}
