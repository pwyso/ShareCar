using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareCar.Models
{
    public class LiftOffer
    {
        [Key]
        public int LiftOfferID { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.Date)]       // Set format to date only (no hh:mm:ss) for display mode
        public DateTime CreateTime { get { return DateTime.Now; } set { } }

        [Required(AllowEmptyStrings = false)]   // Listed below characters not allowed ( "\\x5C" HEX backslash )
        [RegularExpression("[^_+=()*&^%$£!?\"\\(\\)\\.\\,}{#;:'`~\\x5C></|\\@]+", ErrorMessage = "Feedback: provided invalid character.")]
        [Display(Name = "From"), StringLength(50, ErrorMessage = "From: max. 50 characters allowed.")]
        public string StartPointName { get; set; }

        [Required(AllowEmptyStrings = false)]   // Listed below characters not allowed ( "\\x5C" HEX backslash )
        [RegularExpression("[^_+=()*&^%$£!?\"\\(\\)\\.\\,}{#;:'`~\\x5C></|\\@]+", ErrorMessage = "To: provided invalid character.")]
        [Display(Name = "To"), StringLength(50, ErrorMessage = "To: max. 50 characters allowed.")]
        public string EndPointName { get; set; }

        [Required]
        [Display(Name = "Valid From")]
        [DataType(DataType.Date, ErrorMessage = "Valid From: invalid date.")]
        public DateTime StartDate { get; set; }

        // Could be null (no expiry date)
        [Display(Name = "Valid To")]
        [DataType(DataType.Date, ErrorMessage = "Valid To: invalid date.")]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Dep. Time")]
        [RegularExpression("[0-9]{2}", ErrorMessage = "Dep. Time: only numbers allowed.")]
        public string DepartureHour { get; set; }

        [Required]
        [Display(Name = "Dep. Time")]
        [RegularExpression("[0-9]{2}", ErrorMessage = "Dep. Time: only numbers allowed.")]
        public string DepartureMin { get; set; }

        [Required]
        [Display(Name = "Arr. Time")]
        [RegularExpression("[0-9]{2}", ErrorMessage = "Arr. Time: only numbers allowed.")]
        public string ArrivalHour { get; set; }

        [Required]
        [Display(Name = "Arr. Time")]
        [RegularExpression("[0-9]{2}", ErrorMessage = "Arr. Time: only numbers allowed.")]
        public string ArrivalMin { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression("[a-zA-Z0-9. ]+", ErrorMessage = "Car Make: only letters and numbers allowed.")]
        [Display(Name = "Car Make"), StringLength(20, ErrorMessage = "Car Make: max. 20 characters allowed.")]
        public string CarMake { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression("[a-zA-Z0-9. ]+", ErrorMessage = "Car Model: only letters and numbers allowed.")]
        [Display(Name = "Car Model"), StringLength(20, ErrorMessage = "Car Model: max. 20 characters allowed.")]
        public string CarModel { get; set; }

        [Required]
        [RegularExpression("[0-9]{1,2}", ErrorMessage = "Seats Avail.: only numbers allowed.")]
        [Display(Name = "Seats Avail."), Range(1,10, ErrorMessage = "Seats Avail: max no. of seats is 10.")]
        public int SeatsAvailable { get; set; }
     
        // FK of User table
        public string UserID { get; set; }

        // Relationship between tables - LiftOffers:M <=> 1:User
        public virtual User User { get; set; }

        // Relationship between tables - LiftOffers:1 <=> M:SeatBookings 
        public virtual ICollection<SeatBooking> SeatBooking { get; set; }

        // NotMapped - excluded from creating entities and relationships in tables
        // because DayRepository (static class) is used with list of days
        [NotMapped]
        public List<Day> Days { get; set; }

        [NotMapped]
        public virtual DayModel DayModel { get; set; }
    }
}
