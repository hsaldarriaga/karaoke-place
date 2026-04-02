using Microsoft.EntityFrameworkCore;
using karaoke_place.Models;

namespace karaoke_place.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<KaraokeEvent> KaraokeEvents { get; set; } = null!;
    public DbSet<EventParticipant> EventParticipants { get; set; } = null!;
    public DbSet<Song> Songs { get; set; } = null!;
    public DbSet<SongProposal> SongProposals { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User relationships
        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedEvents)
            .WithOne(e => e.CreatedByUser)
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.EventParticipations)
            .WithOne(ep => ep.User)
            .HasForeignKey(ep => ep.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.SongProposals)
            .WithOne(sp => sp.User)
            .HasForeignKey(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // KaraokeEvent relationships
        modelBuilder.Entity<KaraokeEvent>()
            .HasMany(e => e.Participants)
            .WithOne(ep => ep.Event)
            .HasForeignKey(ep => ep.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<KaraokeEvent>()
            .HasMany(e => e.SongProposals)
            .WithOne(sp => sp.Event)
            .HasForeignKey(sp => sp.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Song relationships
        modelBuilder.Entity<Song>()
            .HasMany(s => s.SongProposals)
            .WithOne(sp => sp.Song)
            .HasForeignKey(sp => sp.SongId)
            .OnDelete(DeleteBehavior.SetNull);

        // EventParticipant unique constraint
        modelBuilder.Entity<EventParticipant>()
            .HasIndex(ep => new { ep.EventId, ep.UserId })
            .IsUnique();

        // SongProposal unique constraint
        modelBuilder.Entity<SongProposal>()
            .HasIndex(sp => new { sp.EventId, sp.Order })
            .IsUnique();
    }
}

