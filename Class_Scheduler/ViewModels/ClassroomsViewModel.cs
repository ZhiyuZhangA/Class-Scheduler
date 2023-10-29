using Class_Scheduler.Common.Models;
using Class_Scheduler.Extensions;
using Class_Scheduler.Service;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;

namespace Class_Scheduler.ViewModels
{
    public class ClassroomsViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly IRepositoryService repositoryService;

        public IRepositoryService RepositoryService { get { return repositoryService; } }

        public DelegateCommand AddRoomCommand { get; private set; }

        public DelegateCommand<Room> ModifyRoomCommand { get; private set; }

        public DelegateCommand<Room> DeleteRoomCommand { get; private set; }

        public DelegateCommand SelectAllCommand { get; private set; }

        public DelegateCommand DeleteSelectedCommand { get; private set; }

        public ClassroomsViewModel(IDialogService dialogService, IRepositoryService repositoryService) 
        {
            this.dialogService = dialogService;
            this.repositoryService = repositoryService;

            AddRoomCommand = new DelegateCommand(AddRoom);
            ModifyRoomCommand = new DelegateCommand<Room>(ModifyRoom);
            DeleteRoomCommand = new DelegateCommand<Room>(DeleteRoom);
            SelectAllCommand = new DelegateCommand(SelectAllClassrooms);
            DeleteSelectedCommand = new DelegateCommand(DeleteSelected);
        }

        private bool isAllSelected;
        public bool IsAllSelected
        {
            get { return isAllSelected; }
            set
            {
                isAllSelected = value;
                RaisePropertyChanged();
            }
        }

        private void SelectAllClassrooms()
        {
            foreach (var room in repositoryService.Rooms)
            {
                room.IsSelected = IsAllSelected;
            }
        }

        private void DeleteSelected()
        {
            List<Room> indices = new List<Room>();
            foreach (var room in repositoryService.Rooms)
            {
                if (room.IsSelected)
                    indices.Add(room);
            }

            foreach (var item in indices)
                repositoryService.Rooms.Remove(item);

            // Update the number
            repositoryService.RoomsCount = repositoryService.Rooms.Count;
            IsAllSelected = false;
        }

        private void AddRoom()
        {
            dialogService.ShowDialog(PrismManager.AddRoomDialogName, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    var room = callback.Parameters.GetValue<Room>("Room");
                    if (repositoryService.RoomsCount > 0)
                        room.Id = repositoryService.Rooms[repositoryService.Rooms.Count - 1].Id + 1;
                    else
                        room.Id = 0;
                    repositoryService.Rooms.Add(room);
                    repositoryService.RoomsCount = repositoryService.Rooms.Count;
                }
            });
        }

        private void DeleteRoom(Room room)
        {
            DialogParameters keys = new DialogParameters();
            keys.Add("ShownData", "Are you sure you want to delete this room?\r\n");
            dialogService.ShowDialog(PrismManager.DeleteMemberDialogName, keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    // Delete Members
                    if (!repositoryService.Rooms.Remove(room))
                    {
                        // 日志输出
                    }
                    else
                    {
                        repositoryService.RoomsCount = repositoryService.Rooms.Count;
                    }
                }
            });
        }

        private void ModifyRoom(Room room)
        {
            DialogParameters keys = new DialogParameters
            {
                { "Room", room }
            };
            dialogService.ShowDialog(PrismManager.ModifyRoomDialogName, keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    var tmp = callback.Parameters.GetValue<Room>("Room");
                    repositoryService.Rooms[room.Id] = tmp;
                }
            });
        }
    }
}
