using Class_Scheduler.Common.Models;
using Class_Scheduler.Extensions;
using Class_Scheduler.Service;
using Prism.Services.Dialogs;
using System.IO;
using ExcelDataReader;
using System.Data;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using Microsoft.Win32;
using Class_Scheduler.Service.DataImport;

namespace Class_Scheduler.ViewModels
{
    public class MainViewModel : BindableBase, IConfigureService
    {
        private readonly IRegionManager regionManager;
        private IRepositoryService repositoryService;
        private readonly IDialogService dialogService;

        public IRepositoryService RepositoryService
        {
            get { return repositoryService; }
            set { repositoryService = value; RaisePropertyChanged(); }
        }

        #region Command
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand SaveToCommand { get; private set; }
        public DelegateCommand SettingCommand { get; private set; }
        public DelegateCommand OpenFileCommand { get; private set; }

        public DelegateCommand QuiteCommand { get; private set; }
        #endregion

        public MainViewModel(IRegionManager regionManager, IRepositoryService repositoryService, IDialogService dialogService)
        {
            NavigateCommand = new DelegateCommand<string>(Navigate);
            SaveCommand = new DelegateCommand(SaveData);
            SettingCommand = new DelegateCommand(OpenSettingDialog);
            QuiteCommand = new DelegateCommand(OnQuite);
            SaveToCommand = new DelegateCommand(SaveTo);
            OpenFileCommand = new DelegateCommand(OpenFile);

            this.regionManager = regionManager;
            this.repositoryService = repositoryService;
            this.dialogService = dialogService;
        }

        private void OpenFile()
        {
            string path = "";

            // Get the path of the data first
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml File|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
            }

            // Set the global path
            PrismManager.SavedPath = path;

            // Create import context and select the xml strategy
            ImportDataServices importDataServices = new ImportDataServices();
            importDataServices.CreateImportContext(new XmlGlobalDataImporter());
            importDataServices.SetPath(path);
            importDataServices.ImportData(false);
            SavableObject savableObject = importDataServices.GetSavableObject();
            
            // Convert the savableObject
            repositoryService.LoadData(savableObject);
        }

        private void OnQuite()
        {
            bool quite = true;
            if (PrismManager.SavedPath == null || PrismManager.SavedPath == string.Empty)
            {
                // Open the confirm save dialog
                dialogService.ShowDialog(PrismManager.ConfirmSaveDialogName, callback =>
                {
                    if (callback.Result == ButtonResult.OK)
                    {
                        // Set the saving path.
                        PrismManager.SavedPath = callback.Parameters.GetValue<string>("SavedPath");
                        // Save Data
                        repositoryService.SaveData();
                    }
                    else if (callback.Result == ButtonResult.Abort)
                        quite = false;
                });
            }
            else
            {
                if (repositoryService.UserSetting.AutoSaveWhenClose)
                    repositoryService.SaveData();
            }

            if (quite)
                App.Current.Shutdown();
        }

