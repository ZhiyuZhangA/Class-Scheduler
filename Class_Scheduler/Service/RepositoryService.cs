using AutoMapper;
using Class_Scheduler.Common.Models;
using Class_Scheduler.Common.Models.ClassScheduling;
using Class_Scheduler.Common.Models.DTOs;
using Class_Scheduler.Extensions;
using Microsoft.Win32;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Class_Scheduler.Service
{
    public class RepositoryService : BindableBase, IRepositoryService
    {
        private readonly IMapper mapper;

        private ObservableCollection<Subject> subjects;
        public ObservableCollection<Subject> Subjects
        {
            get
            {
                if (subjects == null)
                    subjects = new ObservableCollection<Subject>();
                return subjects;
            }
            set
            {
                subjects = value; RaisePropertyChanged();
            }
        }

        private ObservableCollection<Student> students;
        public ObservableCollection<Student> Students
        {
            get
            {
                if (students == null)
                    students = new ObservableCollection<Student>();
                return students;
            }
            set { students = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<Teacher> teachers;
        public ObservableCollection<Teacher> Teachers
        {
            get
            {
                if (teachers == null)
                    teachers = new ObservableCollection<Teacher>();
                return teachers;
            }
            set { teachers = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<Room> rooms;
        public ObservableCollection<Room> Rooms
        {
            get
            {
                if (rooms == null)
                    rooms = new ObservableCollection<Room>();
                return rooms;
            }
            set { rooms = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<CourseClass> courseClasses;

        public ObservableCollection<CourseClass> CourseClasses
        {
            get
            {
                if (courseClasses == null)
                    courseClasses = new ObservableCollection<CourseClass>();
                return courseClasses;
            }
            set { courseClasses = value; RaisePropertyChanged(); }
        }

        private List<string> subjectAvailable;

        public List<string> SubjectAvailable
        {
            get
            {
                return subjectAvailable;
            }
            set
            {
                subjectAvailable = value; RaisePropertyChanged();
            }
        }

        private UserSetting userSetting;
        public UserSetting UserSetting
        {
            get { return userSetting; }
            set { userSetting = value; RaisePropertyChanged(); }
        }

        private UserSetting defaultUserSetting;

        public UserSetting DefaultUserSetting
        {
            get { return defaultUserSetting; }
            set { defaultUserSetting = value; }
        }

        private Schedule schedule;

        public Schedule Schedule
        {
            get { return schedule; }
            set { schedule = value; RaisePropertyChanged(); }
        }

        private int studentsCount;
        public int StudentsCount
        {
            get { return studentsCount; }
            set { studentsCount = value; RaisePropertyChanged(); }
        }

        private int lessonsCount;

        public int SubjectsCount
        {
            get { return lessonsCount; }
            set { lessonsCount = value; RaisePropertyChanged(); }
        }

        private int roomsCount;

        public int RoomsCount
        {
            get { return roomsCount; }
            set { roomsCount = value; RaisePropertyChanged(); }
        }

        private int teachersCount;

        public int TeachersCount
        {
            get { return teachersCount; }
            set { teachersCount = value; RaisePropertyChanged(); }
        }

        private int courseClassCount;

        public int CourseClassCount
        {
            get { return courseClassCount; }
            set { courseClassCount = value; RaisePropertyChanged(); }
        }

        private CancellationToken cancellationToken;

        public CancellationToken CancellationToken
        {
            get { return cancellationToken; }
            set { cancellationToken = value; }
        }

        private List<ScheduledClass> scheduledClassesSaved;

        public RepositoryService(IMapper mapper)
        {
            subjectAvailable = new List<string>();
            scheduledClassesSaved = new List<ScheduledClass>();
            UserSetting = new UserSetting();
            Schedule = new Schedule();
            this.mapper = mapper;

            defaultUserSetting = new UserSetting();
            defaultUserSetting.AutoSaveWhenClose = true;
            defaultUserSetting.PopulationSize = 64;
            defaultUserSetting.ClassDivisionBaseline = 14;
            defaultUserSetting.MutationProb = 0.95;
            defaultUserSetting.User = new User();
            defaultUserSetting.User.UserName = string.Empty;
            defaultUserSetting.User.Password = string.Empty;
            defaultUserSetting.User.SchoolName = string.Empty;
            defaultUserSetting.User.ImageSource = "pack://application:,,,/Images/DefaultUserPhoto.png";
            defaultUserSetting.User.BitmapImage = new BitmapImage();
            defaultUserSetting.User.BitmapImage.BeginInit();
            defaultUserSetting.User.BitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultUserSetting.User.BitmapImage.UriSource = new Uri(defaultUserSetting.User.ImageSource);
            defaultUserSetting.User.BitmapImage.EndInit();

            LoadUserSetting();
            InitSchedules();
            
            InitializeObjCount();
        }

        private void InitializeObjCount()
        {
            StudentsCount = Students.Count;
            TeachersCount = Teachers.Count;
            RoomsCount = Rooms.Count;
            SubjectsCount = Subjects.Count;
            CourseClassCount = CourseClasses.Count;
        }

        public void SaveData(bool saveTo=false)
        {
            // If user haven't chose a saving file directory, then ask it to choose
            if (PrismManager.SavedPath == null || PrismManager.SavedPath == string.Empty || saveTo)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Xml File|*.xml";
                if (saveFileDialog.ShowDialog() == true)
                {
                    PrismManager.SavedPath = saveFileDialog.FileName;
                }
            }

            SavableObject savableObject = new SavableObject();
            savableObject.Teachers = mapper.Map<List<TeacherDTO>>(Teachers.ToList());
            savableObject.Students = mapper.Map<List<StudentDTO>>(Students.ToList());
            savableObject.Rooms = mapper.Map<List<RoomDTO>>(Rooms.ToList());
            savableObject.Subjects = mapper.Map<List<SubjectDTO>>(Subjects.ToList());
            savableObject.CourseClasses = mapper.Map<List<CourseClassDTO>>(CourseClasses.ToList());
            savableObject.Schedule = mapper.Map<ScheduleDTO>(schedule);
            savableObject.ScheduleClasses = mapper.Map<List<ScheduledClassDTO>>(scheduledClassesSaved);
            savableObject.DateTime = DateTime.Now;

            XmlSerializeManager.XmlSerialize<SavableObject>(PrismManager.SavedPath, savableObject);
        }

        public void LoadData(SavableObject savableObject)
        {
            if (savableObject == null)
                return;

            Teachers.AddRange(mapper.Map<List<Teacher>>(savableObject.Teachers));
            Students.AddRange(mapper.Map<List<Student>>(savableObject.Students));
            Rooms.AddRange(mapper.Map<List<Room>>(savableObject.Rooms));
            Subjects.AddRange(mapper.Map<List<Subject>>(savableObject.Subjects));
            CourseClasses.AddRange(mapper.Map<List<CourseClass>>(savableObject.CourseClasses));


            if (savableObject.Schedule != null)
                Schedule = mapper.Map<Schedule>(savableObject.Schedule);

            foreach (var subject in Subjects)
                subjectAvailable.Add(subject.Name);

            if (savableObject.ScheduleClasses != null)
                scheduledClassesSaved = mapper.Map<List<ScheduledClass>>(savableObject.ScheduleClasses);

            // Init different data
            AddStudentSchedule(scheduledClassesSaved);
            AddTeacherSchedule(scheduledClassesSaved);
            InitializeObjCount();
        }

        public void LoadData()
        {
            SavableObject savableObject = XmlSerializeManager.XmlDeserializer<SavableObject>(PrismManager.SavedPath);
            LoadData(savableObject);
        }

        public void GenerateCourseClasses()
        {
            CourseClasses.Clear();
            CourseClassGenerator classGenerator = new CourseClassGenerator(Students.ToList(), Teachers.ToList(), Subjects.ToList(), userSetting.ClassDivisionBaseline);
            List<CourseClass> courseClassesTmp = classGenerator.GenerateCourseClass();
            CourseClasses.AddRange(courseClassesTmp);
            CourseClassCount = CourseClasses.Count;
        }

        public void GenerateScheduledClasses(Action callback)
        {
            if (CourseClasses.Count <= 0)
                return; // 提示信息

            foreach (Slot slot in Schedule.Slots)
                slot.ScheduledClasses.Clear();

            CancellationToken = new CancellationToken();

            Task.Run(() =>
            {
                UserRestriction userRestriction = new UserRestriction(UserSetting.GlobalUnavailableTimeSots, 6);
                ScheduledClassGenerator scheduledClassGenerator = new ScheduledClassGenerator(Rooms.ToList(), userRestriction);
                List<ScheduledClass> scheduledClasses = scheduledClassGenerator.GenerateScheduledClass(courseClasses.ToList(), schedule.Slots);
                scheduledClasses = scheduledClasses.OrderBy(p => p.SlotId).ToList();

                if (scheduledClasses == null)
                    return;
                foreach (Slot slot in Schedule.Slots)
                {
                    foreach (var course in scheduledClasses)
                    {
                        if (course.SlotId == slot.SlotId)
                            Application.Current.Dispatcher.Invoke(() => slot.ScheduledClasses.Add(course));
                    }
                }

                scheduledClassesSaved = scheduledClasses;

                // Add Teacher Schedule Data
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AddTeacherSchedule(scheduledClasses);
                });

                // Add Student Schedule Data
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AddStudentSchedule(scheduledClasses);
                });

                Application.Current.Dispatcher.Invoke(callback);
            });
        }

        private void AddTeacherSchedule(List<ScheduledClass> scheduledClasses)
        {
            foreach (var teacher in Teachers)
            {
                teacher.TeacherSchedule = new Schedule();
                teacher.TeacherSchedule.Slots.Clear();

                // Init Slots
                for (int i = 0; i < 45; i++)
                {
                    Slot slot = new Slot();
                    slot.SlotId = i + 1;
                    teacher.TeacherSchedule.Slots.Add(slot);
                }

                // Add to data          
                foreach (var scheduleClass in scheduledClasses)
                {
                    if (scheduleClass.Course.Teacher.Id == teacher.Id)
                    {
                        teacher.TeacherSchedule.Slots[scheduleClass.SlotId - 1].ScheduledClasses.Add(scheduleClass);
                    }
                }
            }
        }

        private void AddStudentSchedule(List<ScheduledClass> scheduledClasses)
        {
            foreach (var student in Students)
            {
                student.StudentSchedule = new Schedule();
                student.StudentSchedule.Slots.Clear();

                // Init Slots
                for (int i = 0; i < 45; i++)
                {
                    Slot slot = new Slot();
                    slot.SlotId = i + 1;
                    student.StudentSchedule.Slots.Add(slot);
                }

                // Add to data          
                foreach (var scheduleClass in scheduledClasses)
                {
                    if (scheduleClass.Course.Students.Exists(std => std.Id == student.Id))
                    {
                        student.StudentSchedule.Slots[scheduleClass.SlotId - 1].ScheduledClasses.Add(scheduleClass);
                    }
                }
            }
        }

        public void SaveUserSetting()
        {
            UserSettingDTO userSettingSaved = mapper.Map<UserSettingDTO>(UserSetting);
            XmlSerializeManager.XmlSerialize<UserSettingDTO>(PrismManager.UserSettingSavedPath, userSettingSaved);
        }

        public void LoadUserSetting()
        {
            UserSettingDTO userSettingSaved = XmlSerializeManager.XmlDeserializer<UserSettingDTO>(PrismManager.UserSettingSavedPath);
            if (userSettingSaved == null)
            {
                UserSetting = defaultUserSetting;
                return;
            }   

            UserSetting = mapper.Map<UserSetting>(userSettingSaved);
            UserSetting.User.BitmapImage = new BitmapImage(new Uri(UserSetting.User.ImageSource));
        }

        public void InitSchedules()
        {
            for (int i = 0; i < 45; i++)
            {
                Slot slot = new Slot(); 
                slot.SlotId = i + 1;
                Schedule.Slots.Add(slot);
            }
        }
    }
}