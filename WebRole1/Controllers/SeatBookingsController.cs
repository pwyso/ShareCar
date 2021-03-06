﻿using Microsoft.AspNet.Identity;
using ShareCar.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShareCar.Controllers
{
    [RequireHttps]
    [Authorize]
    public class SeatBookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SeatBookings/Create/5       - Book a seat for the LiftOffer
        public async Task<ActionResult> Create(int? id)                 // Passed LiftOfferID
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
            // Get ID of currently logged-in user
            ViewBag.CurrentID = User.Identity.GetUserId();
            // Get names of selected days only to display in view
            var selectedDays = await db.Days.Where(
                d => d.LiftOfferID == id && d.Selected == true).Select(d => d.DayName).ToListAsync();
            // Only selected days displayed in view  
            ViewBag.SelectedDays = selectedDays;                       
            return View(liftOffer);
        }

        // POST: SeatBookings/Create/5                                  
        // requestedSeats from dropdown list in view                                
        [HttpPost]        
        [ValidateAntiForgeryToken]                                               
        public async Task<ActionResult> Create(int id, string requestedSeats)   // Passed LiftOfferid  
        {    
            if (requestedSeats == "")
            {
                // validation error message to be implemented
                //requestedSeats = "0"; 
                return RedirectToAction("Create", id);
            }                                                              
            if (ModelState.IsValid)
            {
                LiftOffer liftOffer = await db.LiftOffers.FindAsync(id);
                // Get lift oferer details
                User userDetails = db.Users.Find(liftOffer.UserID);             
                SeatBooking seatBooking = new SeatBooking();
                seatBooking.LiftOfferID = id;
                // Set UserID to currently logged-in user              
                seatBooking.UserID = User.Identity.GetUserId();
                // Set OffererID to lift giver ID
                seatBooking.OffererID = liftOffer.UserID;
                seatBooking.SeatsRequest = int.Parse(requestedSeats);   
                db.SeatBookings.Add(seatBooking);
                await db.SaveChangesAsync();

                return View("Confirmation", userDetails);
            }
            return View("Error");               
        }

        // GET: LiftOffers/Received     - Display received seat booking offers to lift giver
        public async Task<ActionResult> Received()
        {
            // Get ID of currently logged-in user
            var id = User.Identity.GetUserId();                         
            // Only seat bookings for logged-in user
            var seatBookings = await db.SeatBookings.Where(s => s.OffererID == id).ToListAsync();
            // To store detailed information about each seat booking offer      
            var modelList = new List<BookingDetailsModel>();            
            // Get User/LiftOffer/SeatBooking details for each seat booking offer to display in a view
            foreach (SeatBooking sb in seatBookings)
            {
                int liftOfferID = sb.LiftOfferID;
                // Get lift seeker ID 
                string seekerID = sb.UserID;
                LiftOffer offer = await db.LiftOffers.FindAsync(liftOfferID);              
                // Get lift seeker details to display in view (for lift offerer)
                User user = db.Users.Find(seekerID);
                // Check if feedback already left by lift giver, for lift seeker
                Feedback feedback = await db.Feedbacks.Where(f => f.SeatBookingID == sb.SeatBookingID && 
                                f.LeftByID == sb.OffererID && f.UserID == sb.UserID).FirstOrDefaultAsync();
                // All detailed information goes here for display in the view
                var model = new BookingDetailsModel();
                model.Feedback = feedback;
                model.User = user;
                model.LiftOffer = offer;
                model.SeatBooking = sb;
                modelList.Add(model);      
            }
            return View(modelList);                             
        }

        // GET: SeatBookings/Accept/5       - Accept received seat booking offer
        public async Task<ActionResult> Accept(int? id)                 // Passed SeatBookingID
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Get seat booking offer
            SeatBooking seatBooking = await db.SeatBookings.FindAsync(id);
            // Change seat booking offer status to true (Accepted)
            seatBooking.IsAccepted = true;
            db.Entry(seatBooking).State = EntityState.Modified;
            await db.SaveChangesAsync();
            // Get lift offer of which the seat booking offer has been created for
            LiftOffer liftOffer = await db.LiftOffers.FindAsync(seatBooking.LiftOfferID);
            if ((liftOffer == null) || (seatBooking == null))
            {
                return HttpNotFound();
            }
            if (seatBooking.SeatsRequest > liftOffer.SeatsAvailable)
            {
                return View("Error");
            }
            // Update seats available in the lift offer
            liftOffer.SeatsAvailable -= seatBooking.SeatsRequest;
            db.Entry(liftOffer).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Received");
        }

        // GET: SeatBookings/Decline/5     - Delete new (not accepted) seat booking offer 
        public async Task<ActionResult> Decline(int? id)                // Passed SeatBookingID
        {
            SeatBooking seatBooking = await db.SeatBookings.FindAsync(id);
            db.SeatBookings.Remove(seatBooking);
            await db.SaveChangesAsync();
            return RedirectToAction("Received");
        }

        // GET: SeatBookings/Delete/5      - Delete accepted seat booking offer
        public async Task<ActionResult> Delete(int? id)                 // Passed SeatBookingID
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeatBooking seatBooking = await db.SeatBookings.FindAsync(id);
            if (seatBooking == null)
            {
                return HttpNotFound();
            }
            string seekerID = seatBooking.UserID;
            // Get User/LiftOffer details for seat booking offer
            LiftOffer offer = await db.LiftOffers.FindAsync(seatBooking.LiftOfferID);
            User user = db.Users.Find(seekerID);
            // model to store detailed information about seat booking offer, to display in a view
            var model = new BookingDetailsModel();
            model.User = user;
            model.LiftOffer = offer;
            model.SeatBooking = seatBooking;
            return View(model);
        }

        // POST: SeatBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)         // Passed SeatBookingID
        {
            SeatBooking seatBooking = await db.SeatBookings.FindAsync(id);
            // Get lift offer to update number of seats available
            LiftOffer liftOffer = await db.LiftOffers.FindAsync(seatBooking.LiftOfferID);
            liftOffer.SeatsAvailable += seatBooking.SeatsRequest;
            db.Entry(liftOffer).State = EntityState.Modified;
            await db.SaveChangesAsync();
            db.SeatBookings.Remove(seatBooking);
            await db.SaveChangesAsync();
            return RedirectToAction("Received");
        }

        // GET: SeatBookings/MyBookings     - Display all user created seat booking requests
        public async Task<ActionResult> MyBookings()
        {
            // Get ID of currently logged-in user
            var id = User.Identity.GetUserId();
            // Only user seat booking requests
            var seatBookings = await db.SeatBookings.Where(s => s.UserID == id).ToListAsync();
            // modelList to store detailed information about each seat booking offer 
            var modelList = new List<BookingDetailsModel>();
            // Get User/LiftOffer/SeatBooking details for each seat booking offer to display in a view
            foreach (SeatBooking sb in seatBookings)
            {
                int liftOfferID = sb.LiftOfferID;
                // Get lift offer details to display in view
                LiftOffer offer = await db.LiftOffers.FindAsync(liftOfferID);
                // Get lift offerer ID
                string offererID = offer.UserID;
                // Get lift offerer ID to display details in view (for lift seeker)
                User user = db.Users.Find(offererID);
                // Check if feedback already left for lift giver
                Feedback feedback = await db.Feedbacks.Where(f => f.SeatBookingID == sb.SeatBookingID &&
                                f.LeftByID == sb.UserID && f.UserID == sb.OffererID).FirstOrDefaultAsync();
                // All detailed information goes here for display in the view
                BookingDetailsModel model = new BookingDetailsModel();
                model.Feedback = feedback;
                model.User = user;
                model.LiftOffer = offer;
                model.SeatBooking = sb;
                modelList.Add(model);
            }

            return View(modelList);
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