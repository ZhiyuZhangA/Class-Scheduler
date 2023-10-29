using Class_Scheduler.Extensions;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Class_Scheduler.ViewModels.DialogViewModels
{
    public class ConfirmQuitDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "";

        public event Action<IDialogResult> RequestClose;

        private string savingPath;

        public string SavingPath
        {
            get { return savingPath; }
            set { savingPath = value; RaisePropertyChanged(); }
        }

        private string fileName;

        public DelegateCommand ConfirmSaveCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand ChooseLocationCommand { get; private set; }

        public DelegateCommand CloseCommand { get; private set; }

        public ConfirmQuitDialogViewModel()
        {
            ConfirmSaveCommand = new DelegateCommand(ConfirmSave);
            CancelCommand = new DelegateCommand(Cancel);
            ChooseLocationCommand = new DelegateCommand(ChooseLocation);
            CloseCommand = new DelegateCommand(CloseDialog);
        }

        private void CloseDialog()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Abort));
        }

        private void ChooseLocation()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "Choosing a Location";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SavingPath = dialog.FileName;
            }
        }

        private void ConfirmSave()
        {
            if (FileName == null || FileName == string.Empty || SavingPath == null || SavingPath == string.Empty)
                return;

            DialogParameters key = new DialogParameters();
            key.Add("SavedPath", SavingPath + "/" + FileName + ".xml");
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, key));
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; RaisePropertyChanged(); }
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
            SavingPath = PrismManager.DefaultSavingPath;
        }
    }
}
