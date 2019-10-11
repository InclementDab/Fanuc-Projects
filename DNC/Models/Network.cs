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

using static DNC.Focas2;
using static DNC.Serial;


namespace DNC.Models
{

    [Description("TCP/IP")]
    public class TCPConnection : Connection
    {
        public IPAddress IPAddress { get; set; }
        public ushort Port { get; set; }

        public TCPConnection() { }

        public TCPConnection(IPAddress ip, ushort port)
        {
            IPAddress = ip;
            Port = port;
        }

        public override short Open(out ushort handle)
        {
            Debug.WriteLine("Opening Connection");
            Status = ConnectionStatus.Connecting;

            StatusCode = cnc_allclibhndl3(IPAddress.ToString(), Port, 10, out handle);
            Status = StatusCode == 0 ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
            OnConnectionChanged(handle);
            return StatusCode;
        }
    }

    [Description("Serial Port")]
    public class SerialConnection : Connection
    {
        private ObservableCollection<SerialPort> serialPorts;
        public ObservableCollection<SerialPort> SerialPorts
        {
            get => serialPorts;
            set
            {
                serialPorts = value;
                RaisePropertyChanged();
            }
        }
        public SerialPort SerialPort { get; set; }

        public SerialConnection()
        {
            SerialPorts = new ObservableCollection<SerialPort>();

            foreach (string port in SerialPort.GetPortNames())
                SerialPorts.Add(new SerialPort(port));


            if (SerialPorts.Count != 0)
                SerialPort = SerialPorts.First();
        }

        public override short Open(out ushort handle)
        {
            Debug.WriteLine("Opening Connection");
            Status = ConnectionStatus.Connecting;

            if (SerialPort.IsOpen) SerialPort.Close();
            int.TryParse(Regex.Match(SerialPort.PortName, "(\\d+)").Value, out int comNum);

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

            Debug.WriteLine($"Opening Port {SerialPort.PortName}");

            SerialPort.DataReceived += SerialPort_DataReceived;
            SerialPort.Disposed += SerialPort_Disposed;
            SerialPort.PinChanged += SerialPort_PinChanged;

            SerialPort.BaudRate = 9600;
            SerialPort.Parity = Parity.Even;
            SerialPort.StopBits = StopBits.One;
            SerialPort.DataBits = 7;
            SerialPort.Handshake = Handshake.XOnXOff;
            SerialPort.ReadTimeout = 2000;



            

            //SerialPort.DtrEnable = true;
            SerialPort.Open();
            
            SerialPort.RtsEnable = true;

            //SerialPort.Write("Test");
            //StatusCode = cnc_allclibhndl2(comNum, out handle);
            //Status = SerialPort.CDHolding ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
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
