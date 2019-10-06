using DNC.Models;
using DNC.ViewModels;
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
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public EditPrompt()
        {
            InitializeComponent();
            DataContext = this;

            SaveCommand = new RelayCommand(() => DialogResult = true);
            CancelCommand = new RelayCommand(() => DialogResult = false);
        }

        public Machine CurrentMachine { get; set; }

        public void EditMachine(Machine input)
        {
            CurrentMachine = input;
            bool? dr = ShowDialog();

            if (!dr ?? false)
            {
                CurrentMachine = input; // idk how to do this yet
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtName.SelectAll();
            txtName.Focus();
        }
    }
}
