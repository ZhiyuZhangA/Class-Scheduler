using Class_Scheduler.Common.Models.ClassScheduling;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Class_Scheduler.Common.Models.DTOs
{
    [XmlType(TypeName = "Teacher")]
    public class TeacherDTO
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string NickName { get; set; }

        [XmlAttribute]
        public string EmailAddress { get; set; }

        [XmlAttribute]
        public string UnavailableTimeSlotString { get; set; }

        [XmlArray]
        public List<string> Subject { get; set; }

        [XmlArray]
        public List<int> UnavailableTimeSlotId { get; set; }
    }
}
