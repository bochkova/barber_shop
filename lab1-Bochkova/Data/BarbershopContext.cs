
using lab1_bochkova.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace lab1_bochkova.Data
{
    public class BarbershopContext : DbContext
    {
        public BarbershopContext(DbContextOptions<BarbershopContext> options)
            : base(options)
        {
        }

        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Добавление начальных данных для Barbers
            modelBuilder.Entity<Barber>().HasData(
                new Barber { Id = 1, Name = "Иван Иванов", ExperienceLevel = 5 },
                new Barber { Id = 2, Name = "Петр Петров", ExperienceLevel = 3 }
            );

            // Добавление начальных данных для Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "Алексей Смирнов", PreferredStyle = "Классический" },
                new Customer { Id = 2, Name = "Максим Сидоров", PreferredStyle = "Модерн" }
            );

            // Добавление начальных данных для Appointments
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Стрижка машинкой", Price = 1000, Duration = 30 },
                new Service { Id = 2, Name = "Стрижка ножницами", Price = 1500, Duration = 60 }
            );

            DateTime time1 = new DateTime(2024, 10, 20, 17, 0, 0, 0, DateTimeKind.Local);
            DateTime time2 = new DateTime(2024, 10, 20, 19, 0, 0, 0, DateTimeKind.Local);

            // Добавление начальных данных для Appointments
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, BarberId = 1, CustomerId = 1, ServiceId = 1, StartTime = time1, EndTime = time1.AddMinutes(30) },
                new Appointment { Id = 2, BarberId = 2, CustomerId = 2, ServiceId = 2, StartTime = time2, EndTime = time2.AddMinutes(60) }
            );
        }

        public bool CanAppointment(DateTime time, Barber barber, Service service)
        {
            var appointment = this.Appointments.FirstOrDefault(a => a.BarberId == barber.Id &&
                ((a.StartTime <= time && time < a.EndTime) || (a.StartTime < time.AddMinutes(service.Duration) && time.AddMinutes(service.Duration) <= a.EndTime))
            );
            return appointment == null;
        }
    }
}
