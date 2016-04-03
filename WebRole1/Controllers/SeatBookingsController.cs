using Microsoft.AspNet.Identity;
using ShareCar.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShareCar.Controllers
{
    public class SeatBookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SeatBookings/Create/5   - Book seat for the LiftOffer
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
        public async Task<ActionResult> Create(int id, string requestedSeats)      
        {                                                                   
            if (ModelState.IsValid)
            {
                LiftOffer liftOffer = await db.LiftOffers.FindAsync(id);
                User contactDetails = db.Users.Find(liftOffer.UserID);             
                SeatBooking seatBooking = new SeatBooking();
                seatBooking.LiftOfferID = id;
                // Set UserID to currently logged-in user              
                seatBooking.UserID = User.Identity.GetUserId();         
                seatBooking.SeatsRequest = int.Parse(requestedSeats);   
                db.Entry(seatBooking).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return View("Confirmation", contactDetails);
            }
            return View("Error");               
        }

        // GET: LiftOffers/Received     - Display received seat booking offers
        public async Task<ActionResult> Received()
        {
            // Get ID of currently logged-in user
            var id = User.Identity.GetUserId();                         
            // Only seat bookings for logged-in user
            var seatBookings = await db.SeatBookings.Where(s => s.OffererID == id).ToListAsync();
            // To store detailed information for each seat booking offer      
            var modelList = new List<BookingDetailsModel>();            
            // Get User/LiftOffer/SeatBooking details for each seat booking offer to display in view
            foreach (SeatBooking sb in seatBookings)
            {
                int liftOfferID = sb.LiftOfferID; 
                string seekerID = sb.UserID;
                LiftOffer offer = await db.LiftOffers.FindAsync(liftOfferID);
                // Get lift seeker ID to display details in view for lift offerer
                User user = db.Users.Find(seekerID);
                BookingDetailsModel model = new BookingDetailsModel();
                model.User = user;
                model.LiftOffer = offer;
                model.SeatBooking = sb;
                modelList.Add(model);      
            }

            return View(modelList);                             

            //var result = seatBookings.Join(liftOffers,
            //                sb => sb.LiftOfferID, lo => lo.LiftOfferID, (sb, lo) => new
            //                {
            //                    sb.SeatBookingID,
            //                    sb.SeatsRequest,
            //                    sb.UserID,
            //                    sb.IsAccepted,
            //                    lo.SeatsAvailable,
            //                    lo.StartPointName,
            //                    lo.EndPointName,
            //                    lo.DepartureHour,
            //                    lo.DepartureMin,
            //                    lo.ArrivalHour,
            //                    lo.ArrivalMin
            //                });

            //return View(result);
            //return View("Received", "SeatBookings");
        }

        // GET: SeatBookings/Accept/5   - Accept received seat booking offer
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

        // GET: SeatBookings/Decline/5   - Decline received seat booking offer
        //public async Task<ActionResult> Decline(int? id)                // Passed SeatBookingID
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    // Get seat booking offer
        //    SeatBooking seatBooking = await db.SeatBookings.FindAsync(id);
        //    // Get lift offer for which the seat booking offer has been created
        //    LiftOffer liftOffer = await db.LiftOffers.FindAsync(seatBooking.LiftOfferID);
        //    if ((liftOffer == null) || (seatBooking == null))
        //    {
        //        return HttpNotFound();
        //    }
        //    seatBooking.SeatsRequest = 0;
        //    db.Entry(liftOffer).State = EntityState.Modified;
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Received");
        //}

        // GET: SeatBookings/Delete/5   - Delete seat booking offer
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
            // model, to store detailed information for the booking offer to display in view
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

        // GET: SeatBookings/MyBookings     - Display all user seat booking requests
        public async Task<ActionResult> MyBookings()
        {
            // Get ID of currently logged-in user
            var id = User.Identity.GetUserId();
            // Only user seat booking requests
            var seatBookings = await db.SeatBookings.Where(s => s.UserID == id).ToListAsync();
            // modelList, to store detailed information for each seat booking offer 
            var modelList = new List<BookingDetailsModel>();
            // Get User/LiftOffer/SeatBooking details for each seat booking offer to display in view
            foreach (SeatBooking sb in seatBookings)
            {
                int liftOfferID = sb.LiftOfferID;
                string offererID = sb.OffererID;
                LiftOffer offer = await db.LiftOffers.FindAsync(liftOfferID);
                // Get lift offerer ID to display details in view for lift seeker
                User user = db.Users.Find(offererID);
                BookingDetailsModel model = new BookingDetailsModel();
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