        private void OpenSettingDialog()
        {
            dialogService.ShowDialog(PrismManager.SettingsDialogName, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    var settings = callback.Parameters.GetValue<UserSetting>("UserSetting");
                    
                    if (settings.ImportDataPath != string.Empty && settings.ImportDataPath != null)
                    {
                        LoadUserSetting(settings);
                    }
                }          
            });
        }

        private void LoadSavableObject(SavableObject savableObject)
        {
            if (savableObject == null)
                return;
        }

        private void LoadUserSetting(UserSetting settings)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // 判断是否有被打开
            FileStream stream = File.Open(settings.ImportDataPath, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            if (excelDataReader == null)
                return;

            DataSet result = excelDataReader.AsDataSet();

            // Import data
            DataTableCollection tableCollection = result.Tables;

            // tableCollection[0] --> Students
            DataTable studentTable = tableCollection[0];
            for (int i = 1; i < studentTable.Rows.Count; i++)
            {
                Student student = new Student();

                if (repositoryService.Students.Count > 0)
                {
                    student.Id = repositoryService.Students[^1].Id + 1;
                }
                else
                {
                    if (int.TryParse(studentTable.Rows[i][0].ToString(), out int id))
                        student.Id = id;
                }

                student.Name = studentTable.Rows[i][1].ToString()!;
                student.GradeAndClass = studentTable.Rows[i][2].ToString()!;
                student.Sex = studentTable.Rows[i][3].ToString()!;
                student.EmailAddress = studentTable.Rows[i][4].ToString()!;

                bool flag = true;
                for (int j = 5; j < studentTable.Columns.Count; j++)
                {
                    if (flag)
                    {
                        student.SubjectSelected.Add(studentTable.Rows[i][j].ToString()!);
                    }
                    else
                    {
                        if (int.TryParse(studentTable.Rows[i][j].ToString(), out int score))
                            student.SubjectScores.Add(score);
                    }

                    flag = !flag;
                }

                repositoryService.Students.Add(student);
            }
            repositoryService.StudentsCount = repositoryService.Students.Count;

            // tableCollection[1] --> Teachers
            DataTable teacherTable = tableCollection[1];
            for (int i = 1; i < teacherTable.Rows.Count; i++)
            {
                Teacher teacher = new Teacher();
                if (repositoryService.Teachers.Count > 0)
                {
                    teacher.Id = repositoryService.Teachers[repositoryService.Teachers.Count - 1].Id + 1;
                }
                else
                {
                    if (int.TryParse(teacherTable.Rows[i][0].ToString(), out int id))
                        teacher.Id = id;
                }

                teacher.Name = teacherTable.Rows[i][1].ToString()!;
                teacher.NickName = teacherTable.Rows[i][2].ToString()!;
                teacher.EmailAddress = teacherTable.Rows[i][3].ToString()!;
                string subjecStr = teacherTable.Rows[i][4].ToString()!;
                teacher.Subjects = subjecStr;
                string[] subjects = subjecStr.Split(',');
                if (subjects != null)
                    teacher.SubjectList.AddRange(subjects);

                string busyTimes = teacherTable.Rows[i][5].ToString()!;
                if (busyTimes != null && busyTimes != string.Empty)
                {
                    string[] slotsIdStr = busyTimes.Split(",");
                    if (slotsIdStr != null)
                    {
                        foreach (string str in slotsIdStr)
                        {
                            if (int.TryParse(str, out int num))
                                teacher.UnavailableTimeSlotId.Add(num);
                        }
                    }
                }

                repositoryService.Teachers.Add(teacher);
            }
            repositoryService.TeachersCount = repositoryService.Teachers.Count;

            // Subject 添加判断条件
            DataTable subjectTable = tableCollection[2];
            for (int i = 1; i < subjectTable.Rows.Count; i++)
            {
                Subject subject = new Subject();
                if (repositoryService.Subjects.Count > 0)
                {
                    subject.Id = repositoryService.Subjects[^1].Id + 1;
                }
                else
                {
                    if (int.TryParse(subjectTable.Rows[i][0].ToString(), out int id))
                        subject.Id = id;
                }

                subject.Name = subjectTable.Rows[i][1].ToString()!;
                string teacherNamesStr = subjectTable.Rows[i][2].ToString()!;
                subject.TeacherNames = teacherNamesStr;
                string[] teacherNames = teacherNamesStr.Split(',');
                for (int j = 0; j < teacherNames.Length; j++)
                {
                    var collection = repositoryService.Teachers.Where(obj => obj.Name.Equals(teacherNames[j]));
                    Teacher? teacher = null;
                    if (collection.Count() > 0)
                    {
                        teacher = collection.First();
                        subject.TeachersList.Add(teacher);
                    }
                }
                subject.SubjectGroup = subjectTable.Rows[i][3].ToString()!;
                if (int.TryParse(subjectTable.Rows[i][4].ToString()!, out int cnt))
                    subject.CountPerWeek = cnt;
                subject.NorminalName = subjectTable.Rows[i][5].ToString()!;

                repositoryService.Subjects.Add(subject);
                repositoryService.SubjectAvailable.Add(subject.Name);
            }
            repositoryService.SubjectsCount = repositoryService.Subjects.Count;

            // Room
            DataTable roomTable = tableCollection[3];
            for (int i = 1; i < roomTable.Rows.Count; i++)
            {
                Room room = new Room();
                if (int.TryParse(roomTable.Rows[i][0].ToString(), out int id))
                    room.Id = id;

                if (int.TryParse(roomTable.Rows[i][1].ToString(), out int roomNum))
                    room.Name = roomNum;

                room.Purpose = roomTable.Rows[i][2].ToString()!;

                repositoryService.Rooms.Add(room);
            }
            repositoryService.RoomsCount = repositoryService.Rooms.Count;

            excelDataReader.Close();
            //try
            //{

            //}
            //catch(Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.ToString());
            //}
            settings.ImportDataPath = string.Empty;
        }

        public void Navigate(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                // ToDo: 添加日志输出
                return;
            }

            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(target);
        }

        public void SaveData()
        {
            repositoryService.SaveData();
        }

        public void SaveTo()
        {
            repositoryService.SaveData(true);
        }

        public void Configure()
        {
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("SchedulesView");
        }
    }
}