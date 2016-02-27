using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShareCar.Models;
using Microsoft.AspNet.Identity;

namespace ShareCar.Controllers
{
    [Authorize]
    public class LiftOffersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LiftOffers/All searchString = filter, sortOrder = sort by StartPointName or EndPointName
        [AllowAnonymous]
        public ActionResult All(string searchString, string currentFilter, string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "start_name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "end_name" ? "end_name_desc" : "end_name";

            if (searchString != null)
            {
                ViewBag.CurrentFilter = searchString;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var offers = from s in db.LiftOffers select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                offers = offers.Where(o => o.StartPointName == searchString);
            }
            switch (sortOrder)
            {
                case "start_name_desc":
                    offers = offers.OrderByDescending(o => o.StartPointName);
                    break;
                case "end_name":
                    offers = offers.OrderBy(o => o.EndPointName);
                    break;
                case "end_name_desc":
                    offers = offers.OrderByDescending(o => o.EndPointName);
                    break;
                default:
                    offers = offers.OrderBy(o => o.StartPointName);
                    break;
            }

            return View(offers);
        }


        // GET: LiftOffers/UserOffers
        public async Task<ActionResult> UserOffers()
        {
            var id = User.Identity.GetUserId();
            var liftOffers = db.LiftOffers.OrderByDescending(o => o.StartPointName).Where(u => u.UserID == id);
            return View(await liftOffers.ToListAsync());
        }


        // GET: LiftOffers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOffer liftOffer = await db.LiftOffers.FindAsync(id);
            if (liftOffer == null)
            {
                return HttpNotFound();
            }
            return View(liftOffer);
        }

        // GET: LiftOffers/AnonymDetails/5
        [AllowAnonymous]
        public async Task<ActionResult> AnonymDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOffer liftOffer = await db.LiftOffers.FindAsync(id);
            if (liftOffer == null)
            {
                return HttpNotFound();
            }
            return View(liftOffer);
        }

        // GET: LiftOffers/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: LiftOffers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LiftOfferID,CreateTime,StartDate,EndDate,DepartureHour,DepartureMin,ArrivalHour,ArrivalMin,StartPointName,EndPointName,CarMake,CarModel,SeatsAvailable,UserID")] LiftOffer liftOffer)
        {
            var currentUser = User.Identity.GetUserId();
            liftOffer.UserID = currentUser;
            if (ModelState.IsValid)
            {
                db.LiftOffers.Add(liftOffer);
                await db.SaveChangesAsync();
                return RedirectToAction("UserOffers");
            }


            ViewBag.UserID = new SelectList(db.Users, "Id", "Name", liftOffer.UserID);
            return View(liftOffer);
        }

        // GET: LiftOffers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOffer liftOffer = await db.LiftOffers.FindAsync(id);
            if (liftOffer == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "Name", liftOffer.UserID);
            return View(liftOffer);
        }

        // POST: LiftOffers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LiftOfferID,StartDate,EndDate,DepartureHour,DepartureMin,ArrivalHour,ArrivalMin,StartPointName,EndPointName,CarMake,CarModel,SeatsAvailable,UserID")] LiftOffer liftOffer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liftOffer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("UserOffers");
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "Name", liftOffer.UserID);
            return View(liftOffer);
        }

        // GET: LiftOffers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOffer liftOffer = await db.LiftOffers.FindAsync(id);
            if (liftOffer == null)
            {
                return HttpNotFound();
            }
            return View(liftOffer);
        }

        // POST: LiftOffers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LiftOffer liftOffer = await db.LiftOffers.FindAsync(id);
            db.LiftOffers.Remove(liftOffer);
            await db.SaveChangesAsync();
            return RedirectToAction("UserOffers");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
