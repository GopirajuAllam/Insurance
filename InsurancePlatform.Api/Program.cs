using InsurancePlatform.Api.Data;
using InsurancePlatform.Api.Security;
using InsurancePlatform.Api.Services;
using Microsoft.EntityFrameworkCore;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<InsuranceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("InsuranceDatabase") ?? "Data Source=insurance-platform.db"));
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PremiumCalculator>();
builder.Services.AddScoped<PaymentProcessor>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InsuranceDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseMiddleware<AuthTokenMiddleware>();
app.MapControllers();
app.Run();

public partial class Program
{
}
