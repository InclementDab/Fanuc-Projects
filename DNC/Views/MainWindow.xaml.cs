using DNC.Models;
using DNC.ViewModels;
using DNC.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using DNC;



namespace DNC.Views
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

        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tBox = sender as TextBox;
            if (!IsLoaded || tBox.Visibility != Visibility.Visible) return;

            tBox.Focus();
            tBox.SelectAll();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.MachineListViewModel.SelectedItem = (ModelBase)tView.SelectedItem ?? null; //todo
        }

        #region copypaste

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = SelectedItem != null;
            e.CanExecute = true;
        }

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            Clipboard.Clear();
            Clipboard.SetDataObject(SelectedItem, false);
            SelectedItem.ParentList.Remove(SelectedItem);
            */
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Clipboard.SetDataObject(SelectedItem, false);
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            IDataObject a = Clipboard.GetDataObject();
            SelectedItem.ParentList.Add(a as ModelBase);
            */
        }
        #endregion


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
                    dFolder.SetData(e.Data.GetData(typeof(ModelBase)));
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
