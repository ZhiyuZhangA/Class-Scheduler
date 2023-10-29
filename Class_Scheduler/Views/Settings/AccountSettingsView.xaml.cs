using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Class_Scheduler.Views.Settings
{
    /// <summary>
    /// AccountSettingsView.xaml 的交互逻辑
    /// </summary>
    public partial class AccountSettingsView : UserControl
    {
        public AccountSettingsView()
        {
            InitializeComponent();
        }

        private void User_Img_MouseEnter(object sender, MouseEventArgs e)
        {
            pop.IsOpen = true;

        }

        private void User_Img_MouseLeave(object sender, MouseEventArgs e)
        {
            pop.IsOpen = false;
        }

        private void User_Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
