using Class_Scheduler.Common.Models.ClassScheduling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Class_Scheduler.Common.Models.DTOs
{
    [XmlType(TypeName = "Slot")]
    public class SlotDTO
    {
        [XmlArray("ScheduledClasses")]
        public List<ScheduledClassDTO> ScheduledClasses { get; set; }

        [XmlAttribute("SlotId")]
        public int SlotId { get; set; }
    }
}
