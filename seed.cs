#:package DotNetEnv@3.1.1
#:package Npgsql@9.0.3

using DotNetEnv;
using Npgsql;

Env.Load();

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("DATABASE_URL is not set in .env");

var seedUsers = new[]
{
    (Id: 1, Email: "user1@mock.local"),
    (Id: 2, Email: "user2@mock.local"),
    (Id: 3, Email: "user3@mock.local"),
};

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();

foreach (var (id, email) in seedUsers)
{
    await using var cmd = new NpgsqlCommand(
        """
        INSERT INTO "Users" ("Id", "Email", "CreatedAt")
        OVERRIDING SYSTEM VALUE
        VALUES ($1, $2, $3)
        ON CONFLICT ("Id") DO NOTHING
        """, conn);
    cmd.Parameters.AddWithValue(id);
    cmd.Parameters.AddWithValue(email);
    cmd.Parameters.AddWithValue(DateTime.UtcNow);
    await cmd.ExecuteNonQueryAsync();
    Console.WriteLine($"Seeded user {id} ({email})");
}

// Reset the identity sequence so future inserts don't collide
var maxId = seedUsers.Max(u => u.Id);
await using var seqCmd = new NpgsqlCommand(
    $"""SELECT setval(pg_get_serial_sequence('"Users"', 'Id'), GREATEST({maxId}, (SELECT MAX("Id") FROM "Users")))""",
    conn);
await seqCmd.ExecuteNonQueryAsync();

Console.WriteLine("Done.");
