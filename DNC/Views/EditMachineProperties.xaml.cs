using DNC.Models;
using DNC.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DNC.Communication;
using GalaSoft.MvvmLight.Messaging;
using UserControl = System.Windows.Controls.UserControl;

namespace DNC.Views
{
    /// <summary>
    /// Interaction logic for CreatePrompt.xaml
    /// </summary>
    public partial class EditMachineProperties : Window
    {
        public EditMachinePropertiesVM ViewModel;

        public Dictionary<ConnectionType, UserControl> AvailableTypes;

        public EditMachineProperties()
        {
            InitializeComponent();
        }
        
        public void EditMachine(ModelBase machineToEdit)
        {
            DataContext = ViewModel = new EditMachinePropertiesVM(machineToEdit, this);
            
            AvailableTypes = new Dictionary<ConnectionType, UserControl>
            {
                { ConnectionType.TCP, TryFindResource("TypeTCP") as UserControl },
                { ConnectionType.Serial, TryFindResource("TypeSerial") as UserControl }
            };

            ViewModel.CurrentMachine.BeginEdit();
            bool? dr = ShowDialog();
            if (!dr ?? false)
            {
                
            }
            else
            {
                
            }
        }
    }

    public class EditMachinePropertiesVM : ViewModelBase
    {
        private ConnectionType _currentConnectionType;

        public ConnectionType CurrentConnectionType
        {
            get => _currentConnectionType;
            set
            {
                _currentConnectionType = value;
                switch (_currentConnectionType)
                {
                    case ConnectionType.Serial:
                        CurrentMachine.Data.Connection.BaseConnection = new TCPConnection();
                        break;
                    
                    case ConnectionType.TCP:
                        CurrentMachine.Data.Connection.BaseConnection = new SerialConnection();
                        break;
                
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                RaisePropertyChanged("Connection");
            }
        }

        public ICommand TestConnection => new RelayCommand(() =>
        {

        });

        public ICommand SaveCommand => new RelayCommand(() =>
        {
            Messenger.Default.Send(new SerializedListAction(ListAction.Save));
            ParentControl.DialogResult = true;
        });
        public ICommand CancelCommand => new RelayCommand(() =>
        {
            Messenger.Default.Send(new SerializedListAction(ListAction.Load));
            ParentControl.DialogResult = false;
        });

        public EditMachineProperties ParentControl { get; set; }
        public Machine CurrentMachine { get; set; }
        public Dictionary<string, UserControl> ListViewItems { get; set; }

        private KeyValuePair<string, UserControl> _selectedItem;
        public KeyValuePair<string, UserControl> SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        public EditMachinePropertiesVM(ModelBase mBase, EditMachineProperties control)
        {
            CurrentMachine = mBase as Machine;
            ParentControl = control;

            ListViewItems = new Dictionary<string, UserControl>
            {
                {"General",  (UserControl)control.TryFindResource("General")},
                {"Connection",  (UserControl)control.TryFindResource("Connection")}
            };

        }
    }
}
