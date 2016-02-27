using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity.Migrations;

namespace ShareCar.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<User>
    {
        public ApplicationUserManager(IUserStore<User> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<User>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<User>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<User>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole,string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }


    // Drops and creates Database. Then populates it with sample data
    // DropCreateDatabaseIfModelChanges or DropCreateDatabaseAlways to changed as needed

    //public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {

            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            const string roleName = "Admin";
            const string nickName = "admin";
            const string userName = "admin@sharecar.org";
            const string userEmail = "admin@sharecar.org";
            const string password = "Pa$$word1";
            const string phone = "0850101010";

            if (!RoleManager.RoleExists(roleName))
            {
                var roleresult = RoleManager.Create(new IdentityRole(roleName));
            }

            var user = new User { UserName = userName, Email = userEmail, Name = nickName, PhoneNumber = phone };
            var adminresult = UserManager.Create(user, password);

            if (adminresult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, roleName);
            }
            base.Seed(context);

            var users = new List<User>
            {
                new User { UserName = "margaret@gmail.com", Email = "margaret@gmail.com", EmailConfirmed = true, Name = "Margaret", PhoneNumber = "087111111", Age = 29, IsSmoker = IsSmoking.No },
                new User { UserName = "adam@gmail.com", Email = "adam@gmail.com", EmailConfirmed = true, Name = "Adam", PhoneNumber = "087234465", Age = 22, IsSmoker = IsSmoking.No },
                new User { UserName = "lucy@gmail.com", Email = "lucy@gmail.com", EmailConfirmed = true, Name = "Lucy", PhoneNumber = "0868734627", Age = 46, IsSmoker = IsSmoking.No },
                new User { UserName = "steven@gmail.com", Email = "steven@gmail.com",EmailConfirmed = true, Name = "Steven", PhoneNumber = "0896743368", Age = 33, IsSmoker = IsSmoking.Yes }
            };

            foreach (var u in users)
            {
                UserManager.Create(u, password);
                base.Seed(context);
            }

            var offers = new List<LiftOffer>
            {
                new LiftOffer { CreateTime = DateTime.Parse("10/02/2016"), StartPointName = "Dublin", EndPointName = "Swords",
                                StartDate = DateTime.Parse("15/02/2016"), EndDate = DateTime.Parse("31/12/2016"),
                                DepartureHour = "10", DepartureMin = "00", ArrivalHour = "14", ArrivalMin = "00",
                                CarMake = "Honda", CarModel = "Accord", SeatsAvailable = 2, UserID = users.Single(s => s.Email == "adam@gmail.com").Id },
                new LiftOffer { CreateTime = DateTime.Parse("15/02/2016"), StartPointName = "Galway", EndPointName = "Limerick",
                                StartDate = DateTime.Parse("20/02/2016"), EndDate = DateTime.Parse("31/12/2017"),
                                DepartureHour = "16", DepartureMin = "15", ArrivalHour = "18", ArrivalMin = "30",
                                CarMake = "Ford", CarModel = "Mondeo", SeatsAvailable = 3, UserID = users.Single(s => s.Email == "lucy@gmail.com").Id },
                new LiftOffer { CreateTime = DateTime.Parse("20/02/2016"), StartPointName = "Wexford", EndPointName = "Dublin",
                                StartDate = DateTime.Parse("20/02/2016"), EndDate = DateTime.Parse("30/10/2016"),
                                DepartureHour = "09", DepartureMin = "00", ArrivalHour = "09", ArrivalMin = "30",
                                CarMake = "Toyota", CarModel = "Corolla", SeatsAvailable = 1, UserID = users.Single(s => s.Email == "steven@gmail.com").Id },
                new LiftOffer { CreateTime = DateTime.Parse("02/02/2016"), StartPointName = "Wexford", EndPointName = "Roslare",
                                StartDate = DateTime.Parse("25/02/2016"), EndDate = null,
                                DepartureHour = "06", DepartureMin = "30", ArrivalHour = "08", ArrivalMin = "30",
                                CarMake = "Toyota", CarModel = "Corolla", SeatsAvailable = 3, UserID = users.Single(s => s.Email == "steven@gmail.com").Id }
            };

            foreach (LiftOffer off in offers)
            {
                var offersForUser = context.LiftOffers.Where(s => s.User.Id == off.UserID
                                    && s.LiftOfferID == off.LiftOfferID).SingleOrDefault();

                context.LiftOffers.Add(off);
                context.SaveChanges();
            }
        }
    }

    public class ApplicationSignInManager : SignInManager<User, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)  
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}