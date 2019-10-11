using DNC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace DNC.Views
{
    public class TreeViewClipboardBehavior : Behavior<TreeView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            CommandBinding CopyCommandBinding = new CommandBinding(
                ApplicationCommands.Copy,
                CopyCommandExecuted,
                CopyCommandCanExecute);
            AssociatedObject.CommandBindings.Add(CopyCommandBinding);

            CommandBinding CutCommandBinding = new CommandBinding(
                ApplicationCommands.Cut,
                CutCommandExecuted,
                CutCommandCanExecute);
            AssociatedObject.CommandBindings.Add(CutCommandBinding);

            CommandBinding PasteCommandBinding = new CommandBinding(
                ApplicationCommands.Paste,
                PasteCommandExecuted,
                PasteCommandCanExecute);
            AssociatedObject.CommandBindings.Add(PasteCommandBinding);
        }

        private void CopyCommandExecuted(object target, ExecutedRoutedEventArgs e)
        {
            MachineListViewModel item = AssociatedObject.SelectedItem as MachineListViewModel;
            if (item != null && item.CanCopyToClipboard)
            {
                item.CopyToClipboard();
                e.Handled = true;
            }
        }

        private void CopyCommandCanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            MachineListViewModel item = AssociatedObject.SelectedItem as MachineListViewModel;
            if (item != null)
            {
                e.CanExecute = item.CanCopyToClipboard;
                e.Handled = true;
            }
        }

        private void CutCommandExecuted(object target, ExecutedRoutedEventArgs e)
        {
            MachineListViewModel item = AssociatedObject.SelectedItem as MachineListViewModel;
            if (item != null && item.CanCutToClipboard)
            {
                item.CutToClipboard();
                e.Handled = true;
            }
        }

        private void CutCommandCanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            MachineListViewModel item = AssociatedObject.SelectedItem as MachineListViewModel;
            if (item != null)
            {
                e.CanExecute = item.CanCutToClipboard;
                e.Handled = true;
            }
        }


        private void PasteCommandExecuted(object target, ExecutedRoutedEventArgs e)
        {
            MachineListViewModel item = AssociatedObject.SelectedItem as MachineListViewModel;
            if (item != null && item.CanPasteFromClipboard)
            {
                item.PasteFromClipboard();
                e.Handled = true;
            }
        }

        private void PasteCommandCanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            MachineListViewModel item = AssociatedObject.SelectedItem as MachineListViewModel;
            if (item != null)
            {
                e.CanExecute = item.CanPasteFromClipboard;
                e.Handled = true;
            }
        }
    }
}
