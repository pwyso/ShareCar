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
            // Get all lift offers and order them by descending - from most recent to oldest
            var offersAll = await db.LiftOffers.OrderByDescending(o => o.LiftOfferID).ToListAsync();
            //int no = await offersAll.CountAsync();

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

            //if (no > 3)
            //{
            //    // Return 3 most recent offers if more
            //    return View(await offersNotExpired.Take(3).ToList());
            //}
            //else
            //{
            return View(offersNotExpired.Take(3).ToList());
        }
    

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
