using Class_Scheduler.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Class_Scheduler.Common.Models
{
    public class SavableObject
    {
        [XmlElement("LastSavedTime")]
        public string DateTimeString
        {
            get { return DateTime.ToString(); }
            set { DateTime = DateTime.Parse(value); }
        }
        
        [XmlArray("Students")]
        public List<StudentDTO> Students { get; set; }

        [XmlArray("Teachers")]
        public List<TeacherDTO> Teachers { get; set; }

        [XmlArray("Subjects")]
        public List<SubjectDTO> Subjects { get; set; }

        [XmlArray("Rooms")]
        public List<RoomDTO> Rooms { get; set; }

        [XmlArray("CourseClasses")]
        public List<CourseClassDTO> CourseClasses { get; set; }

        [XmlElement("Schedule")]
        public ScheduleDTO Schedule { get; set; }

        [XmlArray("ScheduleClasses")]
        public List<ScheduledClassDTO> ScheduleClasses { get; set; }

        [XmlIgnore]
        public DateTime DateTime { get; set; }

        public SavableObject()
        {
            Students = new List<StudentDTO>();
            Teachers = new List<TeacherDTO>();
            Rooms = new List<RoomDTO>();
            Subjects = new List<SubjectDTO>();
            CourseClasses = new List<CourseClassDTO>();
        }
    }
}