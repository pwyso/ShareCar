using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareCar.Models
{
    public class LiftOffer
    {
        [Key]
        public int LiftOfferID { get; set; }

        [Required]
        [Display(Name = "From"), StringLength(50, ErrorMessage = "Max. 50 characters allowed.")]
        public string StartPointName { get; set; }

        [Required]
        [Display(Name = "To"), StringLength(50, ErrorMessage = "Max. 50 characters allowed.")]
        public string EndPointName { get; set; }

        [Required]
        [Display(Name = "Valid From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Valid To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Depart. Time")]
        public int DepartureHour { get; set; }

        [Required]
        public int DepartureMin { get; set; }

        [Required]
        [Display(Name = "Arriv. Time")]
        public int ArrivalHour { get; set; }

        [Required]
        public int ArrivalMin { get; set; }

        [Required]
        [Display(Name = "Car Make"), StringLength(20, ErrorMessage = "Max. 20 characters allowed.")]
        public string CarMake { get; set; }

        [Required]
        [Display(Name = "Car Model"), StringLength(20, ErrorMessage = "Max. 20 characters allowed.")]
        public string CarModel { get; set; }

        [Required]
        [Display(Name = "Seats Avail."), Range(1,20, ErrorMessage = "Max no. of seats is 20.")]
        public int SeatsAvailable { get; set; }

        // FK of User Table
        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual User User { get; set; }
    }
}
