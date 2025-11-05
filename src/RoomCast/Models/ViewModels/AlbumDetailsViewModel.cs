using System.Collections.Generic;

namespace RoomCast.Models.ViewModels
{
    public class AlbumDetailsViewModel
    {
        public Album Album { get; set; } = null!;
        public List<MediaFile> MediaFiles { get; set; } = new();
    }
}
