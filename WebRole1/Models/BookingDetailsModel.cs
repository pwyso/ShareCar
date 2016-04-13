namespace ShareCar.Models
{
    // Model created for displaying seat booking details (received offers view, posted offers view)
    // also to help create feedback for selected seat booking offer
    public class BookingDetailsModel
    {
        public User User { get; set; }
        public LiftOffer LiftOffer { get; set; }
        public SeatBooking SeatBooking { get; set; }  
        public Feedback Feedback { get; set; }        
    }  
}
