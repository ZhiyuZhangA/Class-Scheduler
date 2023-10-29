using AutoMapper;
using Class_Scheduler.Extensions;
using Class_Scheduler.Service;
using Class_Scheduler.Service.DataImport;
using Class_Scheduler.ViewModels;
using Class_Scheduler.ViewModels.DialogViewModels;
using Class_Scheduler.ViewModels.Settings;
using Class_Scheduler.Views;
using Class_Scheduler.Views.DialogView;
using Class_Scheduler.Views.Settings;
using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;

namespace Class_Scheduler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized()
        {
            var service = App.Current.MainWindow.DataContext as IConfigureService;
            
            if (service != null)
                service.Configure();
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SubjectsView, SubjectsViewModel>();
            containerRegistry.RegisterForNavigation<ClassroomsView, ClassroomsViewModel>();
            containerRegistry.RegisterForNavigation<SchedulesView, SchedulesViewModel>();
            containerRegistry.RegisterForNavigation<StudentsView, StudentsViewModel>();
            containerRegistry.RegisterForNavigation<TeachersView, TeachersViewModel>();
            containerRegistry.RegisterForNavigation<ClassesView, ClassesViewModel>();
            containerRegistry.RegisterForNavigation<AccountSettingsView, AccountSettingsViewModel>();
            containerRegistry.RegisterForNavigation<AdvanceSettingsView, AdvanceSettingsViewModel>();
            containerRegistry.RegisterForNavigation<PreferenceSettingsView, PreferenceSettingsViewModel>(); 

            containerRegistry.RegisterDialog<SettingView, SettingViewModel>();
            containerRegistry.RegisterDialog<DeleteMembersDialogView, DeleteMembersDialogViewModel>();
            containerRegistry.RegisterDialog<ModifyStudentsDialogView, ModifyStudentsDialogViewModel>();
            containerRegistry.RegisterDialog<ModifyTeachersDialogView, ModifyTeachersDialogViewModel>();
            containerRegistry.RegisterDialog<AddSubjectsDialogView, AddSubjectsDialogViewModel>();
            containerRegistry.RegisterDialog<ModifySubjectsDialogView, ModifySubjectsDialogViewModel>();
            containerRegistry.RegisterDialog<AddRoomsDialogView, AddRoomsDialogViewModel>();
            containerRegistry.RegisterDialog<ModifyRoomsDialogView, ModifyRoomsDialogViewModel>();
            containerRegistry.RegisterDialog<AddStudentsDialogView, AddStudentsDialogViewModel>();
            containerRegistry.RegisterDialog<AddTeachersDialogView, AddTeachersDialogViewModel>();
            containerRegistry.RegisterDialog<TeacherScheduleDialogView, TeacherScheduleDialogViewModel>();
            containerRegistry.RegisterDialog<StudentScheduleDialogView, StudentScheduleDialogViewModel>();
            containerRegistry.RegisterDialog<ConfirmQuitDialogView, ConfirmQuitDialogViewModel>();

            containerRegistry.RegisterSingleton<IRepositoryService,  RepositoryService>();
            containerRegistry.RegisterSingleton<ISettingService, SettingService>();
            containerRegistry.Register<IImportDataServices, ImportDataServices>();

            var autoMapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProfile());
            });

            containerRegistry.RegisterInstance<IMapper>(autoMapperConfig.CreateMapper());
        }
    }
}
