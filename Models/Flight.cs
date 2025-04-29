using System.ComponentModel.DataAnnotations;

namespace FlightAirline.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string FlightNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Airline { get; set; } = string.Empty;

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        public int DepartureAirportId { get; set; }
        public Airport? DepartureAirport { get; set; }

        public int ArrivalAirportId { get; set; }
        public Airport? ArrivalAirport { get; set; }

        public ICollection<FlightUpdateLog> UpdateLogs { get; set; } = new List<FlightUpdateLog>();
        public ICollection<TrackedFlight> TrackedFlights { get; set; } = new List<TrackedFlight>();
    }
}