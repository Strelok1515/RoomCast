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

        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumFile> AlbumFiles { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<ScreenMediaAssignment> ScreenMediaAssignments { get; set; }
        public DbSet<AlbumScreenAssignment> AlbumScreenAssignments { get; set; }


        // ✅ Add this override to define relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationship: Screen ↔ ScreenMediaAssignment (one-to-many)
            modelBuilder.Entity<ScreenMediaAssignment>()
                .HasOne(s => s.Screen)
                .WithMany(m => m.ScreenMediaAssignments)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define relationship: MediaFile ↔ ScreenMediaAssignment (one-to-many)
            modelBuilder.Entity<ScreenMediaAssignment>()
                .HasOne(s => s.MediaFile)
                .WithMany()
                .HasForeignKey(s => s.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
