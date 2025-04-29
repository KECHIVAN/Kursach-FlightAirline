namespace FlightAirline.Models
{
    public class FlightDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DepartureAirportId { get; set; }
        public AirportDto? DepartureAirport { get; set; }
        public int ArrivalAirportId { get; set; }
        public AirportDto? ArrivalAirport { get; set; }
        public List<FlightUpdateLogDto> UpdateLogs { get; set; } = new List<FlightUpdateLogDto>();
        public List<TrackedFlightDto> TrackedFlights { get; set; } = new List<TrackedFlightDto>();
    }

    public class AirportDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string TerminalNumber { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        // Не включаем Departures и Arrivals, чтобы разорвать цикл
    }

    public class FlightUpdateLogDto
    {
        public int Id { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int FlightId { get; set; }
    }

    public class TrackedFlightDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }
    }
}