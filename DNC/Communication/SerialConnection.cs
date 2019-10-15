using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DNC.Focas2;

namespace DNC.Communication
{
    // best bet might be to change this to an internal struct and include it in a Public Class SerialConnection : (struct) serialConnection
    // since im not sure if you can have a parameterless constructer here, so it might not be serializable

    [Serializable]
    public struct SerialConnection : IConnection
    {
        private short ReturnCode
        {
            set
            {
                if (value != 0) throw new ConnectionException(this, value);
            }
        }

        public ushort MachineHandle => Handle;

        // Bug: Handle on Initialization will be set to zero. After initial connection it will be set. Unsure how to approach this
        private ushort Handle;
        public ConnectionStatus CurrentConnectionStatus { get; private set; }

        private SerialPort _serialPort;
        public SerialPort SerialPort
        {
            get => _serialPort;
            set
            {
                if (CurrentConnectionStatus == ConnectionStatus.Disconnected)
                    _serialPort = value;

                else throw new InvalidOperationException("Cannot change Serial Port while Connection is Open");
            }
        }

        public void Connect()
        {
            CurrentConnectionStatus = ConnectionStatus.Connecting;
            ReturnCode = cnc_allclibhndl2(1, out Handle);
            CurrentConnectionStatus = ConnectionStatus.Connected;
            OnConnected?.Invoke(this, new ConnectionChangedEventArgs(this));
        }

        public void Disconnect()
        {
            CurrentConnectionStatus = ConnectionStatus.Disconnecting;
            ReturnCode = cnc_freelibhndl(Handle);
            CurrentConnectionStatus = ConnectionStatus.Disconnected;
            OnDisconnected?.Invoke(this, new ConnectionChangedEventArgs(this));
        }

        public bool SendProgram(Program program)
        {
            throw new NotImplementedException();
        }

        public bool VerifyProgram(Program program)
        {
            throw new NotImplementedException();
        }

        #region events
        public event ConnectedEventHandler OnConnected;
        public event DisconnectedEventHandler OnDisconnected;
        #endregion

        #region interface implementation
        // Use this to check all machines in list to make sure you arent making a duplicate connection :)
        public bool Equals(IConnection other)
        {
            return Handle == other?.MachineHandle && CurrentConnectionStatus == other.CurrentConnectionStatus;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Handle.GetHashCode() * 397) ^ (int)CurrentConnectionStatus + 1;
            }
        }

        public void Dispose()
        {
            if (CurrentConnectionStatus != ConnectionStatus.Disconnected)
            {
                Disconnect();
            }
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"Type: {GetType()}, Port: {SerialPort.PortName}, Status: {CurrentConnectionStatus}";
        }

        #endregion
    }
}
