using DNC.Models;
using DNC.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for CreatePrompt.xaml
    /// </summary>
    public partial class EditPrompt : Window
    {
        public EditPromptViewModel ViewModel;

        public Dictionary<ConnectionType, UserControl> AvailableTypes;

        public EditPrompt()
        {
            InitializeComponent();
        }

        public void EditMachine(Machine cMachine)
        {
            DataContext = ViewModel = new EditPromptViewModel(cMachine);
            AvailableTypes = new Dictionary<ConnectionType, UserControl>
            {
                { ConnectionType.TCP, TryFindResource("typeTCP") as UserControl },
                { ConnectionType.Serial, TryFindResource("typeSerial") as UserControl }
            };

            ViewModel.SaveCommand = new RelayCommand(() => DialogResult = true);
            ViewModel.CancelCommand = new RelayCommand(() => DialogResult = false);

            bool? dr = ShowDialog();

            if (!dr ?? false)
            {
                //ViewModel.CurrentMachine = input; // idk how to do this yet
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<ConnectionType, UserControl> pair in AvailableTypes)
                if (pair.Key == ViewModel.CurrentMachine.CurrentConnectionType)
                    ViewModel.CurrentControl = pair.Value;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtName.SelectAll();
            txtName.Focus();
        }
    }

    public class EditPromptViewModel : ViewModelBase
    {
        public ICommand TestConnection { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public Machine CurrentMachine { get; set; }

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

        
        public EditPromptViewModel(Machine cMachine)
        {
            TestConnection = new RelayCommand(() => 
            {

            });
            
            CurrentMachine = cMachine;
            
        }
    }
}
