using System.ComponentModel.DataAnnotations;

namespace ShareCar.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [Required(AllowEmptyStrings = false)]   // Listed below characters not allowed ( "\\x5C" HEX backslash )    
        [RegularExpression("[^_+=()*&^%$£}{#;:'`~\\x5C></|\\@]+", ErrorMessage = "Feedback: provided invalid character.")]
        [Display(Name = "Feedback"), StringLength(90, ErrorMessage = "Feedback: max. 90 characters allowed.")]
        public string Description { get; set; }

        [Required]
        [RegularExpression("[0-9]{1}", ErrorMessage = "Rating: only numbers allowed.")]
        [Display(Name = "Rating"), Range(1, 5, ErrorMessage = "Rating: only 1,2,3,4 or 5 allowed.")]
        public int RatingValue { get; set; }

        [RegularExpression("[a-zA-Z0-9]+", ErrorMessage = "Left by: only numbers and letters allowed.")]
        [Display(Name = "Left by"), StringLength(30, ErrorMessage = "Left by: max. 30 characters allowed.")]
        public string LeftBy { get; set; }

        // User ID as leaving feedback
        public string LeftByID { get; set; }

        public bool IsReported { get; set; }

        // Not as FK - not affected by SeatBooking deletion
        public int SeatBookingID { get; set; }

        // FK of User table
        public string UserID { get; set; }

        // Relationship between tables - Feedbacks:2 <=> 1:User
        public virtual User User { get; set; }
    }
}