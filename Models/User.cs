using System.ComponentModel.DataAnnotations;

namespace FlightAirline.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User"; 

        public ICollection<TrackedFlight> TrackedFlights { get; set; } = new List<TrackedFlight>();
    }
}