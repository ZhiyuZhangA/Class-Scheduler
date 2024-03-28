using Class_Scheduler.Common.Models.ClassScheduling;
using System.Collections.ObjectModel;

namespace Class_Scheduler.Common.Models
{
    public class Student : UserBase
    {
		private string gradeAndClass;

		public string GradeAndClass
		{
			get { return gradeAndClass; }
			set { gradeAndClass = value; RaisePropertyChanged(); }
		}

		public string sex;

		public string Sex
		{
			get { return sex; }
			set { sex = value; RaisePropertyChanged(); }
		}

		private string emailAddress;

		public string EmailAddress
		{
			get { return emailAddress; }
			set { emailAddress = value; RaisePropertyChanged(); }	
		}

		private ObservableCollection<int> subjectScores;

		public ObservableCollection<int> SubjectScores
		{
			get 
			{ 
				if (subjectScores == null)
					subjectScores = new ObservableCollection<int>();
				return subjectScores; 
			}
			set { subjectScores = value; RaisePropertyChanged(); }
		}

		private ObservableCollection<string> subjectsSelected;

		public ObservableCollection<string> SubjectSelected
		{
			get 
			{
                if (subjectsSelected == null)
                    subjectsSelected = new ObservableCollection<string>();
                return subjectsSelected; 
			}
			set { subjectsSelected = value; RaisePropertyChanged(); }
		}

		private Schedule schedule;

		public Schedule StudentSchedule
		{
			get { return schedule; }
			set { schedule = value; RaisePropertyChanged(); }
		}

		private bool isSelected;

		public bool IsSelected
		{
			get { return isSelected; }
			set { isSelected = value; RaisePropertyChanged(); }
		}

		public Student Copy()
		{
			Student student = new Student();
			student.Id = Id;
			student.Name = Name;
			student.EmailAddress = EmailAddress;
			student.SubjectSelected = new ObservableCollection<string>();
			student.SubjectSelected.AddRange(SubjectSelected);
			student.Sex = Sex;
			student.GradeAndClass = GradeAndClass;
			student.SubjectScores = new ObservableCollection<int>();
			student.SubjectScores.AddRange(SubjectScores);
			schedule = new Schedule();
            return student;
		}
    }
}
