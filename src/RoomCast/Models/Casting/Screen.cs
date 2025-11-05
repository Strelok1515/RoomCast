using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace RoomCast.Models.Casting
{
    public class Screen
    {
        public Guid ScreenId { get; set; }

        [Required(ErrorMessage = "Screen name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ These are navigation properties and should not be validated on create/edit
        [ValidateNever]
        public ICollection<ScreenMediaAssignment>? ScreenMediaAssignments { get; set; }

        [ValidateNever]
        public ICollection<AlbumScreenAssignment>? AlbumScreenAssignments { get; set; }
    }
}
