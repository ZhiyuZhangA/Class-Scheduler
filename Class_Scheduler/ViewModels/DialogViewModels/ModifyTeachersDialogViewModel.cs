using Class_Scheduler.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Text;

namespace Class_Scheduler.ViewModels.DialogViewModels
{
    public class ModifyTeachersDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "Modify teacher's data";

        public event Action<IDialogResult> RequestClose;

        public Teacher Teacher { get; set; }

        
        private string unavailableSlots;

        /// <summary>
        /// Unavailable Time Slots
        /// </summary>
        public string UnavailableSlots
        {
            get { return unavailableSlots; }
            set { unavailableSlots = value; RaisePropertyChanged(); }
        }

        public DelegateCommand SubmitCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public ModifyTeachersDialogViewModel() 
        {
            SubmitCommand = new DelegateCommand(SubmitData);
            CancelCommand = new DelegateCommand(Cancel);
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
            Teacher = parameters.GetValue<Teacher>("Teacher");
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < Teacher.UnavailableTimeSlotId.Count; i++)
            //{
            //    sb.Append(Teacher.UnavailableTimeSlotId[i]);
            //    if (i != Teacher.UnavailableTimeSlotId.Count - 1)
            //        sb.Append(",");
            //}
            UnavailableSlots = Teacher.UnavailableTimeSlotString;
        }

        private void SubmitData()
        {
            System.Diagnostics.Debug.WriteLine(UnavailableSlots);
            Teacher.UnavailableTimeSlotString = UnavailableSlots;
            if (UnavailableSlots != string.Empty && UnavailableSlots != null)
            {
                string[] slots = UnavailableSlots.Split(',');
                for (int i = 0; i < slots.Length; i++)
                {
                    if (int.TryParse(slots[i], out int id))
                        Teacher.UnavailableTimeSlotId.Add(id);
                    else
                        System.Diagnostics.Debug.WriteLine("Teacher slot modification error: Format Incorrect!");
                }
            }      

            DialogParameters keys = new DialogParameters
            {
                { "Teacher", Teacher }
            };
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, keys));
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }
    }
}
