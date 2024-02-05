using Microsoft.EntityFrameworkCore;
using Server.Data.Models;

namespace Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {

        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Profile> Profiles { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile) // Principal entity navigation
                .WithOne(up => up.User) // Dependent entity navigation
                .HasForeignKey<Profile>(up => up.Id); // Foreign key

            modelBuilder.Entity<Profile>()
                .HasOne(u => u.User) // Principal entity navigation
                .WithOne(up => up.Profile) // Dependent entity navigation
                .HasForeignKey<User>(up => up.Id); // Foreign key

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles) // Principal entity navigation
                .WithOne(up => up.User) // Dependent entity navigation
                .HasForeignKey(up => up.UserId); // Foreign key
        }
    }
}
