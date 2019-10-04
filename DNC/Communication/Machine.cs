using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static DNC.Focas2;

namespace DNC
{
    public class Machine : IDisposable
    {
        private readonly ushort handle;
        public readonly int StatusCode = 0;
        public string Name;
        

        public Machine(IPAddress ip, ushort port, string name)
        {
            Name = name;
            StatusCode = cnc_allclibhndl3(ip.ToString(), port, 10, out handle);
        }


        public int PushProgram(Program program)
        {

        }


        public void Dispose()
        {
            cnc_freelibhndl(handle);
        }
    }
}
