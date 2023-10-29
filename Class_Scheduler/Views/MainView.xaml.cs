using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Class_Scheduler.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //WindowInteropHelper helper = new WindowInteropHelper(this);
            //SendMessage(helper.Handle, 161, 2, 0);

            if (e.ClickCount == 2)
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            }
        }

        private void Setting_Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            pop_setting.IsOpen = true;
        }

        private void Setting_Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            pop_setting.IsOpen = false;
        }

        private void Save_Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            pop_save.IsOpen = true;
        }

        private void Save_Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            pop_save.IsOpen= false;
        }

        private void SaveTo_Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            pop_saveTo.IsOpen = true;
        }

        private void SaveTo_Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            pop_saveTo.IsOpen = false;
        }

        private void OpenFile_Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            pop_open.IsOpen = true;
        }

        private void OpenFile_Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            pop_open.IsOpen = false;
        }
    }
}
