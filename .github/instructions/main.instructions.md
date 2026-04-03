# Copilot Agent Instructions

## Architecture snapshot

- API controllers live in `Api/*`.
- Business logic lives in `Modules/*`.
- Persistence is EF Core with `AppDbContext` in `Data/AppDbContext.cs`.
- EF entities are in `Models/*` and use a `DB` suffix (example: `KaraokeEventDB`).
- Service-layer/domain models for Karaoke events are in `Modules/KaraokeEvents/Models/*`.

Current implemented module split:
- `Api/KaraokeEvents/KaraokeEventsController.cs` accepts/returns HTTP payloads and maps DTOs to service models.
- `Modules/KaraokeEvents/KaraokeEventService.cs` coordinates business rules.
- `Modules/KaraokeEvents/KaraokeEventRepository.cs` handles data access and maps EF entities to service models.
- `Api/Diagnostic/DiagnosticController.cs` and `Modules/Diagnostic/*` provide health/version diagnostics.

## Layering and mapping rules

- Never expose EF entity types (`*DB`) from controllers or services.
- Repository is the only layer that touches `AppDbContext` and EF entities.
- Controllers should only consume API DTOs and map them to module models before invoking services.
- Keep service and repository as concrete classes (no interface abstraction unless explicitly requested).

## Entity and database safety rules

- Preserve table names with `[Table("...")]` attributes when renaming entity classes.
- Keep existing delete behaviors and unique indexes defined in `OnModelCreating` unless the task requires schema change.
- For schema changes, create EF migrations; do not hand-edit snapshot/designer files unless necessary.

## Validation and API behavior

- Use data annotations + `IValidatableObject` on DTOs for request validation.
- Keep controller responses consistent with existing behavior (`NotFound`, `BadRequest`, `NoContent`, `CreatedAtAction`).
- For time-based checks, use UTC (`DateTime.UtcNow`) to match existing code.

## Dependency injection and startup

- Register new services/repositories in `Program.cs` with scoped lifetime unless there is a strong reason otherwise.
- Database connection comes from environment variable `DATABASE_URL` (loaded via `.env` using `DotNetEnv`).

## Coding style conventions

- Follow existing C# style: file-scoped namespaces, primary constructors where already used, concise methods.
- Keep changes small and focused; avoid broad refactors unless requested.
- Prefer async EF APIs and `AsNoTracking()` for read-only query paths.

## Useful commands

- Build: `dotnet build ./karaoke-place.sln`
- Run: `dotnet run --project karaoke-place.csproj`
- Add EF migration: `dotnet ef migrations add <Name>`
- Apply DB update: `dotnet ef database update`
