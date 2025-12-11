# Miro-Services

Professional microservices reference implementation built with .NET 8 and C# 12. This repository demonstrates practical patterns for service decomposition, synchronous (gRPC) and asynchronous (RabbitMQ) communication, caching with Redis, and persistence with MongoDB.

## Overview

Miro-Services is intended as a learning and reference implementation. It contains multiple independently deployable services that illustrate common integration concerns such as service contracts, event-driven messaging, distributed caching, and environment configuration.

## Services

- `Catalog.API` — Product catalog (MongoDB). Exposes REST endpoints for product queries and management.
- `Basket.API` — Shopping basket (Redis). Uses a gRPC client to consult the Discount service during checkout.
- `Discount.Grpc` — gRPC discount service. Provides discount lookup RPCs used by the basket flow.
- `Ordering.API` — Order processing service. Consumes checkout events published to the event bus (RabbitMQ) and persists orders.
- `Shopping.Aggregator`, `OcelotApiGW` — Aggregator and API gateway projects (if present) to compose and route requests.

Each service is located under `Services/<ServiceName>` and targets .NET 8.

## Architecture highlights

- Synchronous RPC: gRPC for typed, low-latency calls between services (e.g., discount lookups).
- Asynchronous messaging: RabbitMQ (event bus) for decoupled, reliable event-driven flows (checkout → ordering).
- Persistence: MongoDB for catalog data; Redis for basket caching. Orders use a configurable storage implementation under `Ordering` projects.
- Deployability: Services are independently deployable and can be containerized for production or development use.

## Prerequisites

- .NET 8 SDK
- Docker & Docker Compose (recommended)
- MongoDB
- Redis
- RabbitMQ

These can be run locally or using containers. Docker Compose is the fastest way to bootstrap dependencies.

## Configuration

Services read configuration from `appsettings.json` and environment variables. Example `Catalog.API` configuration:

```json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "CatalogDb",
    "CollectionName": "Products"
  }
}
```

Other services include similar sections for Redis and RabbitMQ. Prefer environment variables or a secrets manager for production credentials.

Notable locations:
- `Services/Catalog/Catalog.API/Data/CatalogContext.cs` — MongoDB client, database/collection initialization and seed invocation.
- `Services/Catalog/Catalog.API/Data/CatalogContextSeed.cs` — seed data logic (if present).
- `Services/Basket/Basket.API/GrpcService/DiscountGrpcService.cs` — gRPC client usage for discount lookups.
- `src/BuildingBlocks/EventBus.Messages` — shared event message types used between services.

## Running the solution

Docker Compose (recommended):

1. From repository root: `docker compose up --build` (requires `docker-compose.yml`).
2. Wait for services and infrastructure to become healthy.
3. Tail logs: `docker compose logs -f` or `docker compose logs -f <service>`.

dotnet CLI (service-by-service):

1. Start infrastructure (MongoDB, Redis, RabbitMQ) via Docker or locally.
2. Restore and build the solution:

   ```bash
   dotnet restore
   dotnet build
   ```

3. Run a service, for example the Catalog API:

   ```bash
   dotnet run --project Services/Catalog/Catalog.API
   ```

Repeat for other services.

## Health, contracts and endpoints

- Check each service's `appsettings.json` or `launchSettings.json` for configured ports and health endpoints.
- gRPC `.proto` files and shared DTOs are located in the Discount or Contracts projects; use them to generate clients or validate contracts.
- Event schemas are defined in the `EventBus.Messages` project; consult these types for publisher/consumer payload shapes.

## Testing

Run all unit and integration tests with:

```bash
dotnet test
```

Integration tests that require infrastructure should be run while Docker Compose is active and pointed at test instances.

## Contributing

- Follow `.editorconfig` for code formatting and style.
- See `CONTRIBUTING.md` (create if missing) for PR and branch guidelines.
- Include tests and documentation for new features or bug fixes.

## Troubleshooting

- If the catalog has no data, inspect `CatalogContext` seed logic at `Services/Catalog/Catalog.API/Data/CatalogContext.cs` and verify MongoDB connection string.
- Verify Redis and RabbitMQ settings if basket or ordering flows fail.
- Use service logs (console/Docker logs) for diagnostics.

## License

Add a `LICENSE` file (for example MIT) to this repository to document permitted usage.

## Contact

Open an issue or submit a pull request on GitHub for questions, bugs, or improvements.

---

Next steps I can help with:
- Add `CONTRIBUTING.md`, `EVENTS.md`, or `docker-compose.yml` to the repo.
- Generate per-service `appsettings.template.json` files.
