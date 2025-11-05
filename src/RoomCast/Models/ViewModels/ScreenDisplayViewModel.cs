using RoomCast.Models;
using System.Collections.Generic;

namespace RoomCast.ViewModels
{
    public class ScreenDisplayViewModel
    {
        public string ScreenName { get; set; } = string.Empty;

        // Media files directly assigned to the screen
        public List<MediaFile> AssignedMediaFiles { get; set; } = new();

        // Albums assigned to this screen
        public List<Album> AssignedAlbums { get; set; } = new();
    }
}
