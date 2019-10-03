using DNC.Views;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DNC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private WindowStatus _windowStatus;
        public WindowStatus WindowStatus
        {
            get => _windowStatus;
            set
            {
                _windowStatus = value;
                NotifyPropertyChanged();
            }
        }

        private UserControl _machineListView;
        public UserControl MachineListView
        {
            get => _machineListView;
            set
            {
                _machineListView = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MachineListView = new MachineListView();
            WindowStatus = new WindowStatus();

            
        }

        public double ProgressBar => stProgressBar.Value;

        public void UpdateStatus()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }

    public class WindowStatus : TextBlock
    {

        public WindowStatus()
        {
            Text = "Test";
        }

        public WindowStatus(Status status)
        {
            switch (status)
            {
                case Status.Error:
                
                    
                    break;
                
            }
        }

        

        public enum Status
        {
            Error = 0,
            Caution = 1,
            Completed = 2
        }

        
    }
}
