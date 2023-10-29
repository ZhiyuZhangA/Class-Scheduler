using Class_Scheduler.Extensions;
using Prism.Regions;
using System.Windows;
using System.Windows.Controls;

namespace Class_Scheduler.Views.Settings
{
    /// <summary>
    /// SettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingView : UserControl
    {
        public SettingView(IRegionManager regionManager)
        {   
            RegionManager.SetRegionManager(this, regionManager);
            InitializeComponent();
        }
    }
}
