using DNC.Models;
using DNC.ViewModels;
using System;
using System.Collections.Generic;
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
        private MachineListViewModel ViewModel { get; set; }

        public MachineListView()
        {
            InitializeComponent();
            DataContext = ViewModel = new MachineListViewModel();
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
                    mBase.RaisePropertyChanged("IsNameEditing");
                }
            }
        }

       
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedItem = (ModelBase)e.NewValue ?? (ModelBase)tView.SelectedItem;
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
            
            PropertyInfo pTreeView = mBase.TreeViewItem.GetType().GetProperty("ParentTreeView", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            PropertyInfo pTreeViewItem = mBase.TreeViewItem.GetType().GetProperty("ParentTreeViewItem", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            mBase.ParentTreeView = pTreeView.GetValue(mBase.TreeViewItem) as TreeView;
            mBase.ParentTreeViewItem = pTreeViewItem.GetValue(mBase.TreeViewItem) as TreeViewItem;



            return;




        }




        #region dragdrop
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
                Border border = App.RecursiveGetType<Border>(oElement);
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
                    if (mBaseTarget.GetType() == typeof(Folder))
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
            Border border = App.RecursiveGetType<Border>(oElement);
            border.BorderThickness = new Thickness(0);

            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            FrameworkElement dTarget = e.OriginalSource as FrameworkElement;
            if (e.Data.GetDataPresent(typeof(ModelBase)))
            {
                if (dTarget.DataContext is Folder dFolder)
                {
                    //dFolder.SetData(e.Data.GetData(typeof(ModelBase)));
                    //dFolder.Children.Add(e.Data.GetData(typeof(ModelBase)) as ModelBase);

                }

            }

            e.Handled = true;
            return;

            /*
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
            */
        }

        #endregion

        
    }
}
