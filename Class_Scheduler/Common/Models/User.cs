using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Class_Scheduler.Common.Models
{
    public class User : BindableBase
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string SchoolName { get; set; }

        public string Email { get; set; }

        public string ImageSource { get; set; }

        private BitmapImage image;
        public BitmapImage BitmapImage
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }
    }
}
