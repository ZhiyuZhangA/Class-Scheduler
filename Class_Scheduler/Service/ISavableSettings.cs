using Class_Scheduler.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.ViewModels.Settings
{
    public interface ISavableSettings
    {
        void OnSave();

        void Reset(UserSetting userSetting);
    }
}
