using Class_Scheduler.Common.Models;
using Class_Scheduler.Service;
using Class_Scheduler.ViewModels.Settings;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Media.Imaging;

namespace Class_Scheduler.ViewModels
{
    public class AccountSettingsViewModel : BindableBase, ISavableSettings
    {
        private readonly ISettingService settingService;

        private User user;
        public User User
        {
            get { return user; }
            set { user = value; RaisePropertyChanged(); }
        }

        private BitmapImage image;
        public BitmapImage BitmapImage
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }

        private string schoolName;
        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value; RaisePropertyChanged(); }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }

        private string emailAddress;

        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; RaisePropertyChanged(); }
        }

        public string ImageSource { get; set; }

        public DelegateCommand ReplacePhotoCommand { get; private set; }

        public AccountSettingsViewModel(ISettingService settingService) 
        {
            this.settingService = settingService;
            ReplacePhotoCommand = new DelegateCommand(ReplacePhoto);

            settingService.RegisterSettingInstance(this);

            BitmapImage = new BitmapImage();
            User = settingService.UserSetting.User;
            if (User != null)
            {
                Password = User.Password;
                UserName = User.UserName;
                SchoolName = User.SchoolName;
                EmailAddress = User.Email;
                ImageSource = User.ImageSource;
            }
            
            if (ImageSource != string.Empty && ImageSource != null)
            {
                BitmapImage.BeginInit();
                BitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                BitmapImage.UriSource = new Uri(ImageSource.ToString());
                BitmapImage.EndInit();
            }
        }

        private void ReplacePhoto()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Image Files|*.jpg;*.png;*.jpeg;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {            
                ImageSource = openFileDialog.FileName;
                BitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        public void OnSave()
        {
            System.Diagnostics.Debug.WriteLine(SchoolName);
            User.Email = EmailAddress;
            User.Password = Password;
            User.SchoolName = SchoolName;
            User.UserName = UserName;
            User.BitmapImage = BitmapImage;
            User.ImageSource = ImageSource;
            
            settingService.RegisterData("User", User);
        }

        public void Reset(UserSetting userSetting)
        {
            
        }
    }
}
