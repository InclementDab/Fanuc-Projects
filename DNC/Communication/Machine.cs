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

        private short statusCode;
        public short StatusCode
        {
            get => statusCode;
            set
            {
                statusCode = value;

                Debug.WriteLine(statusCode);
            }
        }
        public string Name;
        

        public Machine(IPAddress ip, ushort port, string name)
        {
            Name = name;
            StatusCode = cnc_allclibhndl3(ip.ToString(), port, 10, out handle);

            //PRGDIR prgd = new PRGDIR();
            //StatusCode = cnc_rdprogdir(handle, 0, 0, 9999, 1024, prgd);
            //string prog = new string(prgd.prg_data);
            //Debug.WriteLine(prog);


            string program = "\nO1234\nG1F0.3W10.\nM30\n%";
            StatusCode = cnc_dwnstart4(handle, 0, "//CNC_MEM/USER/");

            if (StatusCode == 5)
            {
                ODBERR err = new ODBERR();
                
                Debug.WriteLine(cnc_getdtailerr(handle, err));
                Debug.WriteLine($"N:{err.err_no} D:{err.err_dtno}");
            }

            int l = program.Length;
            StatusCode = cnc_download4(handle, ref l, program);

            StatusCode = cnc_dwnend4(handle);

        }


        public int PushProgram(Program program)
        {

            StatusCode = cnc_dwnstart(handle);
            Debug.WriteLine(StatusCode);

            string data = "\nO1234\nG0G90G54\nM30\n %";
            StatusCode = cnc_download(handle, data, (short)data.Length);
            Debug.WriteLine(StatusCode);

            StatusCode = cnc_dwnend(handle);
            Debug.WriteLine(StatusCode);
            return 0;
        }


        public void Dispose()
        {
            cnc_freelibhndl(handle);
        }
    }
}
