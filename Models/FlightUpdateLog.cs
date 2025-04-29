using System.ComponentModel.DataAnnotations;

namespace FlightAirline.Models
{
    public class FlightUpdateLog
    {
        public int Id { get; set; }

        public DateTime UpdateDateTime { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string NewStatus { get; set; } = string.Empty;

        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;

        public int FlightId { get; set; }
        // Свойство Flight удалено
    }
}