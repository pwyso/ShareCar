using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        // GET: Feedbacks/AllAdmin
        public async Task<ActionResult> AllAdmin()
        {
            var feedbacks = db.Feedbacks.OrderByDescending(o => o.FeedbackID);
            return View(await feedbacks.ToListAsync());
        }

        // GET: Feedbacks/FeedbacksReceived
        public async Task<ActionResult> FeedbacksReceived()
        {
            var id = User.Identity.GetUserId();
            var feedbacks = db.Feedbacks.OrderByDescending(o => o.FeedbackID).Where(u => u.UserID == id);
            return View(await feedbacks.ToListAsync());
        }

        // GET: Feedbacks/ReportFeedback/5
        
        public async Task<ActionResult> ReportFeedback(int? id)
        {
            var feedbackToReport = await db.Feedbacks.FindAsync(id);
            feedbackToReport.IsReported = true;
            if (ModelState.IsValid)
            {
                db.Entry(feedbackToReport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return View("ConfirmRepFeedback");
            }
            return RedirectToAction("FeedbacksReceived", "Feedbacks");
        }

        // GET: Feedbacks/FeedbacksReported
        public async Task<ActionResult> FeedbacksReported()
        {
            
            var feedbacks = db.Feedbacks.OrderByDescending(o => o.FeedbackID).Where(u => u.IsReported == true);
            return View(await feedbacks.ToListAsync());
        }

        // GET: Feedbacks/Details/5
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

        // GET: Feedbacks/Create
        public ActionResult Create()
        {
            // gets Id of the current logged-in user
            var currentUser = User.Identity.GetUserId();

            // creates list of all users and removes current logged-in user (cannot leave feedback for themself)
            SelectList list = new SelectList(db.Users.Where(u => u.Id != currentUser).ToList(), "Id", "Name");
            ViewBag.UserID = list;
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FeedbackID,Description,RatingValue,UserID")] Feedback feedback)
        {
            // gets ID of logged-in user to find theirs name, then saves name in Feedbacks table in DB
            var writtenByUser = User.Identity.GetUserId();
            var usr = db.Users.Find(writtenByUser);
            feedback.LeftBy = usr.Name;

            if (ModelState.IsValid)
            {
                db.Feedbacks.Add(feedback);
                await db.SaveChangesAsync();

                // gets just rated user to calculate Rating and update it in User profile/table
                User user = await db.Users.FirstAsync(r => r.Id == feedback.UserID);

                // counts ratings of the rated user
                int noOfRatings = await db.Feedbacks.Where(u => u.UserID == feedback.UserID).CountAsync();
                double newRatingAvg;

                if (noOfRatings != 0)
                {
                    // sum up all ratings of rated user and calculates new Rating
                    int ratingsTotal = await db.Feedbacks.Where(u => u.UserID == feedback.UserID).Select(u => u.RatingValue).SumAsync();
                    newRatingAvg = (double)ratingsTotal / (noOfRatings);
                }
                else
                {
                    newRatingAvg = feedback.RatingValue;
                }
                user.RatingAvg = Math.Round(newRatingAvg, 2);

                // updates Rating for rated user
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("All");
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "Name", feedback.UserID);
            return View(feedback);
        }

        // GET: Feedbacks/Edit/5
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FeedbackID,Description,RatingValue,UserID,LeftBy,IsReported,")] Feedback feedback)
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

        // GET: Feedbacks/Delete/5
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

            // stores ID of rated user to update Rating; ID will be null when feedback deleted
            TempData["UserID"] = feedback.UserID;

            db.Feedbacks.Remove(feedback);
            await db.SaveChangesAsync();

            // retrieves ID of user from deleted feedback
            string uid = (string)TempData["UserID"];

            // gets user to calculate Rating and update it in User profile/table
            User user = db.Users.Find(uid);

            // counts ratings for user after deletion of feedback
            int noOfRatings = await db.Feedbacks.Where(u => u.UserID == uid).CountAsync();
            if (noOfRatings != 0)
            {
                // sum up all ratings of the user to calculate new Rating
                int ratingsTotal = await db.Feedbacks.Where(u => u.UserID == uid).Select(u => u.RatingValue).SumAsync();
                double newRatingAvg = (double)ratingsTotal / (noOfRatings);
                user.RatingAvg = Math.Round(newRatingAvg, 2);
            }
            else
            {
                user.RatingAvg = null;
            }

            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction("FeedbacksReported");
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