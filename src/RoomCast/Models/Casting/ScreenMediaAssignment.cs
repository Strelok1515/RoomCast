using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoomCast.Models;

namespace RoomCast.Models.Casting
{
    public class ScreenMediaAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid ScreenId { get; set; }
        [ForeignKey(nameof(ScreenId))]
        public Screen Screen { get; set; }

        [Required]
        public int MediaFileId { get; set; }   // ✅ matches MediaFile.FileId
        [ForeignKey(nameof(MediaFileId))]
        public MediaFile MediaFile { get; set; }
    }
}
