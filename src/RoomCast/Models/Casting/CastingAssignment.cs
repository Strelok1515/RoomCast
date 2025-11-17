namespace RoomCast.Models.Casting
{
    public class CastingAssignment
    {
        public int Id { get; set; } = 1;

        // FIX: set this to Guid?
        public Guid? CurrentScreenId { get; set; }

        
    }
}
