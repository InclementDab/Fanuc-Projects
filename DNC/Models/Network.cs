using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DNC.Models
{


    public class Connection
    {
        public ConnectionStatus Status { get; set; }
        public ConnectionType Type { get; set; }

#region TCP
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
#endregion

#region Serial
        public string ComPort { get; set; }
        public ObservableCollection<string> ComPorts { get; set; }
#endregion

        public Connection()
        {
            ComPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
        }

    }

    public enum ConnectionStatus
    {
        [Description("Disconnected")]
        Disconnected = 0,

        [Description("Connecting...")]
        Connecting = 1,

        [Description("Connected")]
        Connected = 2
    }

    public enum ConnectionType
    {
        [Description("Serial Port")]
        Serial = 0,

        [Description("TCP/IP")]
        TCP = 1
    }
}
