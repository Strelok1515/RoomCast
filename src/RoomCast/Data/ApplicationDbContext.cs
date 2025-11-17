using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomCast.Models;
using RoomCast.Models.Casting;

namespace RoomCast.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ===== YOUR TABLES =====
        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumFile> AlbumFiles { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<ScreenMediaAssignment> ScreenMediaAssignments { get; set; }
        public DbSet<AlbumScreenAssignment> AlbumScreenAssignments { get; set; }

        // ===== NEW: Casting Assignment Table =====
        public DbSet<CastingAssignment> CastingAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -----------------------------------------
            // RELATIONSHIPS YOU ALREADY HAVE
            // -----------------------------------------

            modelBuilder.Entity<ScreenMediaAssignment>()
                .HasOne(s => s.Screen)
                .WithMany(m => m.ScreenMediaAssignments)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ScreenMediaAssignment>()
                .HasOne(s => s.MediaFile)
                .WithMany()
                .HasForeignKey(s => s.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------------------
            // NEW: Seed the CastingAssignment table
            // -----------------------------------------

            modelBuilder.Entity<CastingAssignment>().HasData(
                new CastingAssignment
                {
                    Id = 1,                     // Always 1 row
                    CurrentScreenId = null
                }// No casting yet
                    
            );
        }
    }
}
