using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using lab1_bochkova.Data;
using lab1_bochkova.Models;
using System.Collections.Generic;
using System.Linq;

namespace lab1_bochkova.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly BarbershopContext _context;

        // Конструктор для получения контекста базы данных
        public AppointmentsController(BarbershopContext context)
        {
            _context = context;
        }

        // Получить все записи
        [HttpGet]
        public ActionResult<IEnumerable<Appointment>> GetAppointments()
        {
            return _context.Appointments.ToList();
        }

        // Получить запись по ID
        [HttpGet("{id}")]
        public ActionResult<Appointment> GetAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
                return NotFound();
            return appointment;
        }

        // Создать новую запись
        [HttpPost]
        public ActionResult<Appointment> CreateAppointment(Appointment appointment)
        {
            // Проверяем, существует ли парикмахер с указанным ID
            var barber = _context.Barbers.FirstOrDefault(b => b.Id == appointment.BarberId);
            if (barber == null)
                return NotFound("Парикмахер не найден.");

            // Проверяем, существует ли клиент с указанным ID
            var customer = _context.Customers.FirstOrDefault(c => c.Id == appointment.CustomerId);
            if (customer == null)
                return NotFound("Клиент не найден.");

            // Проверяем, существует ли услуга с указанным ID
            var service = _context.Services.FirstOrDefault(s => s.Id == appointment.ServiceId);
            if (service == null)
                return NotFound("Услуга не найдена.");

            // Проверяем, доступен ли парикмахер в указанное время
            if (!_context.CanAppointment(appointment.StartTime, barber, service))
                return BadRequest("Парикмахер недоступен в выбранное время.");

            appointment.EndTime = appointment.StartTime.AddMinutes(service.Duration);

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        // Обновить существующую запись
        [HttpPut("{id}")]
        public IActionResult UpdateAppointment(int id, Appointment appointment)
        {
            var existingAppointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (existingAppointment == null)
                return NotFound();

            var barber = _context.Barbers.FirstOrDefault(b => b.Id == appointment.BarberId);
            if (barber == null)
                return NotFound("Парикмахер не найден.");

            var customer = _context.Customers.FirstOrDefault(c => c.Id == appointment.CustomerId);
            if (customer == null)
                return NotFound("Клиент не найден.");

            var service = _context.Services.FirstOrDefault(s => s.Id == appointment.ServiceId);
            if (service == null)
                return NotFound("Услуга не найдена.");

            if (!_context.CanAppointment(appointment.StartTime, barber, service))
                return BadRequest("Парикмахер недоступен в выбранное время.");

            appointment.EndTime = appointment.StartTime.AddMinutes(service.Duration);

            // Обновляем данные существующей записи
            existingAppointment.StartTime = appointment.StartTime;
            existingAppointment.EndTime = appointment.EndTime;
            existingAppointment.BarberId = appointment.BarberId;
            existingAppointment.CustomerId = appointment.CustomerId;
            existingAppointment.ServiceId = appointment.ServiceId;

            _context.SaveChanges();
            return Ok(existingAppointment);
        }

        // Удалить запись по ID
        [HttpDelete("{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
