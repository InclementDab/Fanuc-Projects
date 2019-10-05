using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static DNC.Focas2;

namespace DNC
{
    public class Machine : IDisposable
    {
        
        private short statusCode;
        public short StatusCode
        {
            get => statusCode;
            set
            {
                statusCode = value;

                if (statusCode != 0)
                    Debug.WriteLine(statusCode);
            }
        }

        private ushort handle;
        public string Name;
        public IPAddress IPAddress;
        public int Port;

        public bool IsConnected;

        public readonly ODBSYSEX SystemInfo = new ODBSYSEX();
        public readonly ODBST StatInfo = new ODBST();


        public Machine(IPAddress ip, int port, string name)
        {
            Name = name;
            IPAddress = ip;
            Port = port;
        }

        public bool Connect()
        {
            StatusCode = cnc_allclibhndl3(IPAddress.ToString(), (ushort)Port, 10, out handle);

            if (StatusCode == 0)
            {
                StatusCode = cnc_statinfo(handle, StatInfo);
                StatusCode = cnc_sysinfo_ex(handle, SystemInfo);
                IsConnected = true;
                return IsConnected;
            }
            else return false;
        }

        public bool Connect(IPAddress ip, int port)
        {

            StatusCode = cnc_allclibhndl3(ip.ToString(), (ushort)port, 10, out handle);

            if (StatusCode == 0)
            {
                StatusCode = cnc_statinfo(handle, StatInfo);
                StatusCode = cnc_sysinfo_ex(handle, SystemInfo);
                IsConnected = true;
                return IsConnected;
            }
            else return false;
        }

        public bool Disconnect()
        {
            StatusCode = cnc_freelibhndl(handle);
            IsConnected = false;
            return IsConnected;
        }


        [HandleProcessCorruptedStateExceptions]
        public void ReadDirectory()
        {
            try
            {
                ODBPDFDRV drvOut = new ODBPDFDRV();
                StatusCode = cnc_rdpdf_drive(handle, drvOut);

                List<object> drives = new List<object>();

                for (int i = 1; i <= drvOut.max_num; i++)
                    drives.Add(drvOut.GetType().GetField("drive" + i).GetValue(drvOut) as object); // this outputs CNC_MEM, and DATA_SV, DATA_SV throws memory error :(


                ODBPDFNFIL dirInfo = new ODBPDFNFIL();
                StatusCode = cnc_rdpdf_subdirn(handle, $"//CNC_MEM/", dirInfo);

                short dirNum = dirInfo.dir_num;
                for (short j = 0; j < dirInfo.dir_num; j++)
                {
                    IDBPDFSDIR subIn = new IDBPDFSDIR()
                    {
                        path = $"//CNC_MEM/",
                        req_num = j
                    };
                    ODBPDFSDIR subOut = new ODBPDFSDIR();

                    StatusCode = cnc_rdpdf_subdir(handle, ref dirNum, subIn, subOut);
                    Debug.WriteLine(ToJson(subOut));
                }

                short dsvInfo = 0;
                StatusCode = cnc_dtsvgetmode(handle, out dsvInfo);

                Debug.WriteLine(ToJson(dsvInfo + 100));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }



        public int PushProgram(Program program)
        {
            string prg = "\nO1234\nG1F0.3W10.\nM30\n%";
            StatusCode = cnc_dwnstart4(handle, 0, "//CNC_MEM/USER/");
            int l = prg.Length;
            StatusCode = cnc_download4(handle, ref l, prg);
            StatusCode = cnc_dwnend4(handle);

            return StatusCode;
        }

        public void Dispose()
        {
            StatusCode = cnc_freelibhndl(handle);
        }

        public static string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }


}
