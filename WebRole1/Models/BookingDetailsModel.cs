using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareCar.Models
{
    // Model created for displaying seat booking details (received offers, posted offers)
    public class BookingDetailsModel
    {
        public User User { get; set; }
        public LiftOffer LiftOffer { get; set; }
        public SeatBooking SeatBooking { get; set; }      
    }
   
}
