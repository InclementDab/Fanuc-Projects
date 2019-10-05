using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DNC.Models;
using DNC.Views;

namespace DNC.ViewModels
{
    public class MachineListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ModelBase> EnumeratedList { get; set; }

        private ModelBase _selectedItem;
        public ModelBase SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged();
            }
        }

        public MachineListViewModel()
        {
            EnumeratedList = new ObservableCollection<ModelBase>();            
        }

        private ICommand _addFolderCommand;
        public ICommand AddFolderCommand
        {
            get
            {
                if (_addFolderCommand == null)
                {
                    _addFolderCommand = new RelayCommand(
                        p => true,
                        p => AddListItem("Folder1", ModelType.Folder));
                }
                return _addFolderCommand;
            }
        }

        private ICommand _addMachineCommand;
        public ICommand AddMachineCommand
        {
            get
            {
                if (_addMachineCommand == null)
                {
                    _addMachineCommand = new RelayCommand(
                        p => true,
                        p =>
                        {
                            EditPrompt ep = new EditPrompt();
                            ModelBase mBase = ep.CreateDialog();

                            if (mBase != null)
                                EnumeratedList.Add(mBase);
                        });
                }
                return _addMachineCommand;
            }
        }




        public void AddListItem(string name, ModelType type)
        {
            EnumeratedList.Add(new ModelBase(name, type, EnumeratedList));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
