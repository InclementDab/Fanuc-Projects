using DNC.Models;
using DNC.ViewModels;
using DNC.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }


        #region copypaste
        public ModelBase SelectedItem => ((DataContext as MainWindowViewModel).MachineListView.DataContext as MachineListViewModel).SelectedItem;
        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.Clear();

            DataObject data = new DataObject();
            data.SetData(SelectedItem);            
            Clipboard.SetDataObject(data);

            SelectedItem.ParentList.Remove(SelectedItem);
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Clipboard.SetDataObject(ViewModel.SelectedItem, false);
        }


        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            IDataObject data = Clipboard.GetDataObject();
            
            if (data.GetDataPresent(typeof(Machine)))
            {
                var x = data.GetData(typeof(Machine)) as Machine;
                SelectedItem.ParentList.Add(x);
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded;
            //e.CanExecute = tView.SelectedItem != null;
        }
        #endregion

    }
}
