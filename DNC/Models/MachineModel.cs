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
    [SettingsSerializeAs(SettingsSerializeAs.Binary)]
    public class Machine : ModelBase
    {

        private ushort _handle;
        private TCPConnection TcpConnection { get; set; }
        private SerialConnection SerialConnection { get; set; }

        public ICommand ToggleConnection => new RelayCommand(() =>
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Task.Factory.StartNew(() =>
            {
                StatusCode = Connection.IsConnected ? Connection.Close(_handle) : Connection.Open(out _handle);
            });
            
            Debug.WriteLine($"{sw.ElapsedMilliseconds}ms on Toggle Connection");
        });
        
        public ICommand Edit => new RelayCommand(() =>
        {
            EditPrompt e = new EditPrompt();
            e.EditMachine(this);
            //SerializeList(MachineList);
        });

        
        public ICommand Import => new RelayCommand(() =>
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog
            {
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] fileData = File.ReadAllLines(dialog.FileName);
                ProgramList.Add(new Program(dialog.FileName, fileData));
                //SerializeList(MachineList);
            }
        });

        public Connection Connection
        {
            get => GetActiveConnection();
            set => SetActiveConnection(value);
        }

        private ConnectionType _currentConnectionType;
        public ConnectionType CurrentConnectionType
        {
            get => _currentConnectionType;
            set
            {
                _currentConnectionType = value;
                RaisePropertyChanged();
            }
        }

        public Connection GetActiveConnection()
        {
            RaisePropertyChanged("CurrentConnectionType");
            switch (CurrentConnectionType)
            {
                case ConnectionType.Serial:
                    return SerialConnection;

                case ConnectionType.TCP:
                    return TcpConnection;

                default:
                    return null;
            }
        }

        public void SetActiveConnection(Connection value)
        {
            switch (value)
            {
                case TCPConnection t:
                    CurrentConnectionType = ConnectionType.TCP;
                    TcpConnection = t;
                    break;
                case SerialConnection s:
                    CurrentConnectionType = ConnectionType.Serial;
                    SerialConnection = s;
                    break;
            }

            RaisePropertyChanged("Connection");
        }

        public bool ConnectOnStartup { get; set; }
        public string InvertedConnectString => Connection.IsConnected ? "Disconnect" : "Connect";

        private short _statusCode;
        public short StatusCode
        {
            get => _statusCode;
            set
            {
                _statusCode = value;
                Debug.WriteLine(_statusCode);
            }
        }

        public ObservableCollection<Program> ProgramList { get; set; }

        public string MachineDirectory; // probably put this in individual programs upon register

        protected internal Machine() { }

        public Machine(string name, Connection connection = null) : base(name,"/Resources/Icons/Machine_16x.png")
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            MachineDirectory = "//CNC_MEM/USER/";
            TcpConnection = new TCPConnection(IPAddress.Parse("0.0.0.0"), 8193);
            SerialConnection = new SerialConnection();
            Connection = connection ?? TcpConnection;

            ProgramList = new ObservableCollection<Program>();
            Connection.ConnectionChanged += Connection_ConnectionChanged;
            
            Debug.WriteLine($"{sw.ElapsedMilliseconds}ms on Machine Constructor");
            sw.Stop();
        }


        private void Connection_ConnectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            RaisePropertyChanged("InvertedConnectString");

            ODBST StatInfo = new ODBST(); // do something with these
            ODBSYSEX SystemInfo = new ODBSYSEX(); // hehe

            if (!Connection.IsConnected) return;
            Debug.WriteLine("Loading Machine Info...");
            StatusCode = cnc_statinfo(_handle, StatInfo);
            StatusCode = cnc_sysinfo_ex(_handle, SystemInfo);
            Debug.WriteLine("Machine Info Loaded");
        }

        [HandleProcessCorruptedStateExceptions]
        public static void ReadDirectory(ushort handle)
        {
            short StatusCode;
            try
            {
                ODBPDFDRV drvOut = new ODBPDFDRV();
                StatusCode = cnc_rdpdf_drive(handle, drvOut);

                List<object> drives = new List<object>();

                for (int i = 1; i <= drvOut.max_num; i++)
                    drives.Add(drvOut.GetType().GetField("drive" + i).GetValue(drvOut) as object); // this outputs CNC_MEM, and DATA_SV, DATA_SV throws memory error :(


                ODBPDFNFIL dirInfo = new ODBPDFNFIL();
                StatusCode = cnc_rdpdf_subdirn(handle, $"//CNC_MEM/", dirInfo);

                short dirNum = dirInfo.dir_num;
                for (short j = 0; j < dirInfo.dir_num; j++)
                {
                    IDBPDFSDIR subIn = new IDBPDFSDIR()
                    {
                        path = $"//CNC_MEM/",
                        req_num = j
                    };
                    ODBPDFSDIR subOut = new ODBPDFSDIR();

                    StatusCode = cnc_rdpdf_subdir(handle, ref dirNum, subIn, subOut);
                    Debug.WriteLine(App.ToJson(subOut));
                }

                short dsvInfo = 0;
                StatusCode = cnc_dtsvgetmode(handle, out dsvInfo);

                Debug.WriteLine(App.ToJson(dsvInfo + 100));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }

        public static int PushProgram(Program program, ushort handle, string MachineDirectory)
        {
            short StatusCode = 0;
            Task.Factory.StartNew(() =>
            {
                Debug.WriteLine("Pushing Program to Machine");
                int length = program.MachineSafeData.Length;
                StatusCode = cnc_dwnstart4(handle, 0, MachineDirectory);
                StatusCode = cnc_download4(handle, ref length, program.MachineSafeData);
                StatusCode = cnc_dwnend4(handle);
                Debug.WriteLine("Push Complete");
                //StatusCode = cnc_verify4(handle, )
            });
            return StatusCode;
        }
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
                var x = TreeViewItem.GetType().GetProperty("ParentTreeView", BindingFlags.Instance | BindingFlags.NonPublic);
                return x.GetValue(TreeViewItem) as TreeView;
            }
        }

        public TreeViewItem ParentTreeViewItem
        {
            get
            {
                var x = TreeViewItem.GetType().GetProperty("ParentTreeViewItem", BindingFlags.Instance | BindingFlags.NonPublic);
                return x.GetValue(TreeViewItem) as TreeViewItem;
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
    }
}
