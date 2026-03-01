using InsurancePlatform.Api.Contracts;
using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task RegisterAndLogin_ReturnsTokenAndStoresSession()
    {
        await using var scope = await TestDbScope.CreateAsync();
        var service = new AuthService(scope.DbContext, new FixedClock(new DateTime(2026, 2, 28, 12, 0, 0, DateTimeKind.Utc)));

        await service.RegisterAsync(new RegisterRequest("agent01", "P@ssword123", "Agent One"), CancellationToken.None);
        var authResult = await service.LoginAsync(new LoginRequest("agent01", "P@ssword123"), CancellationToken.None);

        Assert.Equal("agent01", authResult.Username);
        Assert.False(string.IsNullOrWhiteSpace(authResult.Token));
        Assert.Equal(1, await scope.DbContext.SessionTokens.CountAsync());
    }
}
