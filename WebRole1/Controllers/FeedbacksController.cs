using System;
using System.Linq;
using System.Web.Mvc;
using ShareCar.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Data.Entity;

namespace ShareCar.Controllers
{
    [Authorize]
    public class FeedbacksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Feedbacks/All
        [AllowAnonymous]
        public ActionResult All()
        {
            var feedbacks = db.Feedbacks;
            return View(feedbacks);
        }

        // GET: Feedbacks/AllAdmin      - Display all feedbacks (for admin view)
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AllAdmin()
        {
            var feedbacks = db.Feedbacks.OrderByDescending(o => o.FeedbackID);
            return View(await feedbacks.ToListAsync());
        }

        // GET: Feedbacks/Received      - Display all received feedbacks (for user view)
        public async Task<ActionResult> Received()
        {
            var id = User.Identity.GetUserId();
            var feedbacks = db.Feedbacks.OrderByDescending(o => o.FeedbackID).Where(u => u.UserID == id);
            return View(await feedbacks.ToListAsync());
        }

        // GET: Feedbacks/ReportFeedback/5      - Mark feedback as reported by user (found in admin view)
        public async Task<ActionResult> ReportFeedback(int? id)
        {
            var feedbackToReport = await db.Feedbacks.FindAsync(id);
            feedbackToReport.IsReported = true;
            if (ModelState.IsValid)
            {
                db.Entry(feedbackToReport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return View("ConfirmReport");
            }
            return RedirectToAction("Received", "Feedbacks");
        }

        // GET: Feedbacks/Reported      - Display reported feedbacks (for admin)
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Reported()
        {        
            var feedbacks = db.Feedbacks.OrderByDescending(o => o.FeedbackID).Where(u => u.IsReported == true);
            return View(await feedbacks.ToListAsync());
        }

        // GET: Feedbacks/Details/5     - Dislplay feedback details
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // GET: Feedbacks/Create        - Create new feedback
        public ActionResult Create()
        {
            // Details for create feedback, 
            // got from "~/SeatBookings/Received" or "~/SeatBookings/MyBookings"
            var model = (BookingDetailsModel)TempData["model"];
            ViewBag.User = model.User;
            Feedback feedback = new Feedback();
            feedback.UserID = model.User.Id;
            feedback.SeatBookingID = model.SeatBooking.SeatBookingID;
            return View(feedback);
        }

        // POST: Feedbacks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = 
            "FeedbackID,Description,RatingValue,IsReported,UserID,SeatBookingID")] Feedback feedback)
        {
            // Get ID of logged-in user to find their name, then saves name in Feedbacks table in DB
            var writtenByID = User.Identity.GetUserId();
            var usr = db.Users.Find(writtenByID);
            feedback.LeftBy = usr.Name;
            feedback.LeftByID = writtenByID;
            if (ModelState.IsValid)
            {
                db.Feedbacks.Add(feedback);
                await db.SaveChangesAsync();
                // Get just rated user to calculate Rating and update it in User profile/table
                User user = await db.Users.FirstAsync(r => r.Id == feedback.UserID);
                // Count ratings of the rated user
                int noOfRatings = await db.Feedbacks.Where(u => u.UserID == feedback.UserID).CountAsync();
                double newRatingAvg;
                if (noOfRatings != 0)
                {
                    // Sum up all ratings of rated user and calculates new Rating
                    int ratingsTotal = await db.Feedbacks.Where(u => u.UserID == feedback.UserID).Select(u => u.RatingValue).SumAsync();
                    newRatingAvg = (double)ratingsTotal / (noOfRatings);
                }
                else
                {
                    newRatingAvg = feedback.RatingValue;
                }
                user.RatingAvg = Math.Round(newRatingAvg, 2);
                // Updates Rating for rated user
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("All");
            }
            return View(feedback);
        }

        // GET: Feedbacks/Edit/5        - Edit feedback
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "Name", feedback.UserID);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = 
            "FeedbackID,Description,RatingValue,UserID,LeftBy,IsReported,")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedback).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("All");
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "Name", feedback.UserID);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5      - Delete feedback
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Feedback feedback = await db.Feedbacks.FindAsync(id);
            // Stores ID of rated user to update Rating; ID will be null when feedback deleted
            TempData["UserID"] = feedback.UserID;
            db.Feedbacks.Remove(feedback);
            await db.SaveChangesAsync();
            // Retrieve ID of user, before feedback deleted
            string uid = (string)TempData["UserID"];
            // Get user to calculate Rating and update it in User profile/table
            User user = db.Users.Find(uid);
            // Count ratings for user after deletion of feedback
            int noOfRatings = await db.Feedbacks.Where(u => u.UserID == uid).CountAsync();
            if (noOfRatings != 0)
            {
                // Sum up all ratings of the user to calculate new Rating
                int ratingsTotal = await db.Feedbacks.Where(
                    u => u.UserID == uid).Select(u => u.RatingValue).SumAsync();
                double newRatingAvg = (double)ratingsTotal / (noOfRatings);
                user.RatingAvg = Math.Round(newRatingAvg, 2);
            }
            else
            {
                user.RatingAvg = null;
            }
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Reported");
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