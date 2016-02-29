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
            var liftOffers = db.LiftOffers.OrderByDescending(o => o.LiftOfferID);
            int no = await liftOffers.CountAsync();

            if (no > 3)
            {
                return View(await liftOffers.Take(3).ToListAsync());
            }
            else
            {
                return View(await liftOffers.ToListAsync());
            }
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
