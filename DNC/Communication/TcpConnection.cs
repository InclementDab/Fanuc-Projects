using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static DNC.Focas2;

namespace DNC.Communication
{
    [Serializable]
    public class TCPConnection : IConnection
    {
        private short ReturnCode
        {
            set
            {
                if (value != 0) throw new ConnectionException(this, value);
            }
        }

        public ushort MachineHandle => Handle;
        private ushort Handle; // has to be field because of out param
        public ConnectionStatus CurrentConnectionStatus { get; private set; }

        private IPAddress _ipAddress;
        public IPAddress IpAddress
        {
            get => _ipAddress;
            set
            {
                if (CurrentConnectionStatus == ConnectionStatus.Disconnected)
                    _ipAddress = value;

                else throw new InvalidOperationException("Cannot change IP while Connection is Open");
            }
        }

        private int _port;
        public int Port
        {
            get => _port;
            set
            {
                if (CurrentConnectionStatus == ConnectionStatus.Disconnected)
                    _port = value;

                else throw new InvalidOperationException($"Cannot change Port while Connection is Open");
            }
        }

        
        public void Connect()
        {
            CurrentConnectionStatus = ConnectionStatus.Connecting;
            ReturnCode = cnc_allclibhndl3(IpAddress.ToString(), (ushort)Port, 10, out Handle);
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

        public bool Equals(IConnection other)
        {
            throw new NotImplementedException();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }


}
