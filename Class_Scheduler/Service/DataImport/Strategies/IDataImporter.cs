using Class_Scheduler.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.Service.DataImport
{
    /// <summary>
    /// Interface of DataImportStrategies
    /// </summary>
    public interface IDataImporter
    {
        string Import();
        SavableObject Serialize();
        
        string ImportDocPath { get; set; }
    }
}
