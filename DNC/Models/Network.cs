using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
        public ObservableCollection<SerialPort> SerialPorts = new ObservableCollection<SerialPort>();
        public SerialPort SerialPort { get; set; }

        public SerialConnection()
        {
            foreach (string portName in SerialPort.GetPortNames())
                SerialPorts.Add(new SerialPort(portName));

            if (SerialPorts.Count != 0)
                SerialPort = SerialPorts.First();
        }
        
        public override short Open(out ushort handle)
        {
            Debug.WriteLine("Opening Connection");
            Status = ConnectionStatus.Connecting;

            int.TryParse(Regex.Match(SerialPort.PortName, "(\\d+)").Value, out int comNum);

            StatusCode = cnc_allclibhndl2(comNum, out handle);
            Status = StatusCode == 0 ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
            OnConnectionChanged(handle);
            return StatusCode;
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
