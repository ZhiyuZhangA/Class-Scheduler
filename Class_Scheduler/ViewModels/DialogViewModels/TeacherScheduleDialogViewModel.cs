using Class_Scheduler.Common.Models;
using Class_Scheduler.Common.Models.ClassScheduling;
using Class_Scheduler.Extensions;
using Class_Scheduler.Service;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Class_Scheduler.ViewModels.DialogViewModels
{
    public class TeacherScheduleDialogViewModel : BindableBase, IDialogAware
    {
        private readonly IRepositoryService repositoryService;
        public string Title => "Teacher Schedule Timetable";

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand GenerateScheduleCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }

        public List<string> TeacherNames { get; private set; }
        public string SelectedTeacherName { get; set; }

        private Schedule schedule;
        public Schedule Schedule
        {
            get { return schedule; }
            set { schedule = value; RaisePropertyChanged(); }
        }

        public TeacherScheduleDialogViewModel(IRepositoryService repositoryService)
        {
            this.repositoryService = repositoryService;

            GenerateScheduleCommand = new DelegateCommand(generateSchedule);
            CloseCommand = new DelegateCommand(OnDialogClosed);

            TeacherNames = new List<string>();
            Schedule = new Schedule();

            foreach (var item in repositoryService.Teachers)
            {
                TeacherNames.Add(item.Name);
            }

            InitSchedules();
        }

        private void generateSchedule()
        {
            if (SelectedTeacherName != null && SelectedTeacherName != string.Empty)
            {
                var selectedTeachers = repositoryService.Teachers.Where(teacher => teacher.Name == SelectedTeacherName);
                if (selectedTeachers.Any()) 
                {
                    Teacher teacher = selectedTeachers.First();
                    Schedule = teacher.TeacherSchedule;
                }
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Abort));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }

        private void InitSchedules()
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
