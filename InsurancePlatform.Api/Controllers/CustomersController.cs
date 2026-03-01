using InsurancePlatform.Api.Contracts;
using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Domain;
using InsurancePlatform.Api.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly InsuranceDbContext _dbContext;

    public CustomersController(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUser();
        var customer = new Customer
        {
            CreatedByUserId = user.Id,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            DateOfBirth = request.DateOfBirth,
            Address = request.Address.Trim(),
            CreatedUtc = DateTime.UtcNow
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUser();
        var customers = await _dbContext.Customers
            .Where(customer => customer.CreatedByUserId == user.Id)
            .OrderBy(customer => customer.FullName)
            .ToListAsync(cancellationToken);

        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUser();
        var customer = await _dbContext.Customers
            .SingleOrDefaultAsync(candidate => candidate.Id == id && candidate.CreatedByUserId == user.Id, cancellationToken);

        return customer is null ? NotFound() : Ok(customer);
    }
}
