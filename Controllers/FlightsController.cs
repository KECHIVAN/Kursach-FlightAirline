using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightAirline.Data;
using FlightAirline.Models;
using NLog;
using Microsoft.AspNetCore.Authorization;

namespace FlightAirline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightsController : ControllerBase
    {
        private readonly AirportScheduleContext _context;
        private readonly NLog.ILogger _logger;

        public FlightsController(AirportScheduleContext context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetFlights(
            DateTime? date,
            string? flightNumber,
            string? status)
        {
            _logger.Info("Запрос на получение списка рейсов: date={0}, flightNumber={1}, status={2}",
                date, flightNumber, status);

            var query = _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.UpdateLogs)
                .AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(f => f.DepartureTime.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(flightNumber))
            {
                query = query.Where(f => f.FlightNumber.Contains(flightNumber));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(f => f.Status == status);
            }

            var flights = await query.ToListAsync();
            var flightDtos = flights.Select(f => new FlightDto
            {
                Id = f.Id,
                FlightNumber = f.FlightNumber,
                Airline = f.Airline,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                Status = f.Status,
                DepartureAirportId = f.DepartureAirportId,
                DepartureAirport = f.DepartureAirport != null ? new AirportDto
                {
                    Id = f.DepartureAirport.Id,
                    Name = f.DepartureAirport.Name,
                    City = f.DepartureAirport.City,
                    TerminalNumber = f.DepartureAirport.TerminalNumber,
                    ContactInfo = f.DepartureAirport.ContactInfo
                } : null,
                ArrivalAirportId = f.ArrivalAirportId,
                ArrivalAirport = f.ArrivalAirport != null ? new AirportDto
                {
                    Id = f.ArrivalAirport.Id,
                    Name = f.ArrivalAirport.Name,
                    City = f.ArrivalAirport.City,
                    TerminalNumber = f.ArrivalAirport.TerminalNumber,
                    ContactInfo = f.ArrivalAirport.ContactInfo
                } : null,
                UpdateLogs = f.UpdateLogs.Select(ul => new FlightUpdateLogDto
                {
                    Id = ul.Id,
                    UpdateDateTime = ul.UpdateDateTime,
                    NewStatus = ul.NewStatus,
                    Comment = ul.Comment,
                    FlightId = ul.FlightId
                }).ToList(),
                TrackedFlights = f.TrackedFlights.Select(tf => new TrackedFlightDto
                {
                    Id = tf.Id,
                    UserId = tf.UserId,
                    FlightId = tf.FlightId
                }).ToList()
            }).ToList();

            _logger.Info("Возвращено {0} рейсов", flights.Count);
            return flightDtos;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<FlightDto>> GetFlight(int id)
        {
            _logger.Info("Запрос на получение рейса с ID={0}", id);

            var flight = await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.UpdateLogs)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null)
            {
                _logger.Warn("Рейс с ID={0} не найден", id);
                return NotFound();
            }

            var flightDto = new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Airline = flight.Airline,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Status = flight.Status,
                DepartureAirportId = flight.DepartureAirportId,
                DepartureAirport = flight.DepartureAirport != null ? new AirportDto
                {
                    Id = flight.DepartureAirport.Id,
                    Name = flight.DepartureAirport.Name,
                    City = flight.DepartureAirport.City,
                    TerminalNumber = flight.DepartureAirport.TerminalNumber,
                    ContactInfo = flight.DepartureAirport.ContactInfo
                } : null,
                ArrivalAirportId = flight.ArrivalAirportId,
                ArrivalAirport = flight.ArrivalAirport != null ? new AirportDto
                {
                    Id = flight.ArrivalAirport.Id,
                    Name = flight.ArrivalAirport.Name,
                    City = flight.ArrivalAirport.City,
                    TerminalNumber = flight.ArrivalAirport.TerminalNumber,
                    ContactInfo = flight.ArrivalAirport.ContactInfo
                } : null,
                UpdateLogs = flight.UpdateLogs.Select(ul => new FlightUpdateLogDto
                {
                    Id = ul.Id,
                    UpdateDateTime = ul.UpdateDateTime,
                    NewStatus = ul.NewStatus,
                    Comment = ul.Comment,
                    FlightId = ul.FlightId
                }).ToList(),
                TrackedFlights = flight.TrackedFlights.Select(tf => new TrackedFlightDto
                {
                    Id = tf.Id,
                    UserId = tf.UserId,
                    FlightId = tf.FlightId
                }).ToList()
            };

            _logger.Info("Рейс с ID={0} успешно возвращён", id);
            return flightDto;
        }

        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            _logger.Info("Запрос на создание рейса: FlightNumber={0}", flight.FlightNumber);

            if (!await _context.Airports.AnyAsync(a => a.Id == flight.DepartureAirportId) ||
                !await _context.Airports.AnyAsync(a => a.Id == flight.ArrivalAirportId))
            {
                _logger.Error("Неверный DepartureAirportId или ArrivalAirportId");
                return BadRequest("Invalid DepartureAirportId or ArrivalAirportId");
            }

            flight.DepartureAirport = null;
            flight.ArrivalAirport = null;
            flight.UpdateLogs = null;
            flight.TrackedFlights = null;

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            var updateLog = new FlightUpdateLog
            {
                FlightId = flight.Id,
                UpdateDateTime = DateTime.UtcNow,
                NewStatus = flight.Status,
                Comment = "Рейс добавлен"
            };
            _context.FlightUpdateLogs.Add(updateLog);

            await _context.SaveChangesAsync();
            _logger.Info("Рейс с ID={0} успешно создан", flight.Id);
            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(int id, Flight flight)
        {
            _logger.Info("Запрос на обновление рейса с ID={0}", id);

            if (id != flight.Id)
            {
                _logger.Error("ID в запросе не совпадает с ID в теле");
                return BadRequest();
            }

            var existingFlight = await _context.Flights.FindAsync(id);
            if (existingFlight == null)
            {
                _logger.Warn("Рейс с ID={0} не найден", id);
                return NotFound();
            }

            if (existingFlight.Status != flight.Status)
            {
                var updateLog = new FlightUpdateLog
                {
                    FlightId = flight.Id,
                    UpdateDateTime = DateTime.UtcNow,
                    NewStatus = flight.Status,
                    Comment = "Статус обновлён"
                };
                _context.FlightUpdateLogs.Add(updateLog);
                _logger.Info("Добавлен лог обновления статуса для рейса с ID={0}", id);
            }

            _context.Entry(flight).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.Info("Рейс с ID={0} успешно обновлён", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            _logger.Info("Запрос на удаление рейса с ID={0}", id);

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                _logger.Warn("Рейс с ID={0} не найден", id);
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();
            _logger.Info("Рейс с ID={0} успешно удалён", id);
            return NoContent();
        }
    }
}