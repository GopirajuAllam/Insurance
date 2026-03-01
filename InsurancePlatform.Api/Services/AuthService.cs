using System.Security.Cryptography;
using InsurancePlatform.Api.Contracts;
using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Api.Services;

public sealed class AuthService
{
    private readonly InsuranceDbContext _dbContext;
    private readonly PasswordHasher<UserAccount> _passwordHasher = new();
    private readonly IClock _clock;

    public AuthService(InsuranceDbContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        if (await _dbContext.Users.AnyAsync(user => user.Username == username, cancellationToken))
        {
            throw new InvalidOperationException("Username is already in use.");
        }

        var user = new UserAccount
        {
            Username = username,
            FullName = request.FullName.Trim(),
            CreatedUtc = _clock.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(candidate => candidate.Username == username, cancellationToken)
            ?? throw new InvalidOperationException("Invalid username or password.");

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        var now = _clock.UtcNow;
        var session = new UserSessionToken
        {
            UserAccountId = user.Id,
            Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
            CreatedUtc = now,
            ExpiresUtc = now.AddHours(8)
        };

        _dbContext.SessionTokens.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Username, session.Token, session.ExpiresUtc);
    }

    public async Task<UserAccount?> GetUserByTokenAsync(string token, CancellationToken cancellationToken)
    {
        var now = _clock.UtcNow;
        var session = await _dbContext.SessionTokens
            .Include(candidate => candidate.UserAccount)
            .SingleOrDefaultAsync(candidate => candidate.Token == token && candidate.ExpiresUtc > now, cancellationToken);

        return session?.UserAccount;
    }
}
