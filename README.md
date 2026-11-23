# Backend

.NET 8 API for the investment card demo.

```bash
dotnet run --project src/InvestmentCard.Api
```

- Swagger: http://localhost:5080/swagger
- Hangfire: http://localhost:5080/hangfire

In-memory EF + Hangfire. Three customers get seeded on startup; everything resets when you restart.

## Layout

- `Domain` – entities / rules (`Portfolio`, `CreditCard`, etc.)
- `Application` – MediatR handlers
- `Infrastructure` – EF, seed, Hangfire job
- `Api` – controllers

Credit limit is lien × advance rate (Basic 50% / Plus 70% / Premium 90%).
Domain exceptions come back as 422 with a `code` field.

Delinquency job runs on the cron in config (`Delinquency:ThresholdMinutes`, default 2 min for the demo).
