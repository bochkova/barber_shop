using lab1_bochkova.Models;
using System;

namespace lab1_bochkova.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public int CustomerId { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
