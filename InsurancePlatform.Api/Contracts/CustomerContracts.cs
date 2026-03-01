namespace InsurancePlatform.Api.Contracts;

public sealed record CreateCustomerRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth,
    string Address);
