using Class_Scheduler.Common.Models;
using Class_Scheduler.Extensions;
using Class_Scheduler.Service;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Linq;
namespace Class_Scheduler.ViewModels
{
    public class SubjectsViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly IRepositoryService repositoryService;

        public IRepositoryService RepositoryService
        {
            get { return repositoryService; }
        }

        public DelegateCommand AddSubjectCommand { get; private set; }
        public DelegateCommand<Subject> DeleteSubjectCommand { get; private set; }
        public DelegateCommand<Subject> ModifySubjectCommand { get; private set; }
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

        public SubjectsViewModel(IDialogService dialogService, IRepositoryService repositoryService) 
        {
            this.dialogService = dialogService;
            this.repositoryService = repositoryService;

            AddSubjectCommand = new DelegateCommand(AddSubject);
            DeleteSubjectCommand = new DelegateCommand<Subject>(DeleteSubject);
            ModifySubjectCommand = new DelegateCommand<Subject>(ModifySubject);
            DeleteSelectedCommand = new DelegateCommand(DeleteSelected);
            SelectAllCommand = new DelegateCommand(SelectAllSubject);
        }

        private void SelectAllSubject()
        {
            foreach (var subject in repositoryService.Subjects)
            {
                subject.IsSelected = IsAllSelected;
            }
        }

        private void DeleteSelected()
        {
            List<Subject> indices = new List<Subject>();
            foreach (var subject in repositoryService.Subjects)
            {
                if (subject.IsSelected)
                    indices.Add(subject);
            }

            foreach (var item in indices)
            {
                repositoryService.Subjects.Remove(item);
                repositoryService.SubjectAvailable.Remove(item.Name);
            }

            // Update the number
            repositoryService.SubjectsCount = repositoryService.Subjects.Count;
            IsAllSelected = false;
        }

        private void AddSubject()
        {
            dialogService.ShowDialog(PrismManager.AddSubjectDialogName, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    var subject = callback.Parameters.GetValue<Subject>("Subject");
                    if (repositoryService.Subjects.Count > 0)
                        subject.Id = repositoryService.Subjects[repositoryService.Subjects.Count - 1].Id + 1;
                    else
                        subject.Id = 0;
                    repositoryService.Subjects.Add(subject);
                    repositoryService.SubjectAvailable.Add(subject.Name);
                    repositoryService.SubjectsCount = repositoryService.Subjects.Count;
                }
            });
        }

        private void DeleteSubject(Subject subject)
        {
            DialogParameters keys = new DialogParameters();
            keys.Add("ShownData", "Are you sure you want to delete this subject?\r\n");
            dialogService.ShowDialog(PrismManager.DeleteMemberDialogName, keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    // Delete Members
                    if (!repositoryService.Subjects.Remove(subject))
                    {
                        // 日志输出
                    }
                    else
                    {
                        repositoryService.SubjectsCount = repositoryService.Subjects.Count; 
                        repositoryService.SubjectAvailable.Remove(subject.Name);
                    }
                }
            });
        }

        private void ModifySubject(Subject subject)
        {
            DialogParameters keys = new DialogParameters
            {
                { "Subject", subject }
            };
            
            dialogService.ShowDialog(PrismManager.ModifySubjectsDialogName, keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    var tmp = callback.Parameters.GetValue<Subject>("Subject");
                    repositoryService.SubjectAvailable.Remove(subject.Name);

                    var found = repositoryService.Subjects.Where(item => item.Id.Equals(tmp.Id)).FirstOrDefault();
                    found.Name = tmp.Name;
                    found.TeacherNames = tmp.TeacherNames;
                    found.SubjectGroup = tmp.SubjectGroup;
                    found.CountPerWeek = tmp.CountPerWeek;
                    repositoryService.SubjectAvailable.Add(tmp.Name);
                }
            });
        }
    }
}
