# Portfolio Tracker

A real-time stock and cryptocurrency portfolio tracking system built with .NET 10. The application allows users to manage investment portfolios, track live prices from external market APIs, receive price alert notifications through WebSocket connections, and view profit/loss calculations on a performance dashboard.

This project was built as a backend engineering portfolio piece to demonstrate proficiency in distributed systems, real-time communication, caching strategies, and clean architecture patterns commonly expected in mid-level .NET developer roles.

---

## Table of Contents

- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Features](#features)
- [API Endpoints](#api-endpoints)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Testing](#testing)
- [CI/CD](#cicd)

---

## Architecture

The system follows Clean Architecture principles with four layers and two independently deployable processes.

```
                                      External
                                    (CoinGecko)
                                        |
                                        v
+-----------+     RabbitMQ      +-------+-------+     Redis
|   Worker  | ----------------> |      API      | <----------> Cache
| (background)  PriceUpdated   | (Minimal API) |
|           |   AlertTriggered  |               |
+-----------+                   +---+-----------+
      |                             |       |
      |        PostgreSQL           |       | SignalR
      +----------+-----------+-----+       | (WebSocket)
                 |                          |
            [ Database ]              [ Clients ]
```

**Worker process** polls the CoinGecko API every 30 seconds, stores prices in Redis, and publishes price change events to RabbitMQ. The **API process** consumes those events and pushes real-time updates to connected clients via SignalR. Both processes share the same PostgreSQL database and Redis cache.

The read and write paths are separated following a lightweight CQRS approach. Write operations go through Entity Framework Core with full domain validation. Read operations for the dashboard use Dapper with raw SQL for performance, enriched with live price data from Redis.

---

## Tech Stack

| Category | Technology |
|---|---|
| Framework | .NET 10, ASP.NET Core Minimal API |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core (writes), Dapper (reads) |
| Caching | Redis 7 |
| Message Broker | RabbitMQ 3 with MassTransit |
| Real-Time | SignalR (WebSocket) |
| Authentication | JWT Bearer tokens with refresh token rotation |
| Password Hashing | BCrypt |
| Resilience | Polly (retry, circuit breaker) |
| External API | CoinGecko (cryptocurrency prices) |
| Logging | Serilog (structured, console sink) |
| Testing | xUnit, Moq, FluentAssertions |
| Containerization | Docker, Docker Compose |
| CI/CD | GitHub Actions |

---

## Project Structure

```
PortfolioTracker/
├── src/
│   ├── PortfolioTracker.Api/              # HTTP API host, endpoints, SignalR hub, consumers
│   ├── PortfolioTracker.Application/      # Business logic, services, interfaces, DTOs
│   ├── PortfolioTracker.Domain/           # Entities, enums, domain events
│   ├── PortfolioTracker.Infrastructure/   # EF Core, Dapper, Redis, CoinGecko client
│   └── PortfolioTracker.Worker/           # Background jobs (price fetching, alert checking)
├── tests/
│   ├── PortfolioTracker.UnitTests/        # Service-level unit tests
│   └── PortfolioTracker.IntegrationTests/ # HTTP endpoint integration tests
├── docker-compose.yml
└── .github/workflows/ci.yml
```

**Dependency flow:** Domain has no dependencies. Application depends on Domain. Infrastructure depends on Application. API and Worker depend on Application and Infrastructure. Dependencies always point inward.

---

## Features

### Portfolio Management
- Create and delete investment portfolios
- Record buy and sell transactions against holdings
- Automatic average buy price recalculation on each purchase
- Quantity validation on sell operations

### Real-Time Price Tracking
- Background worker fetches cryptocurrency prices from CoinGecko every 30 seconds
- Prices cached in Redis with 60-second TTL
- Cache-first read pattern with API fallback on cache miss
- Polly retry policies with exponential backoff and circuit breaker on external API calls

### Live Updates via SignalR
- WebSocket connection at `/hubs/prices`
- Clients subscribe to specific symbol groups for targeted updates
- Global price feed for dashboard views
- User-specific alert notifications routed by authenticated user ID

### Price Alerts
- Users define alerts with target price and direction (above/below)
- Background job compares cached prices against active alerts every 30 seconds
- Triggered alerts are marked and published through RabbitMQ
- Real-time notification delivered to the specific user via SignalR

### Performance Dashboard (CQRS)
- Portfolio dashboard with total invested, current value, and profit/loss calculations
- Holding-level detail with per-asset P&L and percentage returns
- Read path uses Dapper with raw SQL for fast aggregation queries
- Live prices from Redis combined with database investment data

### Authentication
- JWT access tokens (15-minute expiry)
- Refresh token rotation (7-day expiry) with cryptographically secure token generation
- BCrypt password hashing
- Ownership verification on all resource endpoints

---

## API Endpoints

### Authentication
| Method | Route | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Authenticate and receive tokens |
| POST | `/api/auth/refresh` | Rotate refresh token and get new access token |

### Portfolios (requires authentication)
| Method | Route | Description |
|---|---|---|
| GET | `/api/portfolios` | List all portfolios for the current user |
| GET | `/api/portfolios/dashboard` | Aggregated P&L dashboard with live prices |
| POST | `/api/portfolios` | Create a new portfolio |
| GET | `/api/portfolios/{id}` | Get portfolio with holdings |
| GET | `/api/portfolios/{id}/holdings` | Detailed holding breakdown with live P&L |
| DELETE | `/api/portfolios/{id}` | Delete a portfolio |
| POST | `/api/portfolios/{id}/transactions` | Record a buy or sell transaction |

### Alerts (requires authentication)
| Method | Route | Description |
|---|---|---|
| POST | `/api/alerts` | Create a price alert |
| GET | `/api/alerts` | List active (untriggered) alerts |
| DELETE | `/api/alerts/{id}` | Delete an alert |

### Other
| Method | Route | Description |
|---|---|---|
| GET | `/health` | Health check |
| WS | `/hubs/prices` | SignalR hub for real-time price updates |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run with Docker Compose (recommended)

```bash
git clone https://github.com/aykutkaann/PortfolioTracker.git
cd PortfolioTracker
docker-compose up --build
```

This starts all five services: PostgreSQL, Redis, RabbitMQ, the API, and the Worker. The API applies database migrations automatically on startup.

- API: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`
- RabbitMQ Management: `http://localhost:15672` (guest/guest)

### Run locally (for development)

Start only the infrastructure services:

```bash
docker-compose up postgres redis rabbitmq -d
```

Then run the API and Worker separately:

```bash
cd src/PortfolioTracker.Api
dotnet run

cd src/PortfolioTracker.Worker
dotnet run
```

---

## Configuration

Application settings are managed through `appsettings.Development.json`. Docker Compose overrides these values using environment variables.

| Setting | Development Default | Description |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | `Host=localhost;Port=5432;...` | PostgreSQL connection |
| `ConnectionStrings:Redis` | `localhost:6379` | Redis connection |
| `RabbitMq:Host` | `localhost` | RabbitMQ hostname |
| `JwtSettings:Key` | 32+ character secret | JWT signing key |
| `JwtSettings:Issuer` | `PortfolioTracker` | Token issuer |
| `JwtSettings:Audience` | `PortfolioTracker` | Token audience |
| `JwtSettings:ExpiryMinutes` | `30` | Access token lifetime |

---

## Testing

### Unit Tests

```bash
dotnet test tests/PortfolioTracker.UnitTests
```

Unit tests cover the service layer using Moq for dependency isolation and FluentAssertions for readable assertions. Tests verify authentication flows, portfolio CRUD operations, ownership checks, and transaction logic.

### Integration Tests

```bash
dotnet test tests/PortfolioTracker.IntegrationTests
```

Integration tests run against real PostgreSQL and Redis instances (started via Docker or GitHub Actions service containers).

---

## CI/CD

The GitHub Actions pipeline (`.github/workflows/ci.yml`) runs on every push to `main` or `develop` and on pull requests.

**Pipeline stages:**

1. **Build and Test** -- Restores, builds, and runs all unit and integration tests against PostgreSQL, Redis, and RabbitMQ service containers.
2. **Docker Build** -- Builds both API and Worker Docker images to verify containerization. Only runs if all tests pass.

---

## Design Decisions

**Why Redis for caching instead of in-memory?**
The API and Worker are separate processes. In-memory cache is not shared across processes. Redis provides a distributed cache that both services can read from and write to.

**Why RabbitMQ between Worker and API?**
The Worker fetches prices and the API serves clients. They need to communicate without direct coupling. RabbitMQ decouples them -- if the API restarts, messages queue up and are delivered when it recovers. If the system scales to multiple API instances, all of them receive the events.

**Why Dapper for reads and EF Core for writes?**
Write operations benefit from EF Core's change tracking, validation, and relationship management. Read operations for the dashboard need fast aggregation queries that are simpler and more performant as raw SQL. This is a pragmatic CQRS split without the complexity of separate databases.

**Why refresh token rotation?**
A stolen refresh token becomes useless after one use because rotation issues a new token on every refresh. This limits the attack window compared to long-lived static refresh tokens.
