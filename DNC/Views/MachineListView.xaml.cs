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
        private readonly MachineListViewModel ViewModel;
        public MachineListView()
        {
            InitializeComponent();
            DataContext = ViewModel = new MachineListViewModel();

            ViewModel.AddListItem("machine1", ModelType.Machine);
            ViewModel.AddListItem("machine2", ModelType.Machine);

            ViewModel.EnumeratedList[0].ProgramList.Add(new Program(1, "yeet"));

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton.Equals(MouseButtonState.Pressed))
            {
                if (e.OriginalSource is FrameworkElement tBlock)
                {
                    if (tBlock.DataContext is ModelBase mBase)
                    {
                        DragDrop.DoDragDrop(this, mBase, DragDropEffects.None | DragDropEffects.Move);
                    }
                }
            }
            e.Handled = true;
        }


        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            FrameworkElement oElement = e.OriginalSource as FrameworkElement;
            ModelBase mBaseDropped = e.Data.GetData(typeof(ModelBase)) as ModelBase;
            ModelBase mBaseTarget = oElement.DataContext as ModelBase;

            if (mBaseDropped != mBaseTarget)
            {
                Border border = RecursiveGetType<Border>(oElement);
                Point p = e.GetPosition(oElement);

                double safeZ = oElement.ActualHeight / 3;

                if (p.Y < safeZ)
                {
                    border.BorderThickness = new Thickness(0, 1, 0, 0);
                }
                else if (p.Y > oElement.ActualHeight - safeZ)
                {
                    border.BorderThickness = new Thickness(0, 0, 0, 1);
                }
                else
                {
                    if (mBaseTarget.Type == ModelType.Folder)
                    {
                        border.BorderThickness = new Thickness(1);
                    }
                    else
                    {
                        border.BorderThickness = new Thickness(0);
                    }
                }
            }

            e.Handled = true;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);

            FrameworkElement oElement = e.OriginalSource as FrameworkElement;
            Border border = RecursiveGetType<Border>(oElement);
            border.BorderThickness = new Thickness(0);

            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            FrameworkElement oElement = e.OriginalSource as FrameworkElement;
            ModelBase mBaseDropped = e.Data.GetData(typeof(ModelBase)) as ModelBase;
            ModelBase mBaseTarget = oElement.DataContext as ModelBase;
            RecursiveGetType<Border>(oElement).BorderThickness = new Thickness(0);
            try
            {
                int indx = mBaseTarget.ParentCollection.IndexOf(mBaseTarget);
                Point p = e.GetPosition(oElement);

                double safeZ = oElement.ActualHeight / 3;

                if (p.Y > oElement.ActualHeight - safeZ)
                {
                    if (indx + 1 > mBaseTarget.ParentCollection.Count())
                    {
                        mBaseTarget.ParentCollection.Add(mBaseDropped);
                        mBaseDropped.ParentCollection.Remove(mBaseDropped);
                    }
                    else
                    {
                        mBaseTarget.ParentCollection.Insert(indx + 1, mBaseDropped);
                        mBaseDropped.ParentCollection.Remove(mBaseDropped);
                    }
                }
                else if (p.Y < safeZ)
                {
                    mBaseTarget.ParentCollection.Insert(indx, mBaseDropped);
                    mBaseDropped.ParentCollection.Remove(mBaseDropped);
                }

                else
                {
                    if (mBaseTarget.Type == ModelType.Folder)
                    {
                        mBaseTarget.Children.Add(mBaseDropped);
                        mBaseDropped.ParentCollection.Remove(mBaseDropped);
                    }
                    else
                    {
                        //TODO make cursor = DragDropEffects.None    
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            e.Handled = true;
        }

        private T RecursiveGetType<T>(DependencyObject current)
        {
            if (LogicalTreeHelper.GetParent(current) is T ret)
            {
                return ret;
            }
            else
            {
                return RecursiveGetType<T>(LogicalTreeHelper.GetParent(current));
            }
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

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedItem = e.NewValue as ModelBase;
        }
    }
}
