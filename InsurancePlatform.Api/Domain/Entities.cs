using System.ComponentModel.DataAnnotations;

namespace InsurancePlatform.Api.Domain;

public sealed class UserAccount
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; }

    public List<UserSessionToken> SessionTokens { get; set; } = new();
}

public sealed class UserSessionToken
{
    public int Id { get; set; }
    public int UserAccountId { get; set; }

    [MaxLength(128)]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresUtc { get; set; }
    public DateTime CreatedUtc { get; set; }
    public UserAccount? UserAccount { get; set; }
}

public sealed class Customer
{
    public int Id { get; set; }
    public int CreatedByUserId { get; set; }

    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; }

    public List<Policy> Policies { get; set; } = new();
}

public sealed class Coverage
{
    public int Id { get; set; }
    public int CreatedByUserId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(400)]
    public string Description { get; set; } = string.Empty;

    public decimal CoverageLimit { get; set; }
    public decimal BasePremium { get; set; }
    public DateTime CreatedUtc { get; set; }

    public List<Policy> Policies { get; set; } = new();
}

public sealed class Policy
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int CoverageId { get; set; }
    public int CreatedByUserId { get; set; }
    public int TermInMonths { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public decimal PremiumAmount { get; set; }
    public PolicyStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }

    public Customer? Customer { get; set; }
    public Coverage? Coverage { get; set; }
    public List<PaymentTransaction> Payments { get; set; } = new();
}

public sealed class PaymentTransaction
{
    public int Id { get; set; }
    public int PolicyId { get; set; }
    public decimal Amount { get; set; }

    [MaxLength(40)]
    public string PaymentMethod { get; set; } = string.Empty;

    public PaymentStatus Status { get; set; }

    [MaxLength(250)]
    public string? FailureReason { get; set; }

    public DateTime ProcessedUtc { get; set; }
    public Policy? Policy { get; set; }
}

public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3
}

public enum PolicyStatus
{
    Draft = 1,
    PendingPayment = 2,
    Active = 3
}

public enum PaymentStatus
{
    Completed = 1,
    Failed = 2
}
