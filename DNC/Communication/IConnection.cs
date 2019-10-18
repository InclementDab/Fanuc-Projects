using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using ControlzEx.Standard;
using DNC.Models;

using static DNC.Focas2;

namespace DNC.Communication
{
    public struct SerialConnection : IConnection
    {
        public IEnumerable<SerialPort> SerialPorts => SerialPort.GetPortNames().Select(name => new SerialPort(name));
        public SerialPort SerialPort { get; set; }
        private int SerialPortNumber
        {
            get
            {
                int.TryParse(Regex.Match(SerialPort.PortName, "(\\d+)").Value, out int comNum);
                return comNum;
            }
        }

        public short Connect(out ushort handle)
        {
            return cnc_allclibhndl2(SerialPortNumber, out handle);
        }

        public short Disconnect(ushort handle)
        {
            return cnc_freelibhndl(handle);
        }

        public bool TestConnection(out ushort handle, out short err)
        {
            err = cnc_allclibhndl2(SerialPortNumber, out handle);
            return err == 0;
        }
    }

    public struct TCPConnection : IConnection
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }

        public TCPConnection(IPAddress ip, int port)
        {
            IpAddress = ip;
            Port = port;
        }

        public short Connect(out ushort handle)
        {
            return cnc_allclibhndl3(IpAddress.ToString(), (ushort)Port, 10, out handle);
        }

        public short Disconnect(ushort handle)
        {
            return cnc_freelibhndl(handle);
        }

        public bool TestConnection(out ushort handle, out short err)
        {
            err = cnc_allclibhndl3(IpAddress.ToString(), (ushort)Port, 10, out handle);
            return err == 0;
        }
    }

    public interface IConnection
    {
        short Connect(out ushort handle);
        short Disconnect(ushort handle);
        bool TestConnection(out ushort handle, out short err);
    }

    [Serializable]
    public struct Connection : IDisposable
    {
        public ushort Handle;
        public IConnection BaseConnection;
        public IConnection GetBaseConnection => BaseConnection;
        public ConnectionStatus Status { get; private set; }
        public bool IsConnected => Status == ConnectionStatus.Connected;

        public Connection(IConnection connection)
        {
            BaseConnection = connection;
            Status = ConnectionStatus.Disconnected;

            Handle = 0;
            Connected = null;
            Disconnected = null;
        }

        public void Connect()
        { 
            BaseConnection.Connect(out Handle);
            OnDisconnected(new ConnectionChangedEventArgs(BaseConnection));
        }

        public void Disconnect()
        {
            BaseConnection.Disconnect(Handle);
            OnDisconnected(new ConnectionChangedEventArgs(BaseConnection));
        }

        public void Dispose()
        {
            BaseConnection.Disconnect(Handle);
        }

        private void OnConnected(ConnectionChangedEventArgs e)
        {
            Status = ConnectionStatus.Connected;
            Connected?.Invoke(this, e);
        }

        private void OnDisconnected(ConnectionChangedEventArgs e)
        {
            Status = ConnectionStatus.Disconnected;
            Disconnected?.Invoke(this, e);
        }

        public event ConnectedEventHandler Connected;
        public event DisconnectedEventHandler Disconnected;
    }


    /*

    public interface IConnection : IEquatable<IConnection>, IFormattable, IDisposable
    {
        ushort MachineHandle { get; }
        ConnectionStatus CurrentConnectionStatus { get; }
        bool IsConnected { get; }
        string ActiveDirectory { get; }
        void Connect();
        void Disconnect();
        bool SendProgram(Program program);
        bool VerifyProgram(Program program);
        void ReadMachineDirectory();

        event ConnectedEventHandler OnConnected;
        event DisconnectedEventHandler OnDisconnected;
    }
    */

    public delegate void ConnectedEventHandler(object sender, ConnectionChangedEventArgs e);
    public delegate void DisconnectedEventHandler(object sender, ConnectionChangedEventArgs e);

    public class ConnectionChangedEventArgs : EventArgs
    {
        public IConnection Connection;

        internal ConnectionChangedEventArgs(IConnection connection)
        {
            Connection = connection;
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

    public class ConnectionException : Exception
    {
        public readonly short StatusCode;
        public readonly string StatusType;
        public readonly IConnection Connection;

        public ConnectionException(IConnection connection, short statusCode)
        {
            StatusCode = statusCode;
            StatusType = ((Focas2.focas_ret) statusCode).ToString();
            Connection = connection;
        }
    }

    public class MachineException : Exception
    {
        public readonly Machine CurrentMachine;
        
        public MachineException() { }
        public MachineException(Machine currentMachine)
        {
            CurrentMachine = currentMachine;
        }
    }

    public class ConnectionColorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(ConnectionStatus);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return typeof(Brush).Assembly.Equals(destinationType.Assembly); // gotta use System.Windows.Media, NOT drawing
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {

            if (!(value is Brush b)) return null;

            if (Equals(Brushes.Red, b)) return ConnectionStatus.Disconnected;
            if (Equals(Brushes.Green, b)) return ConnectionStatus.Connected;

            return 3;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is ConnectionStatus status)) return null;


            switch (status)
            {
                case ConnectionStatus.Disconnected:
                    return Brushes.Red;

                case ConnectionStatus.Connected:
                    return Brushes.Green;

                default:
                    return Brushes.Yellow;
            }
        }
    }
}
