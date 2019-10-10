using DNC.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DNC.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public WindowStatus WindowStatus { get; set; }
        public readonly MachineListViewModel MachineListViewModel;

        private int progressBarValue;
        public int ProgressBarValue
        {
            get => progressBarValue;
            set
            {
                progressBarValue = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            MachineListViewModel = new MachineListViewModel();

            WindowStatus = new WindowStatus();

            Messenger.Default.Register<GenericMessage<int>>(this, ChangeProgressValue);
            Messenger.Default.Register<NotificationMessage<string>>(this, CreateNotification);
        }

        public void ChangeProgressValue(GenericMessage<int> gMessage)
        {
            ProgressBarValue = gMessage.Content;
        }

        public void CreateNotification(NotificationMessage<string> msg)
        {
            MessageBox.Show(msg.Content, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }




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
