using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DNC.Models
{

    public enum ModelType
    {
        Folder = 0,
        Machine = 1
    }

    public class ModelBase : INotifyPropertyChanged
    {
        public ModelBase(string name, ModelType type)
        {
            Name = name;
            Type = type;

            if (Type == ModelType.Folder)
                Children = new ObservableCollection<ModelBase>();
        }

        
        public string Icon
        {
            get
            {
                switch (Type)
                {
                    case ModelType.Folder:
                        return "/Icons/Folder_16x.png";

                    case ModelType.Machine:
                        return "/Icons/Machine_16x.png";

                    default:
                        return "";
                }
            }
        }
        public ModelType Type { get; set; }
        public ObservableCollection<ModelBase> Children { get; set; }

        private bool isNameEditing;
        public bool IsNameEditing
        {
            get => isNameEditing;
            set
            {
                isNameEditing = value;
                NotifyPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
