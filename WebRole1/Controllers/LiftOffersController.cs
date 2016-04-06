﻿using System;
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
using System.Collections.ObjectModel;

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

            //var offer = from s in db.LiftOffers select s;
            //var offers = db.LiftOffers.AsEnumerable();

            var offersAll = db.LiftOffers.ToList();
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
                if ( result >= 0)
                {
                    offersNotExpired.Add(off);
                }
            }

            // Pass only not expired offers
            var offers = offersNotExpired.AsEnumerable();

            if (!String.IsNullOrEmpty(searchString))
            {
                offers = offers.Where(o => o.EndPointName == searchString);
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
            var liftOffers = db.LiftOffers.OrderBy(o => o.StartPointName).Where(u => u.UserID == id);
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
            // Get names of selected days only to display in LiftOffer details
            var selectedDays = db.Days.Where(d => d.LiftOfferID == id && d.Selected == true).Select(d => d.DayName).ToList();
            ViewBag.SelectedDays = selectedDays;
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
            // Get names of selected days only to display in LiftOffer details
            var selectedDays = db.Days.Where(d => d.LiftOfferID == id && d.Selected == true).Select(d => d.DayName).ToList();
            ViewBag.SelectedDays = selectedDays;
            return View(liftOffer);
        }

        // GET: LiftOffers/Create
        public ActionResult Create()
        {
            //ViewBag.UserID = new SelectList(db.Users, "Id", "Name");
            var offer = new LiftOffer();
            offer.Days = DayRepository.GetAll().ToList();
            return View(offer);
        }

        // POST: LiftOffers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include =
            "LiftOfferID,StartPointName,EndPointName,StartDate,EndDate,DepartureHour,DepartureMin,ArrivalHour,ArrivalMin,CarMake,CarModel,SeatsAvailable,UserID")]
                LiftOffer liftOffer, List<Day> days)
        {
            // Set UserID to ID of currenttly logged-in user
            liftOffer.UserID = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                db.LiftOffers.Add(liftOffer);
                await db.SaveChangesAsync();

                foreach (Day d in days)
                {
                    d.LiftOfferID = liftOffer.LiftOfferID;
                    db.Days.Add(d);
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("UserOffers");
            }
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
            liftOffer.Days = await db.Days.Where(d => d.LiftOfferID == id).ToListAsync();
            //ViewBag.UserID = new SelectList(db.Users, "Id", "Name", liftOffer.UserID);
            return View(liftOffer);
        }

        // POST: LiftOffers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include =
            "LiftOfferID,StartPointName,EndPointName,StartDate,EndDate,DepartureHour,DepartureMin,ArrivalHour,ArrivalMin,CarMake,CarModel,SeatsAvailable,UserID")]
                LiftOffer liftOffer, List<Day> days)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liftOffer).State = EntityState.Modified;
                await db.SaveChangesAsync();

                foreach (Day d in days)
                {
                    d.LiftOfferID = liftOffer.LiftOfferID;
                    db.Entry(d).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("UserOffers");
            }
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
