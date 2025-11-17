using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomCast.Models.Casting
{
    public class AlbumScreenAssignment
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to Album
        public int AlbumId { get; set; }
        [ForeignKey("AlbumId")]
        public Album Album { get; set; }

        // Foreign key to Screen
        public Guid ScreenId { get; set; }
        [ForeignKey("ScreenId")]
        public Screen Screen { get; set; }

        // Optional: AssignedAt timestamp
        public DateTime? AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
