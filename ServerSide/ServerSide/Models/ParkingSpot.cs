namespace ServerSide.Models
{
    public class ParkingSpot
    {
        public string Id { get; set; }  
        public string ParkingLotId { get; set; }  
        public int SpotNumber { get; set; }
        public string SpotType { get; set; }  
        public string ReservedByUserId { get; set; }  
        public bool IsAvailable { get; set; }
    }
}
