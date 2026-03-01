using InsurancePlatform.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Api.Data;

public sealed class InsuranceDbContext : DbContext
{
    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<UserSessionToken> SessionTokens => Set<UserSessionToken>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Coverage> Coverages => Set<Coverage>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<PaymentTransaction> Payments => Set<PaymentTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>()
            .HasIndex(user => user.Username)
            .IsUnique();

        modelBuilder.Entity<UserSessionToken>()
            .HasIndex(token => token.Token)
            .IsUnique();

        modelBuilder.Entity<Policy>()
            .Property(policy => policy.PremiumAmount)
            .HasColumnType("TEXT");

        modelBuilder.Entity<Coverage>()
            .Property(coverage => coverage.BasePremium)
            .HasColumnType("TEXT");

        modelBuilder.Entity<Coverage>()
            .Property(coverage => coverage.CoverageLimit)
            .HasColumnType("TEXT");

        modelBuilder.Entity<PaymentTransaction>()
            .Property(payment => payment.Amount)
            .HasColumnType("TEXT");
    }
}
