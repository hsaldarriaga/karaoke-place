using karaoke_place.Data;
using karaoke_place.Models;
using Microsoft.EntityFrameworkCore;

namespace karaoke_place.Data;

public static class DatabaseSeeder
{
    private static readonly (int Id, string Email)[] SeedUsers =
    [
        (1, "user1@mock.local"),
        (2, "user2@mock.local"),
        (3, "user3@mock.local"),
    ];

    public static async Task SeedAsync(AppDbContext db)
    {
        var existingIds = await db.Users
            .AsNoTracking()
            .Select(u => u.Id)
            .ToHashSetAsync();

        var toInsert = SeedUsers.Where(u => !existingIds.Contains(u.Id)).ToList();
        if (toInsert.Count == 0) return;

        // Insert with explicit IDs using OVERRIDING SYSTEM VALUE (Postgres identity columns)
        foreach (var (id, email) in toInsert)
        {
            await db.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO "Users" ("Id", "Email", "CreatedAt")
                OVERRIDING SYSTEM VALUE
                VALUES ({0}, {1}, {2})
                ON CONFLICT ("Id") DO NOTHING
                """,
                id, email, DateTime.UtcNow);
        }

        // Reset the sequence so auto-generated IDs don't collide
        var maxId = SeedUsers.Max(u => u.Id);
        await db.Database.ExecuteSqlRawAsync(
            $"""SELECT setval(pg_get_serial_sequence('"Users"', 'Id'), GREATEST({maxId}, (SELECT MAX("Id") FROM "Users")))""");
    }
}
