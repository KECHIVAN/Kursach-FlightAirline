using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightAirline.Data;
using FlightAirline.Models;
using NLog;
using Microsoft.AspNetCore.Authorization; // Добавь этот using

namespace FlightAirline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Требуем авторизацию для всех методов
    public class AirportsController : ControllerBase
    {
        private readonly AirportScheduleContext _context;
        private readonly NLog.ILogger _logger;

        public AirportsController(AirportScheduleContext context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
        }

        [HttpGet]
        [AllowAnonymous] // Разрешаем доступ без авторизации
        public async Task<ActionResult<IEnumerable<Airport>>> GetAirports()
        {
            _logger.Info("Запрос на получение списка аэропортов");
            var airports = await _context.Airports.ToListAsync();
            _logger.Info("Возвращено {0} аэропортов", airports.Count);
            return airports;
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Разрешаем доступ без авторизации
        public async Task<ActionResult<Airport>> GetAirport(int id)
        {
            _logger.Info("Запрос на получение аэропорта с ID={0}", id);
            var airport = await _context.Airports.FindAsync(id);
            if (airport == null)
            {
                _logger.Warn("Аэропорт с ID={0} не найден", id);
                return NotFound();
            }
            _logger.Info("Аэропорт с ID={0} успешно возвращён", id);
            return airport;
        }

        [HttpPost]
        public async Task<ActionResult<Airport>> PostAirport(Airport airport)
        {
            _logger.Info("Запрос на создание аэропорта: Name={0}", airport.Name);
            _context.Airports.Add(airport);
            await _context.SaveChangesAsync();
            _logger.Info("Аэропорт с ID={0} успешно создан", airport.Id);
            return CreatedAtAction(nameof(GetAirport), new { id = airport.Id }, airport);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirport(int id, Airport airport)
        {
            _logger.Info("Запрос на обновление аэропорта с ID={0}", id);
            if (id != airport.Id)
            {
                _logger.Error("ID в запросе не совпадает с ID в теле");
                return BadRequest();
            }

            _context.Entry(airport).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.Info("Аэропорт с ID={0} успешно обновлён", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirport(int id)
        {
            _logger.Info("Запрос на удаление аэропорта с ID={0}", id);
            var airport = await _context.Airports.FindAsync(id);
            if (airport == null)
            {
                _logger.Warn("Аэропорт с ID={0} не найден", id);
                return NotFound();
            }

            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();
            _logger.Info("Аэропорт с ID={0} успешно удалён", id);
            return NoContent();
        }
    }
}