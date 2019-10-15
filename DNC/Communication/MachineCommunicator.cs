using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using static DNC.Focas2;
using static DNC.Serial;

namespace DNC.Communication
{
    public struct TCPConnection : IConnection
    {
        
        public ushort MachineHandle { get; private set; }
        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public struct SerialConnection : IConnection
    {
        public ushort MachineHandle { get; private set; }
        public void Open()
        {
            
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal interface IConnection : INotifyPropertyChanged
    {
        ushort MachineHandle { get; }
        void Open();
        void Close();
    }
}
