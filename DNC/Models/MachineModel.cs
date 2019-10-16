using DNC.ViewModels;
using DNC.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Xaml;
using System.Xml.Serialization;
using DNC.Communication;
using static DNC.Focas2;

namespace DNC.Models
{

    public enum ConnectionType
    {
        [Description("TCP/IP")]
        TCP = 0,

        [Description("Serial Port")]
        Serial = 1
    }

    [Serializable]
    public struct MachineData
    {
        public ushort Handle => Connection.Handle;
        public Connection Connection { get; set; }
        public ControllerData Controller { get; set; }

        [Serializable]
        public struct ControllerData
        {
            public string ControllerName { get; set; }
            public string ControllerVersion { get; set; }
        }
    }

    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Binary)]
    public class Machine : ModelBase, IEditableObject
    {
        public MachineData Data { get; set; } = new MachineData();

        public bool ConnectOnStartup { get; set; }
        public string InvertedConnectString => Data.Connection.IsConnected ? "Disconnect" : "Connect";

        public ObservableCollection<Program> ProgramList { get; set; } = new ObservableCollection<Program>();

        protected internal Machine() { }

        public Machine(string name) : base(name, "/Resources/Icons/Machine_16x.png")
        {
            Data.Connection.Connected += Connection_ConnectionChanged;
            Data.Connection.Disconnected += Connection_ConnectionChanged;
        }


        private void Connection_ConnectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            RaisePropertyChanged("InvertedConnectString");
        }

        #region Commands

        public ICommand ToggleConnection => new RelayCommand(() =>
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Task.Factory.StartNew(() =>
            {

                if (Data.Connection.IsConnected)
                    Data.Connection.Connect();
                else
                    Data.Connection.Disconnect();
            });

            Debug.WriteLine($"{sw.ElapsedMilliseconds}ms on Toggle Connection");
        });



        public ICommand Import => new RelayCommand(() =>
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog
            {
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            string[] fileData = File.ReadAllLines(dialog.FileName);
            ProgramList.Add(new Program(dialog.FileName, fileData));
            //SerializeList(MachineList);
        });

        public ICommand Edit => new RelayCommand(() =>
        {
            EditMachineProperties e = new EditMachineProperties();

            e.EditMachine(this);

        });

        #endregion

        #region interfaces


        public void BeginEdit()
        {
            backup = Clone();
        }

        public void EndEdit()
        {

        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }

        #endregion

    }

    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Binary)]
    public class Folder : ModelBase
    {
        public ObservableCollection<ModelBase> Children { get; set; }

        internal Folder() { }
        public Folder(string name) : base(name, "/Resources/Icons/Folder_16x.png")
        {
            Children = new ObservableCollection<ModelBase>();
        }
    }

    [XmlInclude(typeof(Folder))]
    [XmlInclude(typeof(Machine))]
    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Binary)]
    public abstract class ModelBase : ObservableObject
    {
        public ModelBase Parent => ParentTreeViewItem?.DataContext as ModelBase;

        [NonSerialized]
        public TreeViewItem TreeViewItem;

        public TreeView ParentTreeView
        {
            get
            {
                PropertyInfo x = TreeViewItem.GetType().GetProperty("ParentTreeView", BindingFlags.Instance | BindingFlags.NonPublic);
                return x?.GetValue(TreeViewItem) as TreeView;
            }
        }

        public TreeViewItem ParentTreeViewItem
        {
            get
            {
                PropertyInfo x = TreeViewItem.GetType().GetProperty("ParentTreeViewItem", BindingFlags.Instance | BindingFlags.NonPublic);
                return x?.GetValue(TreeViewItem) as TreeViewItem;
            }
        }

        public int ParentIndex => ParentTreeView.Items.IndexOf(TreeViewItem);


        /// <summary>
        /// Base Constructor
        /// </summary>
        /// <param name="name">Display Name on list</param>
        /// <param name="icon">Location of Icon in resources</param>
        protected ModelBase(string name = null, string icon = null)
        {
            Name = name;
            Icon = icon;
        }
        
        public string Icon { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public bool IsNameEditing { get; set; }

        public ICommand Rename => new RelayCommand(() =>
        {
            IsNameEditing = !IsNameEditing;
            RaisePropertyChanged("IsNameEditing");
        });

        #region interfaces

        protected ModelBase backup;
        public ModelBase Clone()
        {
            return (ModelBase)MemberwiseClone();
        }
        #endregion
    }
}
