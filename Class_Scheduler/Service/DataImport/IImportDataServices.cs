using Class_Scheduler.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.Service.DataImport
{
    public interface IImportDataServices
    {
        void CreateImportContext(IDataImporter importer);
        void ImportData(bool import=true);

        SavableObject GetSavableObject();

        string GetPath();
        void SetPath(string path);
    }
}
