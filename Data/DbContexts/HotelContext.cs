using Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class HotelContext : DbContext
    {
        public HotelContext(DbContextOptions<HotelContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Booking> Bookings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // تحديد سلوك الحذف المناسب


            modelBuilder.Entity<Room>()
                        .HasOne(r => r.Booking)
                        .WithOne(b => b.Room)
                        .HasForeignKey<Booking>(b => b.RoomId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomType>()
                        .HasMany(rt => rt.Rooms)
                        .WithOne(r => r.RoomType)
                        .HasForeignKey(r => r.RoomTypeId)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
