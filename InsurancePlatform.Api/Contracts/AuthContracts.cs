namespace InsurancePlatform.Api.Contracts;

public sealed record RegisterRequest(string Username, string Password, string FullName);

public sealed record LoginRequest(string Username, string Password);

public sealed record AuthResponse(string Username, string Token, DateTime ExpiresUtc);
