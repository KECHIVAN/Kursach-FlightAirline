using Microsoft.EntityFrameworkCore;
using FlightAirline.Models;

namespace FlightAirline.Data
{
    public class AirportScheduleContext : DbContext
    {
        public AirportScheduleContext(DbContextOptions<AirportScheduleContext> options)
            : base(options)
        {
        }

        public DbSet<Airport> Airports { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightUpdateLog> FlightUpdateLogs { get; set; }
        public DbSet<TrackedFlight> TrackedFlights { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.DepartureAirport)
                .WithMany(a => a.Departures)
                .HasForeignKey(f => f.DepartureAirportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.ArrivalAirport)
                .WithMany(a => a.Arrivals)
                .HasForeignKey(f => f.ArrivalAirportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FlightUpdateLog>()
                .HasOne<Flight>()
                .WithMany(f => f.UpdateLogs)
                .HasForeignKey(ful => ful.FlightId);

            modelBuilder.Entity<TrackedFlight>()
                .HasOne(tf => tf.Flight)
                .WithMany(f => f.TrackedFlights)
                .HasForeignKey(tf => tf.FlightId);

            modelBuilder.Entity<TrackedFlight>()
                .HasOne(tf => tf.User)
                .WithMany(u => u.TrackedFlights)
                .HasForeignKey(tf => tf.UserId);
        }
    }
}