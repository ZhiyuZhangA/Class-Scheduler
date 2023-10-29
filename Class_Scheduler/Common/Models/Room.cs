using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.Common.Models
{
    public class Room : BindableBase
    {
        private int id;
        
        public int Id 
        { 
            get { return id; } 
            set { id = value; }
        }

        private int name;

        public int Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }

        private string purpose;

        public string Purpose
        {
            get { return purpose; }
            set { purpose = value; }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged(); }
        }

        public Room Copy()
        {
            Room room = new Room();
            room.Name = Name;
            room.Id = Id;
            room.Purpose = Purpose;
            return room;
        }
    }
}
