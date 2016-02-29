using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShareCar.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        // added property to IdentityUser class
        public string Name { get; set; }
        public IsSmoking IsSmoker { get; set; }
        public int Age { get; set; }

        public virtual ICollection<LiftOffer> LiftOffers { get; set; }

        //public virtual ICollection<Feedback> Feedbacks { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        // for Azure and Local DB
        public ApplicationDbContext() : base("csdb")
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user, admin role, users and lift offers
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Change default name of the tables to custom names
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<IdentityRole>().ToTable("Role");

            // Change default PK names to custom PK names
            modelBuilder.Entity<User>().ToTable("User").Property(p => p.Id).HasColumnName("UserID");
            modelBuilder.Entity<IdentityRole>().ToTable("Role").Property(p => p.Id).HasColumnName("RoleID");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole").Property(p => p.RoleId).HasColumnName("RoleID");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole").Property(p => p.UserId).HasColumnName("UserID");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin").Property(p => p.UserId).HasColumnName("UserID");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole").Property(p => p.UserId).HasColumnName("UserID");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim").Property(p => p.Id).HasColumnName("ClaimID");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim").Property(p => p.UserId).HasColumnName("UserID");
        }

        public DbSet<LiftOffer> LiftOffers { get; set; }

        //public DbSet<Feedback> Feedbacks { get; set; }
    }
}