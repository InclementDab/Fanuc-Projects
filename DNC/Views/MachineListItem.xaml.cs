using DNC.Models;
using DNC.ViewModels;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DNC.Views
{
    /// <summary>
    /// Interaction logic for MachineListItem.xaml
    /// </summary>
    public partial class MachineListItem : TreeViewItem
    {

        public readonly ModelBase ModelBase;
        public readonly MachineListItemViewModel ViewModel;

        public MachineListItem(ModelBase mBase)
        {
            InitializeComponent();
            DataContext = ViewModel = new MachineListItemViewModel(mBase);
            ModelBase = mBase;
        }





    }
}
