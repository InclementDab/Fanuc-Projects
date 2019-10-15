using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlzEx.Standard;
using DNC.Models;

namespace DNC.Communication
{
    
    public interface IConnection : IEquatable<IConnection>, IFormattable, IDisposable
    {
        ushort MachineHandle { get; }
        ConnectionStatus CurrentConnectionStatus { get; }
        void Connect();
        void Disconnect();
        bool SendProgram(Program program);
        bool VerifyProgram(Program program);

        event ConnectedEventHandler OnConnected;
        event DisconnectedEventHandler OnDisconnected;
    }

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
}
