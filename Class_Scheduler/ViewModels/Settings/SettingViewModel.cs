using Class_Scheduler.Extensions;
using Class_Scheduler.Service;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;

namespace Class_Scheduler.ViewModels
{
    public class SettingViewModel : BindableBase, IDialogAware
    {
        private IRegionManager regionManager;
        private readonly ISettingService settingService;
        private readonly IRepositoryService repositoryService;
            
        public IRegionManager RegionManager
        {
            get { return regionManager; }
            set { regionManager = value; } 
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand CancelCommand {  get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

        public DelegateCommand ResetSettingsCommand { get; private set; }

        public string Title => "";

        public event Action<IDialogResult> RequestClose;

        public SettingViewModel(IRegionManager regionManager, ISettingService settingService, IRepositoryService repositoryService)
        {
            this.regionManager = regionManager;
            this.settingService = settingService;
            this.repositoryService = repositoryService;

            NavigateCommand = new DelegateCommand<string>(OnNavigate);
            CloseCommand = new DelegateCommand(OnDialogClosed);
            CancelCommand = new DelegateCommand(OnCancel);
            SaveCommand = new DelegateCommand(OnSave);
            ResetSettingsCommand = new DelegateCommand(ResetSetting);
        }

        private void ResetSetting()
        {
            settingService.Reset(repositoryService.DefaultUserSetting);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            RegionManager.Regions.Remove(PrismManager.SettingViewRegionName);
            
            RequestClose?.Invoke(new DialogResult(ButtonResult.Abort));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            regionManager.Regions[PrismManager.SettingViewRegionName].RequestNavigate("AccountSettingsView");
        }

        private void OnNavigate(string target)
        {
            regionManager.Regions[PrismManager.SettingViewRegionName].RequestNavigate(target);   
        }

        public void OnCancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private void OnSave()
        {
            settingService.Save();
            repositoryService.UserSetting = settingService.UserSetting;
            
            foreach (var teacher in repositoryService.Teachers)
            {
                for (int i = 0; i < settingService.UserSetting.GlobalUnavailableTimeSots.Count; i++)
                    teacher.UnavailableTimeSlotId.Add(settingService.UserSetting.GlobalUnavailableTimeSots[i]);
            }

            repositoryService.SaveUserSetting();

            DialogParameters keys = new DialogParameters();
            keys.Add("UserSetting", settingService.UserSetting);
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, keys));
        }
    }
}
