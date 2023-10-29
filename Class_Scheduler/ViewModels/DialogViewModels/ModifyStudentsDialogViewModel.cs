using Class_Scheduler.Common.Models;
using Class_Scheduler.Service;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.ViewModels.DialogViewModels
{
    public class ModifyStudentsDialogViewModel : BindableBase, IDialogAware
    {
        private readonly IRepositoryService repositoryService;
        public string Title => "Modify student's data";

        public event Action<IDialogResult> RequestClose;

        public Student Student { get; set; }
        public Student InputStd { get; set; }

        public string[] Sexes { get; set; }
        public int[] Scores { get; set; }

        private int[] stdSubjectScore;
        public int[] StdSubjectScores
        {
            get { return stdSubjectScore; }
            set { stdSubjectScore = value; RaisePropertyChanged(); }
        }

        private string[] stdSubjectSelected;
        public string[] StdSubjectSelected
        {
            get { return stdSubjectSelected; }
            set { stdSubjectSelected = value; RaisePropertyChanged(); }
        }

        public IRepositoryService RepositoryService { get { return repositoryService; } }

        public DelegateCommand SubmitCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public ModifyStudentsDialogViewModel(IRepositoryService repositoryService) 
        {
            this.repositoryService = repositoryService;
            Sexes = new string[] { "Male", "Female" };
            Scores = new int[] { 1, 2, 3, 4, 5, 6, 7 };

            SubmitCommand = new DelegateCommand(SubmitData);
            CancelCommand = new DelegateCommand(Cancel);

            InputStd = new Student();
           
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Student = parameters.GetValue<Student>("Student");
            InputStd = Student.Copy();
            StdSubjectScores = InputStd.SubjectScores.ToArray();
            StdSubjectSelected = InputStd.SubjectSelected.ToArray();
        }

        private void SubmitData()
        {
            DialogParameters keys = new DialogParameters
            {
                { "Student", InputStd }
            };
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, keys));
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }
    }
}
