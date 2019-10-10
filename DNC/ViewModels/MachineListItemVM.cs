using DNC.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DNC.ViewModels
{
    public class MachineListItemViewModel : ViewModelBase
    {
        public ICommand Rename { get; private set; }

        public ModelBase Machine { get; private set; }
        public MachineListItemViewModel(ModelBase mBase)
        {
            Machine = mBase;

            Rename = new RelayCommand(() =>
            {
                Machine.IsNameEditing = !Machine.IsNameEditing;
                RaisePropertyChanged("IsNameEditing");
            });
        }

    }
}
