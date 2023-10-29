using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Class_Scheduler.Common.Models.DTOs
{
    [XmlType(TypeName = "ScheduledClass")]
    public class ScheduledClassDTO
    {
        [XmlElement("CourseClass")]
        public CourseClassDTO Course { get; set; }

        [XmlAttribute("RoomId")]
        public int RoomId { get; set; }

        [XmlAttribute("Weekday")]
        public int WeekDay { get; set; }

        [XmlAttribute("SlotNum")]
        public int SlotId { get; set; }
    }
}
