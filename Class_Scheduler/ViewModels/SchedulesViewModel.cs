using Class_Scheduler.Extensions;
using Class_Scheduler.Service;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Threading;

namespace Class_Scheduler.ViewModels
{
    public class SchedulesViewModel : BindableBase
    {
        private IRepositoryService repositoryService;
        private readonly IDialogService dialogService;

        public IRepositoryService RepositoryService
        {
            get { return repositoryService; }
            set {  repositoryService = value; }
        }

        public DelegateCommand GenerateScheduleCommand { get; private set; }
        public DelegateCommand CancelGenerationCommand { get; private set; }

        public DelegateCommand ViewTeacherTimetableCommand { get; private set; }
        public DelegateCommand ViewStudentTimetableCommand { get; private set; }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; RaisePropertyChanged();}
        }

        private string time;

        public string Time
        {
            get { return time; }
            set { time = value; RaisePropertyChanged(); }
        }

        public string[] WeekdayList { get; private set; }   

        private TimeSpan span = new TimeSpan(0, 0, 0);
        private DispatcherTimer timer = new DispatcherTimer();

        public SchedulesViewModel(IRepositoryService repositoryService, IDialogService dialogService)
        {
            this.repositoryService = repositoryService;
            this.dialogService = dialogService;

            GenerateScheduleCommand = new DelegateCommand(GenerateSchedule);
            CancelGenerationCommand = new DelegateCommand(CancelGeneration);
            ViewTeacherTimetableCommand = new DelegateCommand(ViewTeacherTimetable);
            ViewStudentTimetableCommand = new DelegateCommand(ViewStudentTimetable);

            WeekdayList = new string[] { "Mon", "Tue", "Wed", "Thu", "Fri" };

            Status = "Status: Prepared";
            timer.Interval = TimeSpan.FromSeconds(1);
            Time = "Continuing time (00:00:00)";
        }
        
        private void CancelGeneration()
        {
            timer.Tick -= onTimeStart;
            timer.Stop();
            Time = $"Continuing time ({span.ToString()})";

            Status = "Status: Prepared";
        }

        private void GenerateSchedule()
        {
            if (repositoryService.CourseClassCount <= 0)
                return;

            span = new TimeSpan(0, 0, 0);
            Status = "Status: Generating...";

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
         
            timer.Tick += onTimeStart;
            timer.Start();

            repositoryService.GenerateScheduledClasses(onComplete);
        }

        private void ViewTeacherTimetable()
        {
            dialogService.ShowDialog(PrismManager.TeacherScheduleDialogName, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    
                }
            });
        }

        private void ViewStudentTimetable()
        {
            dialogService.ShowDialog(PrismManager.StudentScheduleDialogName, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {

                }
            });

        }

        private void onTimeStart(object sender, EventArgs e)
        {
            span = span.Add(new TimeSpan(0, 0, 1));
            Time = $"Continuing time ({span})";
        }

        private void onComplete()
        {
            // Send Message
            timer.Tick -= onTimeStart;
            timer.Stop();
            Time = $"Continuing time ({span.ToString()})";

            Status = "Status: Prepared";
        }
    }
}
