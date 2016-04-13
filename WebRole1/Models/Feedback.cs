using System.ComponentModel.DataAnnotations;

namespace ShareCar.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Feedback"), StringLength(90, ErrorMessage = "Feedback: max. 90 characters allowed.")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Rating"), Range(1, 5, ErrorMessage = "Rating: only 1,2,3,4 or 5 allowed.")]
        public int RatingValue { get; set; }

        [Display(Name = "Left by")]
        public string LeftBy { get; set; }

        // User ID as leaving feedback
        public string LeftByID { get; set; }

        public bool IsReported { get; set; }

        // Not as FK - not afected by SeatBooking deletion
        public int SeatBookingID { get; set; }

        // FK of User table
        public string UserID { get; set; }

        // Relationship between tables - Feedbacks:2 <=> 1:User
        public virtual User User { get; set; }
    }
}