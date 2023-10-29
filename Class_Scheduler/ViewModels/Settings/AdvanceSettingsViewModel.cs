using Class_Scheduler.Common.Models;
using Class_Scheduler.Service;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using OfficeOpenXml;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Class_Scheduler.ViewModels.Settings
{
    public class AdvanceSettingsViewModel : BindableBase, ISavableSettings
    {
        private readonly IDialogService dialogService;
        private readonly ISettingService settingService;
        private readonly IRepositoryService repositoryService;

        private string importedDocPath;
        private int classDivisionBaseline;
        private int populationSize;
        private double mutationProb;
        private string naTimeSlots;
        private SavableObject savedData;

        private List<int> naSlotsList;

        public DelegateCommand ImportDataCommand { get; private set; }

        public DelegateCommand ExportToExcelCommand { get; private set; }

        public string ImportedDocPath 
        {
            get { return importedDocPath; }
            set { importedDocPath = value; RaisePropertyChanged(); }
        }

        public int ClassDivisionBaseline
        {
            get { return classDivisionBaseline; }
            set { classDivisionBaseline = value; RaisePropertyChanged(); }
        }

        public int PopulationSize
        {
            get { return populationSize; }
            set { populationSize = value; RaisePropertyChanged(); }
        }

        public double MutationProb
        {
            get { return mutationProb; }
            set { mutationProb = value; RaisePropertyChanged(); }
        }

        public string UnavailableTimeSlots
        {
            get { return naTimeSlots; }
            set { naTimeSlots = value; RaisePropertyChanged(); }
        }

        public SavableObject SavedData
        {
            get { return savedData; }
            set { savedData = value; }
        }

        public AdvanceSettingsViewModel(IDialogService dialogService, ISettingService settingService, IRepositoryService repositoryService) 
        {
            this.dialogService = dialogService;
            this.settingService = settingService;
            this.repositoryService = repositoryService;

            ImportDataCommand = new DelegateCommand(ImportData);
            ExportToExcelCommand = new DelegateCommand(ExportToExcel);

            settingService.RegisterSettingInstance(this);
            ClassDivisionBaseline = settingService.UserSetting.ClassDivisionBaseline;
            PopulationSize = settingService.UserSetting.PopulationSize;
            MutationProb = settingService.UserSetting.MutationProb;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < settingService.UserSetting.GlobalUnavailableTimeSots.Count; i++)
            {
                sb.Append(settingService.UserSetting.GlobalUnavailableTimeSots[i].ToString());
                if (i != settingService.UserSetting.GlobalUnavailableTimeSots.Count - 1)
                    sb.Append(',');
            }
            UnavailableTimeSlots = sb.ToString();
        }

        private void ExportToExcel()
        {
            // Select a path
            string ExportedPath = string.Empty;
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "Choosing a Location";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ExportedPath = dialog.FileName;
            }

            // Export data
            /*
             * Tutorial Sources: https://suresh-kamrushi.medium.com/generating-and-formatting-excelsheet-in-c-using-epplus-d3800cfbd3d4
             */

            if (ExportedPath == string.Empty)
                return;

            ExportGlobalSchedule(ExportedPath);
            // ExportStudentsSchedule(ExportedPath);

        }

        private void ExportGlobalSchedule(string ExportedPath)
        {
            string fileName = "/Global_Schedule";
            string extension = ".xlsx";
            string fullPath = ExportedPath + fileName + extension;

            // Check whether the path existed
            int fileIdx = 1;
            while (File.Exists(fullPath))
            {
                fullPath = ExportedPath + fileName + $"({fileIdx})" + extension;
                fileIdx++;
            }

            // Create the file
            var file = new FileInfo(fullPath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage(file);
            var sheetCreate = excel.Workbook.Worksheets.Add("Sheet1");

            string[] weekdays = { "Mon", "Tue", "Wed", "Thu", "Fri" };
            string[] timePeriods = { "8:00 - 8:40", "8:50 - 9:30", "9:40 - 10:20", "10:30 - 11:10", "11:10 - 11:50", "13:00 - 13:40", "13:50 - 14:35", "14:45 - 15:25", "15:35 - 16:15" };

            // Generate the excel timetable
            int lastMaxRow = 2; // to record where to fill the next weekday slot
            int lastRow = 2; // to record where to refresh as filling the next slot in the same weekday
            int col = 2;
            int row = lastMaxRow;
            const int firstCol = 2;
            int weekday = 0;
            bool bgColorSwitch = true;

            // Set the title of time period
            for (int i = 0; i < timePeriods.Length; i++)
                sheetCreate.Cells[1, 2 + i].Value = timePeriods[i];

            // Fill the schedule info
            foreach (var slot in repositoryService.Schedule.Slots)
            {
                List<string> fills = new List<string>();
                foreach (var scheduledClass in slot.ScheduledClasses)
                    fills.Add(scheduledClass.Course.ClassCode + $" ({scheduledClass.Course.Teacher.Name}, {scheduledClass.RoomId})");

                lastMaxRow = Math.Max(lastMaxRow, row + fills.Count);

                for (int i = 0; i < fills.Count; i++)
                {
                    sheetCreate.Cells[row + i, col].Value = fills[i];
                }

                // Set the wire
                //var cells =  sheetCreate.Cells[lastRow, col, row + fills.Count - 1, col];
                //cells.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                row = lastRow;
                col++;

                if (slot.SlotId % 9 == 0)
                {
                    // Fill the weekday
                    sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Merge = true;

                    if (weekday > 4)
                    {
                        System.Diagnostics.Debug.WriteLine("Date out of range when formulating excel");
                        return;
                    }

                    sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Value = weekdays[weekday++];
                    sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Style.Font.Bold = true;
                    sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    // Set the background #FCE4D6
                    sheetCreate.Cells[lastRow, firstCol, lastMaxRow - 1, 10].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    if (bgColorSwitch)
                        sheetCreate.Cells[lastRow, firstCol, lastMaxRow - 1, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ffff00"));
                    else
                        sheetCreate.Cells[lastRow, firstCol, lastMaxRow - 1, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#fce4d6"));
                    bgColorSwitch = !bgColorSwitch;

                    // update the index
                    row = lastMaxRow;
                    lastRow = row;
                    col = firstCol;
                }
            }

            sheetCreate.Cells.AutoFitColumns();
            excel.Save();
        }

        private void ExportStudentsSchedule(string ExportedPath)
        {
            string fileName = "/Students_Schedule";
            string extension = ".xlsx";
            string fullPath = ExportedPath + fileName + extension;

            // Check whether the path existed
            int fileIdx = 1;
            while (File.Exists(fullPath))
            {
                fullPath = ExportedPath + fileName + $"({fileIdx})" + extension;
                fileIdx++;
            }

            // Create the file
            var file = new FileInfo(fullPath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage(file);
            foreach (var std in repositoryService.Students)
            {
                var sheetCreate = excel.Workbook.Worksheets.Add(std.Name + $" ({std.Id})");

                string[] weekdays = { "Mon", "Tue", "Wed", "Thu", "Fri" };
                string[] timePeriods = { "8:00 - 8:40", "8:50 - 9:30", "9:40 - 10:20", "10:30 - 11:10", "11:10 - 11:50", "13:00 - 13:40", "13:50 - 14:35", "14:45 - 15:25", "15:35 - 16:15" };

                // Generate the excel timetable
                int lastMaxRow = 2; // to record where to fill the next weekday slot
                int lastRow = 2; // to record where to refresh as filling the next slot in the same weekday
                int col = 2;
                int row = lastMaxRow;
                const int firstCol = 2;
                int weekday = 0;
                bool bgColorSwitch = true;

                // Set the title of time period
                for (int i = 0; i < timePeriods.Length; i++)
                    sheetCreate.Cells[1, 2 + i].Value = timePeriods[i];

                // Fill the schedule info
                foreach (var slot in std.StudentSchedule.Slots)
                {
                    List<string> fills = new List<string>();
                    foreach (var scheduledClass in slot.ScheduledClasses)
                        fills.Add(scheduledClass.Course.ClassCode + $" ({scheduledClass.Course.Teacher.Name}, {scheduledClass.RoomId})");

                    lastMaxRow = Math.Max(lastMaxRow, row + fills.Count);

                    for (int i = 0; i < fills.Count; i++)
                    {
                        sheetCreate.Cells[row + i, col].Value = fills[i];
                    }

                    // Set the wire
                    //var cells =  sheetCreate.Cells[lastRow, col, row + fills.Count - 1, col];
                    //cells.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    row = lastRow;
                    col++;

                    if (slot.SlotId % 9 == 0)
                    {
                        // Fill the weekday
                        sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Merge = true;

                        if (weekday > 4)
                        {
                            System.Diagnostics.Debug.WriteLine("Date out of range when formulating excel");
                            return;
                        }

                        sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Value = weekdays[weekday++];
                        sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Style.Font.Bold = true;
                        sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        sheetCreate.Cells[lastRow, 1, lastMaxRow - 1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        // Set the background #FCE4D6
                        sheetCreate.Cells[lastRow, firstCol, lastMaxRow - 1, 10].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        if (bgColorSwitch)
                            sheetCreate.Cells[lastRow, firstCol, lastMaxRow - 1, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ffff00"));
                        else
                            sheetCreate.Cells[lastRow, firstCol, lastMaxRow - 1, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#fce4d6"));
                        bgColorSwitch = !bgColorSwitch;

                        // update the index
                        row = lastMaxRow;
                        lastRow = row;
                        col = firstCol;
                    }
                }
                sheetCreate.Cells.AutoFitColumns();
            }
            
            excel.Save();
        }

        private void ImportData()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Customized File|*.xlsx|*.xls|Excel Workbook";
            if (openFileDialog.ShowDialog() == true)
            {
                ImportedDocPath = openFileDialog.FileName;
            }
        }

        public void OnSave()
        {
            settingService.RegisterData("SavableObject", SavedData);
            settingService.RegisterData("ImportDataPath", ImportedDocPath);
            settingService.RegisterData("ClassDivisionBaseline", ClassDivisionBaseline);
            settingService.RegisterData("PopulationSize", PopulationSize);
            settingService.RegisterData("MutationProb", MutationProb);

            naSlotsList = new List<int>();
            if (UnavailableTimeSlots != null && UnavailableTimeSlots != string.Empty) 
            {
                string[] slots = UnavailableTimeSlots.Split(',');
                foreach (string slot in slots)
                {
                    if (int.TryParse(slot, out int res))
                        naSlotsList.Add(res);
                    else
                        System.Diagnostics.Debug.WriteLine("Error in converting string to int: " + slot);
                }
            }
            settingService.RegisterData("GlobalUnavailableTimeSots", naSlotsList);
        }

        public void Reset(UserSetting userSetting)
        {
            PopulationSize = userSetting.PopulationSize;
            MutationProb = userSetting.MutationProb;
            ClassDivisionBaseline = userSetting.ClassDivisionBaseline;
        }
    }
}
