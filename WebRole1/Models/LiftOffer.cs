using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using ShareCar.DateValidation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareCar.Models
{
    public class LiftOffer
    {
        [Key]
        public int LiftOfferID { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.Date)]       // Set format to date only (no hh:mm:ss) for display mode
        public DateTime CreateTime { get { return DateTime.Now; } set { } }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "From"), StringLength(50, ErrorMessage = "From: max. 50 characters allowed.")]
        public string StartPointName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "To"), StringLength(50, ErrorMessage = "To: max. 50 characters allowed.")]
        public string EndPointName { get; set; }

        [Required]
        [Display(Name = "Valid From")]
        [DataType(DataType.Date, ErrorMessage = "Valid From: invalid date.")]
        public DateTime StartDate { get; set; }

        // Could be null (no expiry date)
        [Display(Name = "Valid To")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Dep. Time")]
        public string DepartureHour { get; set; }

        [Required]
        [Display(Name = "Dep. Time")]
        public string DepartureMin { get; set; }

        [Required]
        [Display(Name = "Arr. Time")]
        public string ArrivalHour { get; set; }

        [Required]
        [Display(Name = "Arr. Time")]
        public string ArrivalMin { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Car Make"), StringLength(20, ErrorMessage = "Car Make: max. 20 characters allowed.")]
        public string CarMake { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Car Model"), StringLength(20, ErrorMessage = "Car Model: max. 20 characters allowed.")]
        public string CarModel { get; set; }

        [Required]
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
