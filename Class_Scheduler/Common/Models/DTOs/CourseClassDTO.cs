using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Class_Scheduler.Common.Models.DTOs
{
    [XmlType(TypeName = "CourseClass")]
    public class CourseClassDTO
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string ClassCode { get; set; }

        [XmlAttribute]
        public string ClassName { get; set; }

        [XmlAttribute]
        public int CountPerWeek { get; set; }

        [XmlAttribute]
        public int StudentCount { get; set; }

        [XmlElement]

        public TeacherDTO Teacher { get; set; }

        [XmlArray]

        public List<StudentDTO> Students { get; set; }
       
    }
}
