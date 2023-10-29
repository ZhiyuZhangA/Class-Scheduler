using Class_Scheduler.Common.Models;

namespace Class_Scheduler.Service.DataImport
{
    /// <summary>
    /// Import global data as excel
    /// </summary>
    public class ExcelGlobalDataImporter : IDataImporter
    {
        private string importDocPath = "";
        public string ImportDocPath { get => importDocPath; set => importDocPath = value; }

        public string Import()
        {
            return "";
        }

        public SavableObject Serialize()
        {
            return null;
        }
    }
}
