using DNC.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DNC.Views
{
    /// <summary>
    /// Interaction logic for ConnectionSetup.xaml
    /// </summary>
    /// 
    public partial class ConnectionSetup : Window
    {
     
        public readonly ConnectionSetupViewModel ViewModel;
        public ConnectionSetup(Machine currentMachine)
        {
            InitializeComponent();

            DataContext = ViewModel = new ConnectionSetupViewModel(currentMachine);
            ViewModel.AvailableTypes = new Dictionary<string, UserControl>
            {
                { "TCP/IP", TryFindResource("typeTCP") as UserControl },
                { "Serial Port", TryFindResource("typeSerial") as UserControl }
            };
        }
    }

    public class ConnectionSetupViewModel : INotifyPropertyChanged
    {
        public Machine CurrentMachine { get; set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public ConnectionSetupViewModel(Machine currentMachine)
        {
            CurrentMachine = currentMachine;

            SaveCommand = new RelayCommand(() =>
            {
                switch (SelectedItem.Key)
                {
                    case "TCP/IP":
                        CurrentMachine.Connection.Type = ConnectionType.TCP;
                        break;
                    case "Serial Port":
                        CurrentMachine.Connection.Type = ConnectionType.Serial;
                        break;
                }
            }); // todo: still dont know how to do this

            CancelCommand = new RelayCommand(() => { });
        }

        private KeyValuePair<string, UserControl> _selectedItem;
        public KeyValuePair<string, UserControl> SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged();
            }
        }

        private Dictionary<string, UserControl> _availableTypes;
        public Dictionary<string, UserControl> AvailableTypes
        {
            get => _availableTypes;
            set
            {
                _availableTypes = value;
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
