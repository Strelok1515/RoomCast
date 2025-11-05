using RoomCast.Models.Casting;
using System.Collections.Generic;

namespace RoomCast.Models.ViewModels
{
    public class AlbumAssignmentViewModel
    {
        public int AlbumId { get; set; }
        public Guid SelectedScreenId { get; set; }

        public List<Screen> Screens { get; set; } = new();
    }
}
