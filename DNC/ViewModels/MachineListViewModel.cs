using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace DNC.ViewModels
{
    public class MachineListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ModelBase> EnumeratedList { get; set; }

        public ICommand AddFolderCommand { get; private set; }
        public ICommand AddMachineCommand { get; private set; }
        public ICommand SendProgram { get; private set; }

        public MachineListViewModel()
        {
            EnumeratedList = new ObservableCollection<ModelBase>();

            AddFolderCommand = new RelayCommand(() => EnumeratedList.Add(new Folder("Folder1")));
            AddMachineCommand = new RelayCommand(() =>
            {
                Machine m;
                int i = 0;
                do
                {
                    m = new Machine($"Machine{i++}");
                } while (EnumeratedList.Contains(m));

                EnumeratedList.Add(m);

                EditPrompt e = new EditPrompt();
                e.EditMachine(m);
            });

            SendProgram = new RelayCommand(() =>
            {
                if (SelectedItem is Machine machine)
                {
                    machine.PushProgram(SelectedProgram);
                }
            });


        }

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

        private Program _selectedProgram;
        public Program SelectedProgram
        {
            get => _selectedProgram;
            set
            {
                _selectedProgram = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    } 
}
