using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Class_Scheduler.Common.Models.DTOs
{
    [XmlType(TypeName = "Schedule")]
    public class ScheduleDTO
    {
        [XmlElement("Slots")]
        public List<SlotDTO> Slots { get; set; }
    }
}
