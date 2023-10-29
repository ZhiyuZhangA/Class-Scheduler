using Class_Scheduler.Common.Models.ClassScheduling;
using System.Collections.Generic;

namespace Class_Scheduler.Common.Models
{
	/// <summary>
	/// 添加老师不同时间段的数据
	/// </summary>
    public class Teacher : UserBase
    {
		private string emailAddress;

		public string EmailAddress
		{
			get { return emailAddress; }
			set { emailAddress = value; RaisePropertyChanged(); }
		}

		private string nickName;
		
		public string NickName
		{
			get { return nickName; }
			set { nickName = value; RaisePropertyChanged(); }
		}

		private List<string> subjectList = new List<string>();

		public List<string> SubjectList
		{
			get { return subjectList; }
			set { subjectList = value; }
		}

		private string subjects;
		public string Subjects
		{
			get { return subjects; }
			set { subjects = value; RaisePropertyChanged(); }
		}

		private string naTimeSlotString;

		public string UnavailableTimeSlotString
		{
			get { return naTimeSlotString; }
			set { naTimeSlotString = value; RaisePropertyChanged(); }
		}

		private List<int> naTimeSlotId;

		public List<int> UnavailableTimeSlotId
		{
            get { return naTimeSlotId; }
            set { naTimeSlotId = value; RaisePropertyChanged(); }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged(); }
        }


		private Schedule teacherSchedule;

		public Schedule TeacherSchedule
		{
			get { return teacherSchedule; }
			set { teacherSchedule = value; RaisePropertyChanged(); }
		}

		public Teacher()
		{
			UnavailableTimeSlotId = new List<int>();
			UnavailableTimeSlotString = "Null";
            TeacherSchedule = new Schedule();
        }
    }
}
