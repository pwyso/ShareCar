using System;
using System.ComponentModel.DataAnnotations;

namespace ShareCar.Models
{
    public class SeatBooking
    {
        [Key]
        public int SeatBookingID { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.Date)]     // Set format to date only (no hh:mm:ss) for display mode
        public DateTime CreateTime { get { return DateTime.Now; } set { } }

        [Required]
        [Display(Name = "Seats Req."), Range(0, 10, ErrorMessage = "Seats Req: max no. of seats is 10.")]
        public int SeatsRequest { get; set; }

        public string OffererID { get; set; }

        [Display(Name = "Status")]
        public bool IsAccepted { get; set; } 

        // FK of User table
        public string UserID { get; set; }

        // Relationship between tables - SeatBookings:M <=> 1:User
        public virtual User User { get; set; }

        // FK of LiftOffers table
        public int LiftOfferID { get; set; }

        // Relationship between tables - SeatBookings:M <=> 1:LiftOffers
        public virtual LiftOffer LiftOffer { get; set; }
    }
}
