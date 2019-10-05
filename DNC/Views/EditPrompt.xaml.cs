using DNC.Models;
using DNC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        public EditPrompt()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ModelBase CreateDialog()
        {
            bool? dr = ShowDialog();
            return (dr ?? false) ? new ModelBase(txtName.Text, ModelType.Machine, null) : null;

        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        p => true,
                        p => DialogResult = true 
                        );
                }
                return _saveCommand;
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(
                        p => true,
                        p => DialogResult = false
                        );
                }
                return _cancelCommand;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtName.SelectAll();
            txtName.Focus();
        }
    }
}
