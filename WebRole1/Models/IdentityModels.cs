﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShareCar.Models
{
    public class User : IdentityUser
    {
        // Added properties to IdentityUser class

        [Required(AllowEmptyStrings = false)]
        [RegularExpression("[a-zA-Z0-9]+", ErrorMessage = "Name: only numbers and letters allowed.")]
        [Display(Name = "Name"), StringLength(30, ErrorMessage = "Name: max. 30 characters allowed.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Smoker")]
        public IsSmoking IsSmoker { get; set; }

        [Required]
        [RegularExpression("[0-9]{2}", ErrorMessage = "Age: only numbers allowed.")]
        [Display(Name = "Age"), Range(18, 99, ErrorMessage = "Age: you must be min. 18 years old.")]
        public int Age { get; set; }

        [Display(Name = "Rating")]
        public double? RatingAvg { get; set; }

        // Relationship between tables - User:1 <=> M:LiftOffers 
        public virtual ICollection<LiftOffer> LiftOffers { get; set; }

        // Relationship between tables - User:1 <=> M:Feedbacks
        public virtual ICollection<Feedback> Feedbacks { get; set; }

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
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<SeatBooking> SeatBookings { get; set; }
    }
}