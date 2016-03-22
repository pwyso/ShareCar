using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareCar.Models
{
    public class Day
    {
        public int DayID { get; set; }
        public string DayName { get; set; }
        public bool Selected { get; set; }

        // FK of LiftOffer table
        public int LiftOfferID { get; set; }
        public virtual LiftOffer LiftOffer { get; set; }
    }
}
