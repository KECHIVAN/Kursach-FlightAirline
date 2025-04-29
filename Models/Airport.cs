using System.ComponentModel.DataAnnotations;

namespace FlightAirline.Models
{
    public class Airport
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string TerminalNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContactInfo { get; set; } = string.Empty;

        public ICollection<Flight> Departures { get; set; } = new List<Flight>();
        public ICollection<Flight> Arrivals { get; set; } = new List<Flight>();
    }
}