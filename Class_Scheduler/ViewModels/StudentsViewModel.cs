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
using System.Configuration;
using Class_Scheduler.Service;
using Class_Scheduler.Common.Models.DTOs;
using AutoMapper;

namespace Class_Scheduler.ViewModels
{
    public class StudentsViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly IRepositoryService repositoryService;

        public IRepositoryService RepositoryService { get { return repositoryService; } }

        public DelegateCommand AddStudentCommand { get; private set; }
        public DelegateCommand<Student> DeleteStudentCommand { get; private set; }

        public DelegateCommand<Student> ModifyStudentCommand { get; private set; }

        public DelegateCommand SelectAllCommand { get; private set; }
        public DelegateCommand DeleteSelectedCommand { get; private set; }

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

        public StudentsViewModel(IDialogService dialogService, IRepositoryService repositoryService, IMapper mapper)
        {
            this.dialogService = dialogService;
            this.repositoryService = repositoryService;

            AddStudentCommand = new DelegateCommand(AddStudent);
            DeleteStudentCommand = new DelegateCommand<Student>(DeleteStudent);
            ModifyStudentCommand = new DelegateCommand<Student>(ModifyStudent);
            SelectAllCommand = new DelegateCommand(SelectAllStd);
            DeleteSelectedCommand = new DelegateCommand(DeleteSelected);
        }

        private void SelectAllStd()
        {
            foreach (var std in repositoryService.Students)
            {
                std.IsSelected = IsAllSelected;
            }
        }

        private void DeleteSelected()
        {
            List<Student> indices = new List<Student>();
            foreach (var std in repositoryService.Students)
            {
                if (std.IsSelected)
                    indices.Add(std);
            }

            foreach (var item in indices)
                repositoryService.Students.Remove(item);

            // Update the number
            repositoryService.StudentsCount = repositoryService.Students.Count;
            IsAllSelected = false;
        }

        private void AddStudent()
        {
            IsAllSelected = true;
            dialogService.ShowDialog(PrismManager.AddStudentsDialogName, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    var student = callback.Parameters.GetValue<Student>("Student");
                    if (repositoryService.Students.Count > 0)
                        student.Id = repositoryService.Students[repositoryService.Students.Count - 1].Id + 1;
                    else
                        student.Id = 0;
                    repositoryService.Students.Add(student);
                    repositoryService.StudentsCount = repositoryService.Students.Count;
                }
            });
        }

        private void DeleteStudent(Student student)
        {
            DialogParameters keys = new DialogParameters();
            keys.Add("ShownData", "Are you sure you want to delete this student?\r\n");
            dialogService.ShowDialog(PrismManager.DeleteMemberDialogName, keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    // Delete Members
                    if (!repositoryService.Students.Remove(student))
                    {
                        // 日志输出
                    }
                    else
                    {
                        repositoryService.StudentsCount = repositoryService.Students.Count;
                    }
                }
            });
        }

        private void ModifyStudent(Student student)
        {
            DialogParameters keys = new DialogParameters
            {
                { "Student", student }
            };
            dialogService.ShowDialog(PrismManager.ModifyStudentDialogName, keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    var std = callback.Parameters.GetValue<Student>("Student");

                    var found = repositoryService.Students.Where(obj => obj.Id.Equals(std.Id)).FirstOrDefault();
                    found.SubjectSelected = std.SubjectSelected;
                    found.SubjectScores = std.SubjectScores;
                    found.Sex = std.Sex;
                    found.Name = std.Name;
                    found.EmailAddress = std.EmailAddress;
                }
            });
        }
    }
}
