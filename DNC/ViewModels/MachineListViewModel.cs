using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DNC.Models;

namespace DNC.ViewModels
{
    public class MachineListViewModel
    {
        public ObservableCollection<ModelBase> EnumeratedList { get; set; }

        public MachineListViewModel()
        {
            EnumeratedList = new ObservableCollection<ModelBase>();


            EnumeratedList.Add(new ModelBase("Folder1", ModelType.Folder));
            EnumeratedList.Add(new ModelBase("Machine1", ModelType.Machine));
            EnumeratedList[0].Children.Add(new ModelBase("Machine2", ModelType.Machine));

            

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
                        p => AddFolder());
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
                        p => AddMachine());
                }
                return _addMachineCommand;
            }
        }

      


        public void AddMachine()
        {

        }


        public void AddFolder()
        {

        }





    }


}
