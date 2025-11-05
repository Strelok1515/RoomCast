using System;
using System.Collections.Generic;
using RoomCast.Models;

namespace RoomCast.ViewModels
{
    public class ScreenAssignmentViewModel
    {
        public Guid ScreenId { get; set; }
        public string ScreenName { get; set; } = string.Empty;

        public List<MediaFile> AllMediaFiles { get; set; } = new();
        public List<int> SelectedMediaIds { get; set; } = new();
    }
}
