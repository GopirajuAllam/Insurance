namespace InsurancePlatform.Api.Contracts;

public sealed record ProcessPaymentRequest(int PolicyId, decimal Amount, string PaymentMethod);
