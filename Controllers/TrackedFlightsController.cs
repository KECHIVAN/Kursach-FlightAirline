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
    public class TrackedFlightsController : ControllerBase
    {
        private readonly AirportScheduleContext _context;
        private readonly NLog.ILogger _logger;

        public TrackedFlightsController(AirportScheduleContext context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackedFlight>>> GetTrackedFlights(int? userId)
        {
            _logger.Info("Запрос на получение отслеживаемых рейсов: userId={0}", userId);

            var query = _context.TrackedFlights
                .Include(tf => tf.Flight)
                .ThenInclude(f => f.UpdateLogs)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(tf => tf.UserId == userId.Value);
            }

            var trackedFlights = await query.ToListAsync();
            _logger.Info("Возвращено {0} отслеживаемых рейсов", trackedFlights.Count);
            return trackedFlights;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrackedFlight>> GetTrackedFlight(int id)
        {
            _logger.Info("Запрос на получение отслеживаемого рейса с ID={0}", id);

            var trackedFlight = await _context.TrackedFlights
                .Include(tf => tf.Flight)
                .ThenInclude(f => f.UpdateLogs)
                .FirstOrDefaultAsync(tf => tf.Id == id);

            if (trackedFlight == null)
            {
                _logger.Warn("Отслеживаемый рейс с ID={0} не найден", id);
                return NotFound();
            }

            _logger.Info("Отслеживаемый рейс с ID={0} успешно возвращён", id);
            return trackedFlight;
        }

        [HttpPost]
        public async Task<ActionResult<TrackedFlight>> PostTrackedFlight(TrackedFlight trackedFlight)
        {
            _logger.Info("Запрос на добавление отслеживаемого рейса: UserId={0}, FlightId={1}", trackedFlight.UserId, trackedFlight.FlightId);

            if (!await _context.Users.AnyAsync(u => u.Id == trackedFlight.UserId) ||
                !await _context.Flights.AnyAsync(f => f.Id == trackedFlight.FlightId))
            {
                _logger.Error("Неверный UserId или FlightId");
                return BadRequest("Invalid UserId or FlightId");
            }

            _context.TrackedFlights.Add(trackedFlight);
            await _context.SaveChangesAsync();

            _logger.Info("Отслеживаемый рейс с ID={0} успешно добавлен", trackedFlight.Id);
            return CreatedAtAction(nameof(GetTrackedFlight), new { id = trackedFlight.Id }, trackedFlight);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrackedFlight(int id)
        {
            _logger.Info("Запрос на удаление отслеживаемого рейса с ID={0}", id);

            var trackedFlight = await _context.TrackedFlights.FindAsync(id);
            if (trackedFlight == null)
            {
                _logger.Warn("Отслеживаемый рейс с ID={0} не найден", id);
                return NotFound();
            }

            _context.TrackedFlights.Remove(trackedFlight);
            await _context.SaveChangesAsync();
            _logger.Info("Отслеживаемый рейс с ID={0} успешно удалён", id);
            return NoContent();
        }
    }
}