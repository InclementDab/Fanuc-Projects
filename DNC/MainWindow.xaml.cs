using DNC.Models;
using DNC.ViewModels;
using DNC.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
using System.Xaml;
using System.Xml.Serialization;

namespace DNC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public readonly MainWindowViewModel ViewModel;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel = new MainWindowViewModel();
        }


        #region copypaste

        public ModelBase SelectedItem => ((DataContext as MainWindowViewModel).MachineListView.DataContext as MachineListViewModel).SelectedItem;
        private IFormatter Formatter;

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.Clear();

            Formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            Formatter.Serialize(ms, SelectedItem);
            Clipboard.SetData("MemoryStream", ms); // DO NOT TOUCH IT WILL BREAK

            if (SelectedItem.Parent is Folder folder)
            {
                folder.Children.Remove(SelectedItem);
            }
            else if (SelectedItem.Parent == null)
            {
                (ViewModel.MachineListView.DataContext as MachineListViewModel).MachineList.Remove(SelectedItem);
            }
            
        }


        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.Clear();
            Formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            Formatter.Serialize(ms, SelectedItem);
            Clipboard.SetData("MemoryStream", ms); // DO NOT TOUCH IT WILL BREAK
        }


        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent("MemoryStream"))
            {
                MemoryStream ms = data.GetData("MemoryStream") as MemoryStream;
                Formatter = new BinaryFormatter();
                ModelBase mBase = Formatter.Deserialize(ms) as ModelBase;

                if (SelectedItem is Folder folder)
                {
                    folder.Children.Add(mBase);
                }
                else if (SelectedItem is Machine)
                {
                    if (SelectedItem.Parent is Folder folder2)
                    {
                        folder2.Children.Add(mBase);
                    }
                    else if (SelectedItem.Parent == null)
                    {
                        (ViewModel.MachineListView.DataContext as MachineListViewModel).MachineList.Add(mBase);
                    }
                }
                else
                {
                    throw new Exception("Not sure how you got here");
                }

            }
        }



        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedItem != null;
        }
        #endregion


        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show($"Delete {SelectedItem.Name}?", "Please Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                (SelectedItem.ParentTreeView.ItemsSource as ObservableCollection<ModelBase>).Remove(SelectedItem);
            }
        }
    }
}
