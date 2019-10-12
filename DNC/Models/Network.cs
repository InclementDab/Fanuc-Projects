using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;
using static DNC.Focas2;
using static DNC.Serial;


namespace DNC.Models
{


    [Description("TCP/IP")]
    [Serializable]
    public class TCPConnection : Connection
    {
        private string _IPAddress;
        public IPAddress IPAddress
        {
            get => IPAddress.Parse(_IPAddress ?? "0.0.0.0");
            set
            {
                _IPAddress = value.ToString();
            }
        }

        public int Port { get; set; }

        internal TCPConnection() { }

        public TCPConnection(IPAddress ip, int port)
        {
            IPAddress = ip;
            Port = port;
        }

        public override short Open(out ushort handle)
        {
            Debug.WriteLine("Opening Connection");
            Status = ConnectionStatus.Connecting;

            StatusCode = cnc_allclibhndl3(IPAddress.ToString(), (ushort)Port, 10, out handle);
            Status = StatusCode == 0 ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
            OnConnectionChanged(handle);
            return StatusCode;
        }
    }

    [Description("Serial Port")]
    [Serializable]
    public class SerialConnection : Connection
    {
        private ObservableCollection<string> serialPorts;
        public ObservableCollection<string> SerialPorts
        {
            get => serialPorts;
            set
            {
                serialPorts = value;
                RaisePropertyChanged();
            }
        }
        public string _SerialPort { get; set; }

        public SerialConnection()
        {
            SerialPorts = new ObservableCollection<string>();

            foreach (string port in SerialPort.GetPortNames())
                SerialPorts.Add(port);


            if (SerialPorts.Count != 0)
                _SerialPort = SerialPorts.First();
        }

        public override short Open(out ushort handle)
        {
            Debug.WriteLine("Opening Connection");
            Status = ConnectionStatus.Connecting;

            
            int.TryParse(Regex.Match(_SerialPort, "(\\d+)").Value, out int comNum);

            PortDefUser p = new PortDefUser()
            {
                baud = 9600,
                stop_bit = 1,
                parity = 1,
                data_bit = 7,
                hardflow = 0,
                dc_enable = 0,
                dc_put = 1,
                dc1_code = 0x11,
                dc2_code = 0x12,
                dc3_code = 0x13, // maybe 0x93
                dc4_code = 0x14
            };

            //StatusCode = (short)rs_open(comNum, p, "rw");
            //Debug.WriteLine("PUT: " + rs_putc(1, comNum));
            //Debug.WriteLine("BUF: " + rs_buffer(comNum, rs_buffer_val.RS_CHK_BUF_W));
            handle = 0;


            SerialPort sPort = new SerialPort("COM1");
            Debug.WriteLine($"Opening Port {sPort.PortName}");


            if (sPort.IsOpen) sPort.Close();
            sPort.DataReceived += SerialPort_DataReceived;
            sPort.Disposed += SerialPort_Disposed;
            sPort.PinChanged += SerialPort_PinChanged;

            sPort.BaudRate = 9600;
            sPort.Parity = Parity.Even;
            sPort.StopBits = StopBits.One;
            sPort.DataBits = 7;
            sPort.Handshake = Handshake.XOnXOff;
            sPort.ReadTimeout = 2000;





            //sPort.DtrEnable = true;
            sPort.Open();

            sPort.RtsEnable = true;

            //sPort.Write("Test");
            //StatusCode = cnc_allclibhndl2(comNum, out handle);
            //Status = sPort.CDHolding ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
            Status = StatusCode == 0 ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
            OnConnectionChanged(handle);
            return StatusCode;
        }

        private void SerialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            if (e.EventType == SerialPinChange.Break) return;
            Debug.WriteLine($"Pin Changed: {e.EventType}");
        }

        private void SerialPort_Disposed(object sender, EventArgs e)
        {
            if (sender is SerialPort sPort)
                Debug.WriteLine($"{sPort.PortName} Disposed");
        }

        public List<char> AllRecieved = new List<char>();

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender is SerialPort sPort)
            {
                try
                {
                    while (sPort.BytesToRead > 0)
                    {
                        char[] buffer = new char[sPort.BytesToRead];
                        sPort.Read(buffer, 0, buffer.Length);
                        AllRecieved.AddRange(buffer.ToList());
                    }

                }
                catch (IOException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }


    }

    [Description("Generic Connection")]
    [Serializable]
    [XmlInclude(typeof(TCPConnection))]
    [XmlInclude(typeof(SerialConnection))]
    public abstract class Connection : ObservableObject
    {
        private ConnectionStatus _status = 0;
        public ConnectionStatus Status
        {
            get => _status;
            protected set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }

        public short StatusCode { get; protected set; }
        public bool IsConnected => Status == ConnectionStatus.Connected;

        public abstract short Open(out ushort handle);
        public short Close(ushort handle)
        {
            Debug.WriteLine("Closing Connection");
            Status = ConnectionStatus.Disconnecting;

            StatusCode = cnc_freelibhndl(handle);
            Status = StatusCode == 0 ? ConnectionStatus.Disconnected : ConnectionStatus.Connected;
            OnConnectionChanged(handle);
            return StatusCode;
        }

        public delegate void ConnectionChangedEventHandler(object sender, ConnectionChangedEventArgs e);
        public event ConnectionChangedEventHandler ConnectionChanged;

        protected virtual void OnConnectionChanged(ushort handle)
        {
            ConnectionChangedEventArgs args = new ConnectionChangedEventArgs(Status, StatusCode, handle);
            ConnectionChangedEventHandler h = ConnectionChanged;
            h?.Invoke(this, args);
        }
    }

    public class ConnectionChangedEventArgs : EventArgs
    {
        public readonly ushort Handle;
        public readonly ConnectionStatus Status;
        public readonly short StatusCode;

        public DateTime TimeChanged { get; protected set; }

        public ConnectionChangedEventArgs(ConnectionStatus status, short statusCode, ushort handle)
        {
            Status = status;
            StatusCode = statusCode;
            Handle = handle;
            TimeChanged = DateTime.Now;
        }
    }

    [TypeConverter(typeof(ConnectionColorConverter))]
    public enum ConnectionStatus
    {
        [Description("Disconnected")]
        Disconnected = 0,

        [Description("Connected")]
        Connected = 1,

        [Description("Connecting...")]
        Connecting = 2,

        [Description("Disconnecting...")]
        Disconnecting = 3
    }

    public class ConnectionColorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(ConnectionStatus);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Brush);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {

            if (!(value is Brush b)) return null;

            if (Equals(Brushes.Red, b)) return ConnectionStatus.Disconnected;
            if (Equals(Brushes.Yellow, b)) return ConnectionStatus.Connecting;
            if (Equals(Brushes.Green, b)) return ConnectionStatus.Connected;

            return Brushes.Pink;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is ConnectionStatus status)) return null;


            switch (status)
            {
                case ConnectionStatus.Disconnected:
                    return Brushes.Red;

                case ConnectionStatus.Connecting:
                    return Brushes.Yellow;

                case ConnectionStatus.Connected:
                    return Brushes.Green;

                default:
                    return Brushes.Gray;
            }
        }
    }
}
