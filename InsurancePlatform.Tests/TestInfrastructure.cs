using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InsurancePlatform.Tests;

internal sealed class TestDbScope : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    private TestDbScope(SqliteConnection connection, InsuranceDbContext dbContext)
    {
        _connection = connection;
        DbContext = dbContext;
    }

    public InsuranceDbContext DbContext { get; }

    public static async Task<TestDbScope> CreateAsync()
    {
        SQLitePCL.Batteries_V2.Init();

        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InsuranceDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new InsuranceDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        return new TestDbScope(connection, dbContext);
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }
}

internal sealed class FixedClock : IClock
{
    private readonly DateTime _utcNow;

    public FixedClock(DateTime utcNow)
    {
        _utcNow = utcNow;
    }

    public DateTime UtcNow => _utcNow;
}
