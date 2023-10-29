using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class_Scheduler.Common.Models;
using Prism.Commands;
using Class_Scheduler.Extensions;
using Class_Scheduler.Service;

namespace Class_Scheduler.ViewModels
{
    public class TeachersViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly IRepositoryService repositoryService;

        public IRepositoryService RepositoryService { get { return repositoryService; } }

        public DelegateCommand AddTeacherCommand { get; private set; }

        public DelegateCommand<Teacher> DeleteTeacherCommand { get; private set; }

        public DelegateCommand<Teacher> ModifyTeacherCommand { get; private set; }

        public DelegateCommand DeleteSelectedCommand { get; private set; }

        public DelegateCommand SelectAllCommand { get; private set; }


        private bool isAllSelected;
        public bool IsAllSelected
        {
            get { return isAllSelected; }
            set
            {
                isAllSelected = value;
                RaisePropertyChanged();
            }
        }

        public TeachersViewModel(IDialogService dialogService, IRepositoryService repositoryService)
        {
            this.dialogService = dialogService;
            this.repositoryService = repositoryService;

            AddTeacherCommand = new DelegateCommand(AddTeacher);
            DeleteTeacherCommand = new DelegateCommand<Teacher>(DeleteTeacher);
            ModifyTeacherCommand = new DelegateCommand<Teacher>(ModifyTeacher);
            DeleteSelectedCommand = new DelegateCommand(DeleteSelected);
            SelectAllCommand = new DelegateCommand(SelectAllTeachers);
        }

        private void SelectAllTeachers()
        {
            foreach (var teacher in repositoryService.Teachers)
            {
                teacher.IsSelected = IsAllSelected;
            }
        }

        private void DeleteSelected()
        {
            List<Teacher> indices = new List<Teacher>();
            foreach (var teacher in repositoryService.Teachers)
            {
                if (teacher.IsSelected)
                    indices.Add(teacher);
            }

            foreach (var item in indices)
                repositoryService.Teachers.Remove(item);

            // Update the number
            repositoryService.TeachersCount = repositoryService.Teachers.Count;
            IsAllSelected = false;
        }

        private void AddTeacher()
        {
            dialogService.ShowDialog(PrismManager.AddTeachersDialogName, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    Teacher teacher = callback.Parameters.GetValue<Teacher>("Teacher");
                    if (repositoryService.Teachers.Count > 0)
                        teacher.Id = repositoryService.Teachers[repositoryService.Teachers.Count - 1].Id + 1;
                    else
                        teacher.Id = 0;
                    RepositoryService.Teachers.Add(teacher);
                    RepositoryService.TeachersCount++;
                }
            });
        }

        private void DeleteTeacher(Teacher teacher)
        {
            DialogParameters keys = new DialogParameters();
            keys.Add("ShownData", "Are you sure you want to delete this teacher?\r\n");

            dialogService.ShowDialog(PrismManager.DeleteMemberDialogName, keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    // Delete Members
                    if (!RepositoryService.Teachers.Remove(teacher))
                    {
                        // 日志输出
                    }
                    else
                    {
                        repositoryService.TeachersCount = repositoryService.Teachers.Count;
                    }
                }
            });
        }

        private void ModifyTeacher(Teacher teacher)
        {
            DialogParameters keys = new DialogParameters
            {
                { "Teacher", teacher }
            };
            dialogService.ShowDialog(PrismManager.ModifyTeacherDialogName, keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    RepositoryService.Teachers[teacher.Id] = callback.Parameters.GetValue<Teacher>("Teacher");
                }
            });
        }
    }
}
