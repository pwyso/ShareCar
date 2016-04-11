using System.Web.Mvc;
using ShareCar.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ShareCar.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Home/Index
        public async Task<ActionResult> Index()
        {
            // Get lift offers with min. 1 seat available, order by descending - from most recent to oldest
            var offersAll = await db.LiftOffers.OrderByDescending(
                            o => o.LiftOfferID).Where(o => o.SeatsAvailable > 0).ToListAsync();
            // Create list with not expired offers to display in a view
            var offersNotExpired = new List<LiftOffer>();

            foreach (var off in offersAll)
            {
                // If EndDate null, then add offer to list of not expired offers
                if (off.EndDate == null)
                {
                    offersNotExpired.Add(off);
                }
                // Get EndDate from offer. If null then set default - 01/01/0001
                DateTime endDate = off.EndDate.GetValueOrDefault();
                // Check if EndDate is older then current date (offer expired ?)
                int result = DateTime.Compare(endDate, DateTime.Today);
                // If EndDate not older, then add offer to list of not expired offers
                if (result >= 0)
                {
                    offersNotExpired.Add(off);
                }
            }
            return View(offersNotExpired.Take(3).ToList());
        }

        // GET: Home/About
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        // GET: Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
