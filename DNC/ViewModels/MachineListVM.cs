using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;
using DNC.Models;
using DNC.Properties;
using DNC.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;

namespace DNC.ViewModels
{
    public class MachineListVM : ViewModelBase
    {
        public SerializedList<ModelBase> MachineList { get; set; }

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

        public ICommand AddFolderCommand => new RelayCommand(() =>
        {
            MachineList.Add(new Folder("Folder1"));
        });

        public ICommand AddMachineCommand => new RelayCommand(() =>
        {
            ModelBase m;
            int i = 0;
            do
            {
                m = new Machine($"Machine{i++}");
            } while (MachineList.Contains(m));

            MachineList.Add(m);

            EditMachineProperties e = new EditMachineProperties();
            e.EditMachine(m);
        });

        public ICommand SendProgram { get; private set; }

        public MachineListVM()
        {
            MachineList = new SerializedList<ModelBase>();
            Messenger.Default.Register<SerializedListAction>(MachineList, MachineList.DoSerializedListAction);
        }
    }
}
