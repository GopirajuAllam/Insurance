# Pharmacy Benefit Manager Dashboard

This standalone project packages the PBM dashboard work into its own repository-ready folder.

## Contents

- `InsurancePlatform.Api`: ASP.NET Core API with the `GET /api/dashboard/pbm` endpoint and demo seed data
- `pbm-dashboard`: Angular 14 frontend with Bootstrap-based interactive dashboard views
- `database`: SQL Server schema and seed script for PBM reporting

## Run The API

```bash
dotnet run --project InsurancePlatform.Api/InsurancePlatform.Api.csproj --launch-profile http
```

The API runs on `http://localhost:5058` for the Angular proxy configuration.

## Run The Angular App

```bash
cd pbm-dashboard
npm install
npm start
```

## SQL Server

Use `database/pbm-dashboard-sqlserver.sql` to create a SQL Server schema and starter dataset.
