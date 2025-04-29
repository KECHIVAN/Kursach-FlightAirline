namespace FlightAirline.Models
{
    public class TrackedFlight
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null!;
    }
}