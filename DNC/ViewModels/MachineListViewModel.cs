using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DNC.Models;
using DNC.Properties;
using DNC.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;

namespace DNC.ViewModels
{
    public class MachineListViewModel : ViewModelBase
    {
        public ObservableCollection<ModelBase> MachineList
        {
            get => Settings.Default.EnumeratedList;
            private set
            {
                Settings.Default.EnumeratedList = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        private ModelBase _selectedItem;
        public ModelBase SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        private Program _selectedProgram;
        public Program SelectedProgram
        {
            get => _selectedProgram;
            set
            {
                _selectedProgram = value;
                RaisePropertyChanged();
            }
        }

        public ICommand AddFolderCommand { get; private set; }
        public ICommand AddMachineCommand { get; private set; }
        public ICommand SendProgram { get; private set; }

        public MachineListViewModel()
        {
            AddFolderCommand = new RelayCommand(() => MachineList.Add(new Folder("Folder1", MachineList)));
            AddMachineCommand = new RelayCommand(() =>
            {
                Machine m;
                int i = 0;
                do
                {
                    m = new Machine($"Machine{i++}", MachineList);
                } while (MachineList.Contains(m));

                MachineList.Add(m);

                EditPrompt e = new EditPrompt();
                e.EditMachine(m);
            });

            if (MachineList == null)
                Settings.Default.EnumeratedList = new ObservableCollection<ModelBase>();           

            AddTestItems();
        }

        public void AddTestItems()
        {
            var Machine = new Machine("MAM", MachineList, new TCPConnection()
            {
                IPAddress = IPAddress.Parse("192.168.128.63"),
                Port = 8193
            });

            MachineList.Add(Machine);


            var Machine1 = new Machine("MakinoC", MachineList)
            {
                Connection = new SerialConnection()
                {
                    SerialPort = new SerialPort("COM1")
                    
                }
            };

            MachineList.Add(Machine1);
        }
    }
}
