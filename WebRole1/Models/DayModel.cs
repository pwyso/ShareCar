using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShareCar.Models
{
    public class DayModel
    {
        public DayModel()
        {
            this.Days = new List<Day>();
        }

        [NotMapped]
        public List<Day> Days { get; set; }
    }

    // Days keept in memory instead of keeping in DB
    public static class DayRepository
    {
        // Get selected day
        public static Day Get(int id)
        {
            return GetAll().FirstOrDefault(x => x.DayID.Equals(id));
        }
        // Get all days
        public static IEnumerable<Day> GetAll()
        {
            return new List<Day>
            {
                new Day {  DayID = 1, DayName = "Monday"},
                new Day {  DayID = 2, DayName = "Tuesday"},
                new Day {  DayID = 3, DayName = "Wednesday"},
                new Day {  DayID = 4, DayName = "Thursday"},
                new Day {  DayID = 5, DayName = "Friday"},
                new Day {  DayID = 6, DayName = "Saturday"},
                new Day {  DayID = 7, DayName = "Sunday"}
            };
        }
    }
}
