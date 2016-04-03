using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareCar.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [Required]
        [Display(Name = "Feedback"), StringLength(90, ErrorMessage = "Max. 90 characters allowed.")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Rating"), Range(1, 5, ErrorMessage = "Only 1,2,3,4 or 5 allowed.")]
        public int RatingValue { get; set; }

        [Display(Name = "Left by")]
        public string LeftBy { get; set; }

        public bool IsReported { get; set; }

        // FK of User Table
        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual User User { get; set; }
    }
}