using Class_Scheduler.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.Service.DataImport
{
    public class DataImportContext
    {
        private IDataImporter importer;
        private string path;
        private SavableObject savableObject;

        public string Path => path;
        public SavableObject SavableObject => savableObject;


        public DataImportContext(IDataImporter importer)
        {
            this.importer = importer;
        }

        public void Import(bool import=true)
        {
            if (import)
                path = importer.Import();

            savableObject = importer.Serialize();   
        }

        public void SetPath(string path)
        {
            importer.ImportDocPath = path;
        }
    }
}
