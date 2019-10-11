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
using System.Windows.Input;
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
    public class MachineListViewModel : ViewModelBase
    {

        private const string SETTINGS_DIR = "Settings.bin";
        private ObservableCollection<ModelBase> DeserializeList()
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

        private void SerializeList(ObservableCollection<ModelBase> list)
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

        #region Individual Item Commands
        public ICommand RenameCommand { get; private set; }
        public ICommand ToggleConnection { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Import { get; private set; }
        #endregion
        public MachineListViewModel()
        {
            AddFolderCommand = new RelayCommand(() =>
            {
                MachineList.Add(new Folder("Folder1", null));
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
            });

            #region Individual Item Command Implementation

            RenameCommand = new RelayCommand(() =>
            {
                SelectedItem.IsNameEditing = !SelectedItem.IsNameEditing;
                SelectedItem.RaisePropertyChanged("IsNameEditing");
            });

            ToggleConnection = new RelayCommand(() =>
            {
                if (!(SelectedItem is Machine sMachine)) throw new NotImplementedException();
                Task.Factory.StartNew(() =>
                {
                    sMachine.ToggleConnection();
                });
            });

            Edit = new RelayCommand(() =>
            {
                if (!(SelectedItem is Machine sMachine)) throw new NotImplementedException();
                EditPrompt e = new EditPrompt();
                e.EditMachine(sMachine);
            });

            Import = new RelayCommand(() =>
            {
                if (!(SelectedItem is Machine sMachine)) throw new NotImplementedException();
                System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog
                {
                    RestoreDirectory = true
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string[] fileData = File.ReadAllLines(dialog.FileName);
                    sMachine.ProgramList.Add(new Program(dialog.FileName, fileData));
                }
            });
            #endregion

            MachineList = DeserializeList();
            //if (MachineList == null) MachineList = new ObservableCollection<ModelBase>();

           
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

        public void AddTestItems()
        {
            var Machine = new Machine("MAM", null, new TCPConnection(IPAddress.Parse("192.168.128.63"), 8193));

            MachineList.Add(Machine);


            var Machine1 = new Machine("MakinoC", null)
            {
                Connection = new SerialConnection()
                {
                    _SerialPort = "COM1"

                }
            };

            MachineList.Add(Machine1);
        }
    }
}
