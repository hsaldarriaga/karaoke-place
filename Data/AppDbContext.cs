using Microsoft.EntityFrameworkCore;
using karaoke_place.Models;

namespace karaoke_place.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserDB> Users { get; set; } = null!;
    public DbSet<KaraokeEventDB> KaraokeEvents { get; set; } = null!;
    public DbSet<EventParticipantDB> EventParticipants { get; set; } = null!;
    public DbSet<SongDB> Songs { get; set; } = null!;
    public DbSet<SongProposalDB> SongProposals { get; set; } = null!;
    public DbSet<UserPreferredSongDB> UserPreferredSongs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User relationships
        modelBuilder.Entity<UserDB>()
            .HasMany(u => u.CreatedEvents)
            .WithOne(e => e.CreatedByUser)
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserDB>()
            .HasMany(u => u.EventParticipations)
            .WithOne(ep => ep.User)
            .HasForeignKey(ep => ep.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserDB>()
            .HasMany(u => u.SongProposals)
            .WithOne(sp => sp.User)
            .HasForeignKey(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserDB>()
            .HasMany(u => u.PreferredSongs)
            .WithOne(ps => ps.User)
            .HasForeignKey(ps => ps.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserDB>()
            .HasIndex(u => u.Auth0Subject)
            .IsUnique();

        // KaraokeEvent relationships
        modelBuilder.Entity<KaraokeEventDB>()
            .HasMany(e => e.Participants)
            .WithOne(ep => ep.Event)
            .HasForeignKey(ep => ep.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<KaraokeEventDB>()
            .HasMany(e => e.SongProposals)
            .WithOne(sp => sp.Event)
            .HasForeignKey(sp => sp.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Song relationships
        modelBuilder.Entity<SongDB>()
            .HasMany(s => s.SongProposals)
            .WithOne(sp => sp.Song)
            .HasForeignKey(sp => sp.SongId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SongDB>()
            .HasMany(s => s.PreferredByUsers)
            .WithOne(ps => ps.Song)
            .HasForeignKey(ps => ps.SongId)
            .OnDelete(DeleteBehavior.Restrict);
        // EventParticipant unique constraint
        modelBuilder.Entity<EventParticipantDB>()
            .HasIndex(ep => new { ep.EventId, ep.UserId })
            .IsUnique();

        // UserPreferredSong indexes
        modelBuilder.Entity<UserPreferredSongDB>()
            .HasIndex(ps => ps.UserId);

        modelBuilder.Entity<UserPreferredSongDB>()
            .HasIndex(ps => ps.SongId);

        modelBuilder.Entity<UserPreferredSongDB>()
            .HasIndex(ps => new { ps.UserId, ps.SongId })
            .IsUnique();
    }
}

