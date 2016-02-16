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

namespace ShareCar.Controllers
{
    [Authorize]
    public class LiftOffersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LiftOffers
        public async Task<ActionResult> Index()
        {
            var liftOffers = db.LiftOffers.Include(l => l.User);
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
        public async Task<ActionResult> Create([Bind(Include = "LiftOfferID,StartDate,EndDate,DepartureHour,DepartureMin,ArrivalHour,ArrivalMin,StartPointName,EndPointName,CarMake,CarModel,SeatsAvailable,UserID")] LiftOffer liftOffer)
        {
            if (ModelState.IsValid)
            {
                db.LiftOffers.Add(liftOffer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
            return RedirectToAction("Index");
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
