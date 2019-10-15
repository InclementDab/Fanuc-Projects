using DNC.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DNC.ViewModels
{

    public class SoftwareUpdateMessage 
    {
        public Action<string> Callback { get; set; }

        public SoftwareUpdateMessage(Action<string> callback)
        {
            Callback = callback;
        }
        
    }
    public class MainWindowVM : ViewModelBase
    {
        public WindowStatus WindowStatus { get; set; }
        public UserControl MachineListView { get; set; } = new MachineListView();
        public UserControl ProgramListView { get; set; } = new ProgramListView();

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

        public MainWindowVM()
        {
            WindowStatus = new WindowStatus();

            Messenger.Default.Register<GenericMessage<int>>(this, ChangeProgressValue);
            Messenger.Default.Register<NotificationMessage<string>>(this, CreateNotification);
            Messenger.Default.Register<SoftwareUpdateMessage>(this, UpdateSoftwareHandler);


        }

        public void ChangeProgressValue(GenericMessage<int> gMessage)
        {
            ProgressBarValue = gMessage.Content;
        }

        public void CreateNotification(NotificationMessage<string> msg)
        {
            MessageBox.Show(msg.Content, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void UpdateSoftwareHandler(SoftwareUpdateMessage updateMessage)
        {
            
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
