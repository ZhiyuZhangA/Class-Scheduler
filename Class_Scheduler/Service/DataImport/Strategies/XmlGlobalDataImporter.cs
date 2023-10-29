using Class_Scheduler.Common.Models;
using Class_Scheduler.Extensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.Service.DataImport
{
    /// <summary>
    /// Import global data as xml
    /// </summary>
    public class XmlGlobalDataImporter : IDataImporter
    {
        private string importDocPath = "";
        public string ImportDocPath { get => importDocPath; set => importDocPath = value; }

        public string Import()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml File|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                importDocPath = openFileDialog.FileName;
            }

            return importDocPath;
        }

        public SavableObject Serialize()
        {
            SavableObject savableObject = XmlSerializeManager.XmlDeserializer<SavableObject>(importDocPath);
            return savableObject;
        }
    }
}
