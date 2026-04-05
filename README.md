# Karaoke Place API

Simple ASP.NET Core Web API for managing karaoke events, users, and songs.

## Prerequisites

- .NET SDK `10.0`
- Docker Desktop (recommended for PostgreSQL)
- Optional: `dotnet-ef`

Install EF CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

---

## Run locally from source

### 1) Start PostgreSQL

You can start only the database with Docker:

```bash
docker compose up -d db
```

### 2) Create a `.env` file

Create `.env` in the project root:

```env
DATABASE_URL=Host=localhost;Port=5432;Database=karaoke_place;Username=postgres;Password=postgres
```

### 3) Apply migrations

```bash
dotnet ef database update
```

### 4) Run the API

```bash
dotnet run --project karaoke-place.csproj
```

The API will be available at:

- `http://localhost:5068`
- `https://localhost:7158`

OpenAPI endpoint:

- `http://localhost:5068/openapi/v1.json`

---

## Run everything with Docker Compose

To run both the API and PostgreSQL in containers:

```bash
docker compose up --build
```

The API will be available at:

- `http://localhost:8080`

To stop everything:

```bash
docker compose down
```

To stop and remove the database volume too:

```bash
docker compose down -v
```
---

## Useful commands

Build the project:

```bash
dotnet build ./karaoke-place.sln
```

Run database migrations:

```bash
dotnet ef database update
```

Seed data script:

```bash
dotnet run seed.cs
```
