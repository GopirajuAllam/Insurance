# Insurance Platform Backend

This repository contains a backend-only insurance management platform built with ASP.NET Core and C#.

The application is designed for insurance operations teams that need a lightweight API to manage:

- user registration and login authentication
- customer records
- insurance coverage definitions
- premium quotation and policy creation
- payment processing for issued policies

## Tech Stack

- ASP.NET Core Web API (`net7.0`)
- C#
- Entity Framework Core
- SQLite
- xUnit for automated tests

## Core Features

### Authentication

The API provides user registration and login endpoints. After login, a bearer token is generated and stored as a session token. All business endpoints require that token in the `Authorization` header.

### Customer Management

Authenticated users can create and retrieve customer records, including:

- full name
- email
- phone number
- date of birth
- address

### Coverage Management

Users can define insurance coverage products with:

- coverage name
- description
- coverage limit
- base premium

### Premium Quoting and Policy Creation

The system calculates policy premiums based on:

- customer age
- selected coverage
- risk level
- policy term

Users can request a quote first, then create a policy using the same pricing logic.

### Payment Processing

Policies start in a pending payment state. The payment module:

- records payment transactions
- validates the payment amount against the policy premium
- activates the policy when payment is sufficient
- stores failure details when payment is insufficient

## Project Structure

- `InsurancePlatform.Api`: main API project
- `InsurancePlatform.Tests`: automated test project
- `InsurancePlatform.sln`: solution file

## Main API Endpoints

### Authentication

- `POST /api/auth/register`
- `POST /api/auth/login`

### Customers

- `POST /api/customers`
- `GET /api/customers`
- `GET /api/customers/{id}`

### Coverages

- `POST /api/coverages`
- `GET /api/coverages`

### Policies

- `POST /api/policies/quote`
- `POST /api/policies`
- `GET /api/policies/{id}`

### Payments

- `POST /api/payments`

## Running the Project

### Prerequisites

- .NET 7 SDK installed

### Run the API

```bash
dotnet run --project InsurancePlatform.Api/InsurancePlatform.Api.csproj
```

### Run Tests

```bash
dotnet test InsurancePlatform.Tests/InsurancePlatform.Tests.csproj
```

## Database

The application uses SQLite and creates the database automatically at startup with EF Core `EnsureCreated()`. The default local database file is created inside the API project directory.

## Automated Test Coverage

The test suite currently validates:

- user registration and login session creation
- premium calculation behavior
- successful payment activation flow
- insufficient payment failure flow

## Notes

This project is backend-only and does not include a frontend or UI layer.
