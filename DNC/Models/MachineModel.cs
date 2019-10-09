using DNC.ViewModels;
using DNC.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
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

    public class Machine : ModelBase
    {

        private ushort handle;
        private TCPConnection TCPConnection { get; set; }
        private SerialConnection SerialConnection { get; set; }

        public Connection Connection
        {
            get => GetActiveConnection();
            set => SetActiveConnection(value);
        }

        public ConnectionType CurrentConnectionType { get; set; }

        public Connection GetActiveConnection()
        {
            RaisePropertyChanged("CurrentConnectionType");
            switch (CurrentConnectionType)
            {
                case ConnectionType.Serial:
                    return SerialConnection;

                case ConnectionType.TCP:
                    return TCPConnection;

                default:
                    return null;
            }
        }

        public void SetActiveConnection(Connection value)
        {
            if (value is TCPConnection t)
            {
                CurrentConnectionType = ConnectionType.TCP;
                TCPConnection = t;
            }

            if (value is SerialConnection s)
            {
                CurrentConnectionType = ConnectionType.Serial;
                SerialConnection = s;
            }

            RaisePropertyChanged("Connection");
        }

        public bool ConnectOnStartup { get; set; }
        public string InvertedConnectString => Connection.IsConnected ? "Disconnect" : "Connect";

        private short statusCode;
        public short StatusCode
        {
            get => statusCode;
            private set
            {
                statusCode = value;
                Debug.WriteLine(statusCode);
            }
        }

        public ObservableCollection<Program> ProgramList { get; set; }

        public ICommand ToggleConnection { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Import { get; private set; }

        public string MachineDirectory { get; private set; } // probably put this in individual programs upon register

        public Machine(string name, ICollection<ModelBase> parentList, Connection connection = null) : base(name, parentList, "/Resources/Icons/Machine_16x.png")
        {
            MachineDirectory = "//CNC_MEM/USER/";
            Connection = connection ?? new TCPConnection(IPAddress.Parse("0.0.0.0"), 8193);
            ProgramList = new ObservableCollection<Program>();

            ToggleConnection = new RelayCommand(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    StatusCode = Connection.IsConnected ? Connection.Close(handle) : Connection.Open(out handle);
                });
            });

            Edit = new RelayCommand(() =>
            {
                EditPrompt e = new EditPrompt();
                e.EditMachine(this);
            });

            Import = new RelayCommand(() =>
            {
                System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog
                {
                    RestoreDirectory = true
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string[] fileData = File.ReadAllLines(dialog.FileName);
                    ProgramList.Add(new Program(dialog.FileName, fileData));
                }
            });


            Connection.ConnectionChanged += Connection_ConnectionChanged;
        }

        public ODBST StatInfo = new ODBST();
        public ODBSYSEX SystemInfo = new ODBSYSEX();

        private void Connection_ConnectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            RaisePropertyChanged("InvertedConnectString");

           
            if (Connection.IsConnected)
            {
                Debug.WriteLine("Loading Machine Info...");
                StatusCode = cnc_statinfo(handle, StatInfo);
                StatusCode = cnc_sysinfo_ex(handle, SystemInfo);
                Debug.WriteLine("Machine Info Loaded");
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public void ReadDirectory()
        {
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
                    Debug.WriteLine(ToJson(subOut));
                }

                short dsvInfo = 0;
                StatusCode = cnc_dtsvgetmode(handle, out dsvInfo);

                Debug.WriteLine(ToJson(dsvInfo + 100));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }

        public int PushProgram(Program program)
        {
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

        public static string ToJson<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);


    }

    public class Folder : ModelBase
    {
        public ObservableCollection<ModelBase> Children;
        public Folder(string name, ICollection<ModelBase> parentList) : base(name, parentList, "/Resources/Icons/Folder_16x.png")
        {
            Children = new ObservableCollection<ModelBase>();
        }
    }


    [Serializable()]
    public abstract class ModelBase : ObservableObject, IDataObject
    {
        public ICommand Rename { get; private set; }

        public ICollection<ModelBase> ParentList { get; private set; }

        public ModelBase(string name, ICollection<ModelBase> parentList, string icon)
        {
            Name = name;
            Icon = icon;
            ParentList = parentList;

            Rename = new RelayCommand(() =>
            {
                IsNameEditing = !IsNameEditing;
                RaisePropertyChanged("IsNameEditing");
            });
        }

        public string Icon { get; private set; }

        public bool IsNameEditing { get; set; }

        public string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        #region dataobject
        public readonly string[] formats = new string[]
        {
            "DNC.Models.ModelBase",
            "DNC.Models.Machine",
            "DNC.Models.Folder"
        };

        public readonly Type[] Tformats = new Type[]
        {
            typeof(ModelBase),
            typeof(Machine),
            typeof(Folder)
        };

        public object GetData(string format)
        {
            if (formats.Contains(format))
                return new DataObject(this);

            else throw new NotImplementedException();
        }

        public object GetData(Type format)
        {
            if (Tformats.Contains(format))
                return new DataObject(this);

            else throw new NotImplementedException();
        }

        public object GetData(string format, bool autoConvert)
        {
            if (formats.Contains(format))
                return new DataObject(this);

            else throw new NotImplementedException();
        }

        public bool GetDataPresent(string format) => formats.Contains(format);

        public bool GetDataPresent(Type format) => Tformats.Contains(format);

        public bool GetDataPresent(string format, bool autoConvert) => formats.Contains(format);

        public string[] GetFormats() => formats;

        public string[] GetFormats(bool autoConvert) => formats;

        public virtual void SetData(object data)
        {
            throw new NotImplementedException();
        }

        public virtual void SetData(string format, object data)
        {
            throw new NotImplementedException();
        }

        public virtual void SetData(Type format, object data)
        {
            throw new NotImplementedException();
        }

        public virtual void SetData(string format, object data, bool autoConvert)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
