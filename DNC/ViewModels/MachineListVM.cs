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

        private const string SETTINGS_DIR = "Settings.bin";
        private static ObservableCollection<ModelBase> DeserializeList()
        {
            Debug.WriteLine("Reading List...");
            try
            {

                using (FileStream fs = new FileStream(SETTINGS_DIR, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                {
                    IFormatter formatter = new BinaryFormatter();
                    return (formatter.Deserialize(fs) as ObservableCollection<ModelBase>) ?? new ObservableCollection<ModelBase>();
                }
                
            }
            catch (SerializationException e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private void SerializeList(Collection<ModelBase> list)
        {

            Debug.WriteLine("Saving List...");
            using (FileStream fs = new FileStream(SETTINGS_DIR, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, list);
            }
            
        }


        private ObservableCollection<ModelBase> _machineList;
        public ObservableCollection<ModelBase> MachineList
        {
            get => _machineList;
            set
            {
                _machineList = value;

                Task.Factory.StartNew(() =>
                {
                    SerializeList(MachineList);
                });
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

        public MachineListVM()
        {

            MachineList = DeserializeList() ?? new ObservableCollection<ModelBase>();

            AddFolderCommand = new RelayCommand(() =>
            {
                MachineList.Add(new Folder("Folder1"));
                SerializeList(MachineList);
            });

            AddMachineCommand = new RelayCommand(() =>
            {
                Machine m;
                int i = 0;
                do
                {
                    m = new Machine($"Machine{i++}", null);
                } while (MachineList.Contains(m));

                MachineList.Add(m);

                EditPrompt e = new EditPrompt();
                e.EditMachine(m);
                SerializeList(MachineList);
            });
        }



        private void MachineList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                SerializeList(MachineList);
            });

        }

        public bool CanCopyToClipboard => true;
        public bool CanCutToClipboard => true;
        public bool CanPasteFromClipboard => true;

        public void CopyToClipboard()
        {
            throw new NotImplementedException();
        }

        public void CutToClipboard()
        {
            throw new NotImplementedException();
        }

        public void PasteFromClipboard()
        {
            throw new NotImplementedException();
        }
    }
}
