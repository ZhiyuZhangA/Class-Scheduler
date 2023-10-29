using Class_Scheduler.Common.Models;
using Class_Scheduler.Extensions;
using Class_Scheduler.Service;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;

namespace Class_Scheduler.ViewModels
{
    public class ClassesViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly IRepositoryService repositoryService;

        public IRepositoryService RepositoryService
        {
            get { return repositoryService; }
        }

        public DelegateCommand GenerateClassesCommand { get; private set; }
        public DelegateCommand<CourseClass> ModifyClassCommand { get; private set; }
        public DelegateCommand<CourseClass> DeleteClassCommand {  get; private set; }
        public DelegateCommand AddClassCommand { get; private set; }
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

        public ClassesViewModel(IDialogService dialogService, IRepositoryService repositoryService)
        {
            this.dialogService = dialogService;
            this.repositoryService = repositoryService;

            GenerateClassesCommand = new DelegateCommand(GenerateClasses);
            ModifyClassCommand = new DelegateCommand<CourseClass>(ModifyClass);
            DeleteClassCommand = new DelegateCommand<CourseClass>(DeleteClass);
            SelectAllCommand = new DelegateCommand(SelectAllClasss);
            DeleteSelectedCommand = new DelegateCommand(DeleteSelected);
        }

        private void SelectAllClasss()
        {
            foreach (var courseClass in repositoryService.CourseClasses)
            {
                courseClass.IsSelected = IsAllSelected;
            }
        }

        private void DeleteSelected()
        {
            List<CourseClass> indices = new List<CourseClass>();
            foreach (var courseClass in repositoryService.CourseClasses)
            {
                if (courseClass.IsSelected)
                    indices.Add(courseClass);
            }

            foreach (var item in indices)
                repositoryService.CourseClasses.Remove(item);

            // Update the number
            repositoryService.CourseClassCount = repositoryService.CourseClasses.Count;
            IsAllSelected = false;
        }

        private void DeleteClass(CourseClass courseClass)
        {
            DialogParameters keys = new DialogParameters();
            keys.Add("ShownData", "Are you sure you want to delete this class?\r\n");
            dialogService.ShowDialog(PrismManager.DeleteMemberDialogName, keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    // Delete Members
                    if (!repositoryService.CourseClasses.Remove(courseClass))
                    {
                        // 日志输出
                    }
                    else
                    {
                        repositoryService.CourseClassCount = repositoryService.CourseClasses.Count;
                    }
                }
            });
        }
        
        private void ModifyClass(CourseClass courseClass)
        {
            
        }

        private void GenerateClasses()
        {
            repositoryService.GenerateCourseClasses();
        }
    }
}
