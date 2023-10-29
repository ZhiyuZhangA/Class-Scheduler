using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.Common.Models.ClassScheduling
{
    public class UserRestriction
    {
        /// <summary>
        /// Unavailable Time Slot for the entire grade
        /// </summary>
        public List<int> GlobalNaSlots = new List<int>();

        /// <summary>
        /// Maximum number of courses that a teacher can have (To decrease the burden)
        /// </summary>
        public int MaxCoursePerDay;

        public UserRestriction(List<int> globalNaSlot, int maxCoursePerDay) 
        {
            GlobalNaSlots = globalNaSlot;
            MaxCoursePerDay = maxCoursePerDay;
        }
    }
}
