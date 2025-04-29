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
    public class FlightUpdateLogsController : ControllerBase
    {
        private readonly AirportScheduleContext _context;
        private readonly NLog.ILogger _logger;

        public FlightUpdateLogsController(AirportScheduleContext context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightUpdateLog>>> GetFlightUpdateLogs(int? flightId)
        {
            _logger.Info("Запрос на получение логов обновлений: flightId={0}", flightId);

            var query = _context.FlightUpdateLogs.AsQueryable();

            if (flightId.HasValue)
            {
                query = query.Where(l => l.FlightId == flightId.Value);
            }

            var logs = await query.ToListAsync();
            _logger.Info("Возвращено {0} логов обновлений", logs.Count);
            return logs;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FlightUpdateLog>> GetFlightUpdateLog(int id)
        {
            _logger.Info("Запрос на получение лога обновления с ID={0}", id);

            var log = await _context.FlightUpdateLogs
                .FirstOrDefaultAsync(l => l.Id == id);

            if (log == null)
            {
                _logger.Warn("Лог с ID={0} не найден", id);
                return NotFound();
            }

            _logger.Info("Лог с ID={0} успешно возвращён", id);
            return log;
        }
    }
}