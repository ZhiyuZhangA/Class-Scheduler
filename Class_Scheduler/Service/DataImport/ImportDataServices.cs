using Class_Scheduler.Common.Models;
using Class_Scheduler.Service.DataImport;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.Service
{
    public class ImportDataServices : BindableBase, IImportDataServices
    {
        private DataImportContext dataImportContext;

        public ImportDataServices() 
        {
            
        }

        public void CreateImportContext(IDataImporter importer)
        {
            dataImportContext = new DataImportContext(importer);
        }

        public string GetPath()
        {
            return dataImportContext.Path;
        }

        public void SetPath(string path)
        {
            dataImportContext.SetPath(path);
        }

        public SavableObject GetSavableObject()
        {
            return dataImportContext.SavableObject;
        }

        /// <summary>
        /// Import the data of xml or excel
        /// If you don't want to open the file explorer dialog, set import to be false, but set the path first using method SetPath(string path)
        /// </summary>
        /// <param name="import"></param>
        public void ImportData(bool import=true)
        {
            if (dataImportContext == null)
            {
                System.Diagnostics.Debug.WriteLine("Haven't create an import context");
                return;
            }

            dataImportContext.Import(import);
        }
    }
}
