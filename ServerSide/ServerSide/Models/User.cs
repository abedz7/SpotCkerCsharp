namespace ServerSide.Models
{
    public class User
    {
        public string Id { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public List<Car> Cars { get; set; } = new List<Car>();  
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool HasDisabledCertificate { get; set; }
        public bool IsMom { get; set; }
    }
}
