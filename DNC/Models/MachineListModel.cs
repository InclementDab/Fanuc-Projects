using DNC.ViewModels;
using DNC.Views;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static DNC.Focas2;

namespace DNC.Models
{

    public class Machine : ModelBase, IDisposable, IDataObject
    {

        private ushort handle;
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }

        public bool ConnectOnStartup { get; set; }
        public string IsConnectedString => IsConnected ? "Disconnect" : "Connect";

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                NotifyPropertyChanged("IsConnectedString");
            }
        }

        public ObservableCollection<Program> ProgramList { get; private set; }

        #region Commands

        public ICommand Connect { get; private set; }
        public ICommand Edit { get; private set; }

        #endregion


        public readonly ODBSYSEX SystemInfo = new ODBSYSEX();
        public readonly ODBST StatInfo = new ODBST();

        public Machine(string name) : base(name, "/Resources/Icons/Machine_16x.png")
        {

            ProgramList = new ObservableCollection<Program>();
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

                foreach (PropertyInfo pInfo in GetType().GetProperties())
                {
                    //var x = pInfo.GetValue(m);
                    //GetType().GetRuntimeProperty(pInfo.Name).SetValue(pInfo, x);
                    //m.GetType().GetProperty(pInfo.Name).SetValue(this, pInfo.GetValue(m));
                }
            });
        }

        private short statusCode;
        public short StatusCode
        {
            get => statusCode;
            set
            {
                statusCode = value;
                Debug.WriteLine(statusCode);
            }
        }


        public void OpenConnection()
        {
            StatusCode = cnc_allclibhndl3(IPAddress.ToString(), (ushort)Port, 10, out handle);

            if (StatusCode == 0)
            {
                StatusCode = cnc_statinfo(handle, StatInfo);
                StatusCode = cnc_sysinfo_ex(handle, SystemInfo);
                IsConnected = true;
            }
        }

        public void CloseConnection()
        {
            StatusCode = cnc_freelibhndl(handle);
            IsConnected = (StatusCode != 0);
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
            string prg = "\nO1234\nG1F0.3W10.\nM30\n%";
            StatusCode = cnc_dwnstart4(handle, 0, "//CNC_MEM/USER/");
            int l = prg.Length;
            StatusCode = cnc_download4(handle, ref l, prg);
            StatusCode = cnc_dwnend4(handle);

            return StatusCode;
        }

        public void Dispose()
        {
            if (IsConnected)
                StatusCode = cnc_freelibhndl(handle);
        }

        public static string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public void SetData(object data)
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
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        public void SetData(object data)
        {
            Children.Add(data as ModelBase);
        }
    }

    public abstract class ModelBase : INotifyPropertyChanged
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

        public string[] formats = new string[]
        {
            "DNC.Models.ModelBase",
            "DNC.Models.Machine",
            "DNC.Models.Folder"
        };

        public Type[] Tformats = new Type[]
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



        public void SetData(string format, object data)
        {
            throw new NotImplementedException();
        }

        public void SetData(Type format, object data)
        {
            throw new NotImplementedException();
        }

        public void SetData(string format, object data, bool autoConvert)
        {
            throw new NotImplementedException();
        }
    }
}
