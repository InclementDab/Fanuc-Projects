using DNC.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
     
        public ConnectionSetupViewModel ViewModel { get; private set; }
        public ConnectionSetup()
        {
            InitializeComponent();
        }

        public void EditConnection(Machine currentMachine)
        {
            DataContext = ViewModel = new ConnectionSetupViewModel(currentMachine);
            ViewModel.AvailableTypes = new Dictionary<ConnectionType, UserControl>
            {
                { ConnectionType.TCP, TryFindResource("typeTCP") as UserControl },
                { ConnectionType.Serial, TryFindResource("typeSerial") as UserControl }
            };

            bool? dr = ShowDialog();

            if (!dr ?? false)
            {
                // idk how to do this yet, but its the cancel command
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<ConnectionType, UserControl> pair in ViewModel.AvailableTypes)
                if (pair.Key == ViewModel.CurrentMachine.CurrentConnectionType)
                    ViewModel.CurrentControl = pair.Value;   
        }
    }

    public class ConnectionSetupViewModel : ViewModelBase
    {
        public Machine CurrentMachine { get; set; }
        public ICommand CloseCommand { get; private set; }

        private UserControl _currentControl;
        public UserControl CurrentControl
        {
            get => _currentControl;
            set
            {
                _currentControl = value;
                RaisePropertyChanged();
            }
        }

        public ConnectionSetupViewModel(Machine currentMachine)
        {
            
            CurrentMachine = currentMachine;
            CloseCommand = new RelayCommand(() =>
            {
                
            });
        }

        private Dictionary<ConnectionType, UserControl> _availableTypes;
        public Dictionary<ConnectionType, UserControl> AvailableTypes
        {
            get => _availableTypes;
            set
            {
                _availableTypes = value;
                RaisePropertyChanged();
            }
        }


    }
}
