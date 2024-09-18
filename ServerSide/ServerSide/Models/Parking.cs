namespace ServerSide.Models
{
    public class Parking
    {
        public string Id { get; set; }
        public string SpotId { get; set; }
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
