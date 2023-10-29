using Class_Scheduler.Common.Models;
using Class_Scheduler.Service;
using ExcelDataReader;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.ViewModels.Settings
{
    public class PreferenceSettingsViewModel : BindableBase, ISavableSettings
    {
        private readonly IDialogService dialogService;
        private readonly ISettingService settingService;

        private bool autoSave;
        public bool AutoSaveClose
        {
            get { return autoSave; }
            set { autoSave = value; RaisePropertyChanged(); }
        }

        public PreferenceSettingsViewModel(IDialogService dialogService, ISettingService settingService) 
        {
            this.dialogService = dialogService;
            this.settingService = settingService;

            settingService.RegisterSettingInstance(this);

            AutoSaveClose = settingService.UserSetting.AutoSaveWhenClose;
        }

        public void OnSave()
        {
            settingService.RegisterData("AutoSaveWhenClose", AutoSaveClose);
        }

        public void Reset(UserSetting userSetting)
        {
            AutoSaveClose = userSetting.AutoSaveWhenClose;
        }
    }
}
