using MHealth.Models.DTO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


/**
 * This model to connect with the database
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.Domain
{
    public class DatabaseContext : IdentityDbContext<UserModel>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) 
        { 
        }

        //public DbSet<LocationModel> Locations { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<MRIPostModel> MRIPosts { get; set; }
        public DbSet<RatingModel> Ratings { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Set Primary Key

            modelBuilder.Entity<BookingModel>()
                .HasKey(e => new { e.Id, e.UserId, e.StaffId });

            modelBuilder.Entity<MRIPostModel>()
                .HasKey(e => new { e.Id, e.UserId, e.StaffId });

            modelBuilder.Entity<RatingModel>()
                .HasKey(e => new { e.Id, e.UserId, e.StaffId });

            //Set Foreign Key Booking

            modelBuilder.Entity<UserModel>()
                .HasMany<BookingModel>()
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserModel>()
                .HasMany<BookingModel>()
                .WithOne()
                .HasForeignKey(e => e.StaffId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            //Set Foreign Key MRIPost

            modelBuilder.Entity<BookingModel>()
                .HasOne<MRIPostModel>()
                .WithOne()
                .HasForeignKey<MRIPostModel>(m => new { m.BookingId, m.UserId, m.StaffId })
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            //Set Foreign Key Rating

            modelBuilder.Entity<UserModel>()
                .HasMany<RatingModel>()
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserModel>()
                .HasMany<RatingModel>()
                .WithOne()
                .HasForeignKey(e => e.StaffId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
