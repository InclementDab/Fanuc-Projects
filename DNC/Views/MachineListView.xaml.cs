using DNC.Models;
using DNC.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for MachineList.xaml
    /// </summary>
    public partial class MachineListView : UserControl
    {
        private MachineListViewModel ViewModel;
        public MachineListView()
        {
            InitializeComponent();

            DataContext = ViewModel = new MachineListViewModel();
        }

        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = GetDragState(sender, e);
            e.Handled = true;
        }

        private void TreeView_DragLeave(object sender, DragEventArgs e)
        {
            e.Effects = GetDragState(sender, e);
            e.Handled = true;
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = GetDragState(sender, e);
            e.Handled = true;
        }

        private DragDropEffects GetDragState(object sender, DragEventArgs e)
        {
            if (LogicalTreeHelper.GetParent(e.OriginalSource as DependencyObject) is StackPanel panel && sender is TreeView tView) // gets parent of originalsource, which gives us StackPanel
                if (panel.DataContext is ModelBase mBase && mBase != tView.SelectedItem) // gets what we are dragged over, also Check if dragged over self
                    if (mBase.Type == ModelType.Folder) // Check if dragged over folder
                        return DragDropEffects.Move;


            return DragDropEffects.None;
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }


        private void TreeView_LeftMouseDown(object sender, MouseEventArgs e)
        {
            foreach (ModelBase mBase in ViewModel.EnumeratedList)
                mBase.IsNameEditing = false;

            if (sender is TreeView tView)
            {


                if (tView.SelectedItem != null)
                {
                    DragDrop.DoDragDrop(tView, tView.SelectedItem, DragDropEffects.Move);
                }
            }
        }

        private TextBlock lBlock;
        private DateTime dTime;
        private void TreeView_PreviewLeftMouseUp(object sender, MouseButtonEventArgs e)
        {   
            if (sender is TreeView tView)
            {
                if (e.OriginalSource is TextBlock tBlock && tView.SelectedItem != null)
                {
                    if (tBlock != lBlock)
                    {
                        lBlock = tBlock;
                        dTime = DateTime.Now;  
                    }
                    else if (tBlock.DataContext is ModelBase mBase && DateTime.Now > dTime.AddSeconds(1))
                    {
                        mBase.IsNameEditing = !mBase.IsNameEditing;
                        lBlock = null;
                    }
                }
            }
        }



        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void TreeView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }



        

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {

            

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox tBox = sender as TextBox;
                if (tBox.DataContext is ModelBase mBase)
                {
                    mBase.IsNameEditing = !mBase.IsNameEditing;
                    mBase.Name = tBox.Text;
                }
            }

            
            
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tBox = sender as TextBox;
            if (!IsLoaded || tBox.Visibility != Visibility.Visible) return;

            tBox.Focus();
            tBox.SelectAll();
        }


    }
}
