using DNC.Models;
using DNC.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
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
        protected MachineListVM ViewModel { get; set; }

        public MachineListView()
        {
            InitializeComponent();
            DataContext = ViewModel = new MachineListVM();
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tBox = sender as TextBox;
            if (!(tBox?.DataContext is ModelBase mBase)) return;

            switch (e.Key)
            {
                case Key.Return:
                {
                    mBase.IsNameEditing = !mBase.IsNameEditing;
                    mBase.Name = tBox.Text;
                    mBase.RaisePropertyChanged("IsNameEditing");
                    break;
                }
                case Key.Escape:
                {
                    mBase.IsNameEditing = !mBase.IsNameEditing;
                    mBase.RaisePropertyChanged("IsNameEditing");
                    break;
                }
            }
        }


        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedItem = (ModelBase)e.NewValue ?? (ModelBase)TView.SelectedItem;
        }

        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tBox = sender as TextBox;
            if (!IsLoaded || tBox.Visibility != Visibility.Visible) return;

            tBox.Focus();
            tBox.SelectAll();
        }


        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;
            ModelBase mBase = border.DataContext as ModelBase;
            mBase.TreeViewItem = App.RecursiveGetType<TreeViewItem>(border); // sets the TreeViewItem in the ModelBase
        }




        #region dragdrop



        private Point startPoint;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            startPoint = e.GetPosition(this);

            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton.Equals(MouseButtonState.Pressed) && ViewModel.SelectedItem != null)
            {
                Point currentPoint = e.GetPosition(this);
                if (Math.Abs(currentPoint.X - startPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPoint.Y - startPoint.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    DragDrop.DoDragDrop(ViewModel.SelectedItem.TreeViewItem, ViewModel.SelectedItem, DragDropEffects.None | DragDropEffects.Move);
                }
            }

            endHandle:
            e.Handled = true;
        }


        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            // left off here mBaseTarget and mBaseDropped were giving same values

            //DragDropTarget = (e.OriginalSource as FrameworkElement).DataContext as ModelBase;

            if (!(((FrameworkElement)e.OriginalSource).DataContext is ModelBase mBaseTarget)) goto endHandled; // mBaseTarget (Machine Underneath of Dragged Target, needs border)
            if (!(e.Data.GetData(e.Data.GetFormats().FirstOrDefault()) is ModelBase mBaseDragged)) goto endHandled; // mBaseDragged (Dragged Target, to be moved)
            
            if (mBaseTarget == mBaseDragged) goto endHandled;

            mBaseTarget.TreeViewItem.BorderBrush = Brushes.Black;

            if (mBaseTarget is Machine)
            {
                Point p = e.GetPosition(mBaseTarget.TreeViewItem);
                double safeZ = mBaseTarget.TreeViewItem.ActualHeight / 3;

                mBaseTarget.TreeViewItem.BorderThickness = new Thickness(0, p.Y < safeZ ? 1 : 0, 0, p.Y > mBaseTarget.TreeViewItem.ActualHeight - safeZ ? 1 : 0);

                e.Effects = p.Y < safeZ || p.Y > mBaseTarget.TreeViewItem.ActualHeight - safeZ ? DragDropEffects.Move : DragDropEffects.None;
            }
            else
            {
                mBaseTarget.TreeViewItem.BorderThickness = new Thickness(mBaseTarget is Folder ? 1 : 0);
                e.Effects = DragDropEffects.Move;
            }

            endHandled:
            e.Handled = true;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);

            if (!(((FrameworkElement)e.OriginalSource).DataContext is ModelBase mBaseTarget)) goto endHandled; // mBaseTarget (Dragged Target, to be moved)
            if (!(e.Data.GetData(e.Data.GetFormats().FirstOrDefault()) is ModelBase mBaseDropped)) goto endHandled; // mBaseDropped (Underneath Target)

            mBaseTarget.TreeViewItem.BorderThickness = new Thickness(0);

            endHandled:
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            

            if (!(((FrameworkElement)e.OriginalSource).DataContext is ModelBase mBaseTarget)) goto endHandled; // mBaseTarget (Dragged Target, to be moved)
            if (!(e.Data.GetData(e.Data.GetFormats().FirstOrDefault()) is ModelBase mBaseDropped)) goto endHandled; // mBaseDropped (Underneath Target)

            mBaseTarget.TreeViewItem.BorderThickness = new Thickness(0);

            if (e.Effects == DragDropEffects.None) goto endHandled;

            if (mBaseTarget is Folder folder)
            {
                ListCutPaste(mBaseDropped.ParentTreeView.ItemsSource as ObservableCollection<ModelBase>, folder.Children, mBaseDropped, mBaseTarget, -1);
            }
            else if (mBaseTarget is Machine)
            {
                Point p = e.GetPosition(mBaseTarget.TreeViewItem);
                double safeZ = mBaseTarget.TreeViewItem.ActualHeight / 3;

                int insertAt = p.Y < safeZ ? -1 : p.Y > mBaseTarget.TreeViewItem.ActualHeight - safeZ ? 1 : 0;
                if (mBaseTarget.Parent is Folder folder2)
                {
                    ListCutPaste(mBaseDropped.ParentTreeView.ItemsSource as ObservableCollection<ModelBase>, folder2.Children, mBaseDropped, mBaseTarget, 100);
                }
                else if (mBaseTarget.Parent == null)
                {
                    ListCutPaste(mBaseDropped.ParentTreeView.ItemsSource as ObservableCollection<ModelBase>, ViewModel.MachineList, mBaseDropped, mBaseTarget, insertAt);
                    //insertAt += ViewModel.MachineList.IndexOf(mBaseTarget);
                    //ViewModel.MachineList.Insert(insertAt, mBaseDropped);
                    //ViewModel.MachineList.Remove(mBaseDropped);
                }
            }
            else
            {
                throw new Exception("Not sure how you got here");
            }

            endHandled:
            e.Handled = true;
        }

        #endregion

        public enum RelativeIndex
        {
            Before = -1,
            After = 1
        }

        public static void ListCutPaste(Collection<ModelBase> SourceList, Collection<ModelBase> TargetList, ModelBase ItemToMove, ModelBase TargetItem, int i)
        {
            int Index = TargetList.IndexOf(TargetItem) + i;
            int SourceIndex = SourceList.IndexOf(ItemToMove);
            Debug.WriteLine(Index);
            if (Index <= 0)
            {
                TargetList.Insert(0, ItemToMove);
            }
            else if (Index > TargetList.Count)
            {
                TargetList.Add(ItemToMove);
            }
            else
            {
                TargetList.Insert(Index, ItemToMove);
            }

            if (TargetList.Contains(ItemToMove))
            {
                if (TargetList == SourceList)
                {
                    SourceList.RemoveAt(SourceIndex + SourceIndex > Index ? 1 : 0);
                }
                else
                {
                    SourceList.Remove(ItemToMove);
                }
            }
            else
            {

            }
        }

        private void MachineListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (TView.ItemContainerGenerator.ContainerFromItem(ViewModel.MachineList.FirstOrDefault()) is TreeViewItem tvi)
            {
                tvi.IsSelected = true;
            }
        }
    }
}
