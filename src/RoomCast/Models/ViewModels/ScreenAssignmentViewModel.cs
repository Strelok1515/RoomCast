using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RoomCast.Models;
using System.ComponentModel.DataAnnotations;

namespace RoomCast.ViewModels
{
    public class ScreenAssignmentViewModel
    {
        [Required]
        public Guid ScreenId { get; set; }

        public string ScreenName { get; set; } = string.Empty;

        [ValidateNever]
        public List<MediaFile> AllMediaFiles { get; set; } = new();

        // IMPORTANT: this must be List<int> and the view must post this exact name
        public List<int> SelectedMediaIds { get; set; } = new();
    }
}

