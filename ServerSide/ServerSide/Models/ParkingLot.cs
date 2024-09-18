namespace ServerSide.Models
{
    public class ParkingLot
    {
        public string Id { get; set; }  // Unique Identifier (Primary Key)
        public string Name { get; set; }  // Name of the parking lot
        public decimal Longitude { get; set; }  // Longitude of the parking lot location
        public decimal Latitude { get; set; }  // Latitude of the parking lot location
    }
}
