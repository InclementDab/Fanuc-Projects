using DNC.ViewModels;
using DNC.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using static DNC.Focas2;

namespace DNC.Models
{



    public class Machine : ModelBase, IDisposable
    {

        private ushort handle;
        public Connection Connection { get; set; }

        public bool ConnectOnStartup { get; set; }
        public string IsConnectedString => IsConnected ? "Disconnect" : "Connect";

        private ConnectionStatus _connectionStatus;
        public ConnectionStatus ConnectionStatus
        {
            get => _connectionStatus;
            private set
            {
                _connectionStatus = value;
                NotifyPropertyChanged();
            }
        }


        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                _isConnected = value;
                NotifyPropertyChanged("IsConnectedString");
            }
        }

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

        public ObservableCollection<Program> ProgramList { get; private set; }

        public ICommand Connect { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Import { get; private set; }

        public string MachineDirectory { get; private set; } // probably put this in individual programs upon register


        public readonly ODBSYSEX SystemInfo = new ODBSYSEX();
        public readonly ODBST StatInfo = new ODBST();

        public Machine(string name) : base(name, "/Resources/Icons/Machine_16x.png")
        {
            ConnectionStatus = ConnectionStatus.Disconnected;
            ProgramList = new ObservableCollection<Program>();
            MachineDirectory = "//CNC_MEM/USER/";

            Connect = new RelayCommand(() =>
            {
                if (!IsConnected)
                    OpenConnection();
                else
                    CloseConnection();
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

            Messenger m = new Messenger();
            
        }

        public void OpenConnection()
        {
            ConnectionStatus = ConnectionStatus.Connecting;

            switch (Connection.Type)
            {
                case ConnectionType.TCP:
                    StatusCode = cnc_allclibhndl3(Connection.IPAddress.ToString(), (ushort)Connection.Port, 10, out handle);
                    break;
                case ConnectionType.Serial:

                    string inString = Regex.Match(Connection.ComPort, "(\\d+)").Value;
                    int.TryParse(inString, out int comNum);
                    Debug.WriteLine(inString);

                    StatusCode = cnc_allclibhndl2(comNum, out handle);
                    break;
            }
            


            if (StatusCode == 0)
            {
                StatusCode = cnc_statinfo(handle, StatInfo);
                StatusCode = cnc_sysinfo_ex(handle, SystemInfo);
                ConnectionStatus = ConnectionStatus.Connected;
                IsConnected = true;
            }
        }

        public void CloseConnection()
        {
            StatusCode = cnc_freelibhndl(handle);
            IsConnected = (StatusCode != 0);
            if (!IsConnected) ConnectionStatus = ConnectionStatus.Disconnected;
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
            int length = program.MachineSafeData.Length;
            StatusCode = cnc_dwnstart4(handle, 0, MachineDirectory);
            StatusCode = cnc_download4(handle, ref length, program.MachineSafeData);
            StatusCode = cnc_dwnend4(handle);

            return StatusCode;
        }



        public void Dispose()
        {
            if (IsConnected)
                StatusCode = cnc_freelibhndl(handle);
        }

        public static string ToJson<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

        public void Register<TMessage>(object recipient, Action<TMessage> action, bool keepTargetAlive = false)
        {
            throw new NotImplementedException();
        }

        public void Register<TMessage>(object recipient, object token, Action<TMessage> action, bool keepTargetAlive = false)
        {
            throw new NotImplementedException();
        }

        public void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action, bool keepTargetAlive = false)
        {
            throw new NotImplementedException();
        }

        public void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action, bool keepTargetAlive = false)
        {
            throw new NotImplementedException();
        }

        public void Send<TMessage>(TMessage message)
        {
            throw new NotImplementedException();
        }

        public void Send<TMessage, TTarget>(TMessage message)
        {
            throw new NotImplementedException();
        }

        public void Send<TMessage>(TMessage message, object token)
        {
            throw new NotImplementedException();
        }

        public void Unregister(object recipient)
        {
            throw new NotImplementedException();
        }

        public void Unregister<TMessage>(object recipient)
        {
            throw new NotImplementedException();
        }

        public void Unregister<TMessage>(object recipient, object token)
        {
            throw new NotImplementedException();
        }

        public void Unregister<TMessage>(object recipient, Action<TMessage> action)
        {
            throw new NotImplementedException();
        }

        public void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            throw new NotImplementedException();
        }
    }

    public class Folder : ModelBase, IDataObject
    {
        public ObservableCollection<ModelBase> Children { get; set; }
        public Folder(string name) : base(name, "/Resources/Icons/Folder_16x.png")
        {
            Children = new ObservableCollection<ModelBase>();
        }

        public override void SetData(object data)
        {
            Children.Add(data as ModelBase);
        }
    }

    public abstract class ModelBase : INotifyPropertyChanged, IDataObject
    {
        public ICommand Rename { get; private set; }

        public ModelBase(string name, string icon)
        {
            Name = name;
            Icon = icon;

            Rename = new RelayCommand(() =>
            {
                IsNameEditing = !IsNameEditing;
            });
        }

        public string Icon { get; set; }

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

        public string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                return this;

            else throw new NotImplementedException();
        }

        public object GetData(Type format)
        {
            if (Tformats.Contains(format))
                return this;

            else throw new NotImplementedException();
        }

        public object GetData(string format, bool autoConvert)
        {
            if (formats.Contains(format))
                return this;

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
