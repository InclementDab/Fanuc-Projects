using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DNC
{
    public class Serial
    {
        public enum rs_open_val
        {
            FCA_BAUD_1200 = 1200,
            FCA_BAUD_2400 = 2400,
            FCA_BAUD_4800 = 4800,
            FCA_BAUD_9600 = 9600,
            FCA_BAUD_19200 = 19200,
            FCA_BAUD_38400 = 38400,
            FCA_BAUD_57600 = 57600,
            FCA_BAUD_76800 = 76800
        }

        public enum parity
        {
            PARITY_N = 0,
            PARITY_E = 1,
            PARITY_O = 2
        }

        public enum rs_buffer_val
        {
            RS_CHK_BUF_R = 0,
            RS_CHK_BUF_W = 1,
            RS_CLR_BUF_R = 2,
            RS_CLR_BUF_W = 3,
            RS_GET_BUF_R = 4,
            RS_GET_BUF_W = 5
        }

        public enum rs_status_val
        {
            STS_CD_ON = 0x0002,
            STS_PARITY = 0x0008,
            STS_OVERRUN = 0x0010,
            STS_FRAME = 0x0020,
            STS_DR_ON = 0x0080,
            STS_OVERFLOW = 0x0100,
            STS_EMPTY = 0x0200,
            STS_R_STOP = 0x0800,
            STS_BUF_FULL = 0x2000,
            STS_S_STOP = 0x8000
        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public class PortDefUser
        {
            public int baud;       /* baud rate at which running	*/
            public int stop_bit;   /* stop bit(s)					*/
            public int parity;     /* parity bit(s)				*/
            public int data_bit;   /* Number of bits/byte			*/
            public int hardflow;   /* hardware flow control		*/
            public int dc_enable;  /* DC code flow control 		*/
            public int dc_put;     /* output DC code on open/close */
            public int dc1_code;   /* DC1 code number				*/
            public int dc2_code;   /* DC2 code number				*/
            public int dc3_code;   /* DC3 code number				*/
            public int dc4_code;   /* DC4 code number				*/
        }


        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public class FCA_DIRINFO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
            char file_name; /* file name (ASCZ string)			*/
                            /*	 first char. : 0xFF - not used	*/
                            /*				 : 0x00 - deleted	*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            char file_size;  /* file size (ASCZ string)			*/
            char wrt_protect;   /* write protect					*/
                                /*	'P' - on						*/
                                /*	' ' - off						*/
            char record_code;   /* recording code					*/
                                /*	'B' - binary					*/
                                /*	'E' - EIA						*/
                                /*	' ' - ISO						*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            char vol_no;     /* volume number (ASCZ string)		*/
                                /*	"  " - single volume			*/
            char multi_vol;     /* multi volume 					*/
                                /*	' ' - single volume 			*/
                                /*	'C' - continuous volume 		*/
                                /*	'L' - last volume				*/
        }



        [DllImport("FCA32.dll", EntryPoint = "rs_open")]
        public static extern int rs_open(int port, [In, MarshalAs(UnmanagedType.LPStruct)] PortDefUser param, [In, MarshalAs(UnmanagedType.HString)] object mode);

        [DllImport("FCA32.dll", EntryPoint = "rs_close")]
        public static extern int rs_close(int port);

        [DllImport("FCA32.dll", EntryPoint = "rs_putc")]
        public static extern int rs_putc(int c, int port);

        [DllImport("FCA32.dll", EntryPoint = "rs_write")]
        public static extern int rs_write([In, Out, MarshalAs(UnmanagedType.AsAny)] object buffer, int size, int port);

        [DllImport("FCA32.dll", EntryPoint = "rs_getc")]
        public static extern int rs_getc(int port);

        [DllImport("FCA32.dll", EntryPoint = "rs_read")]
        public static extern int rs_read([In, Out, MarshalAs(UnmanagedType.AsAny)] object buffer, int size, int port);

        [DllImport("FCA32.dll", EntryPoint = "rs_buffer")]
        public static extern int rs_buffer(int port, int cmnd);

        [DllImport("FCA32.dll", EntryPoint = "rs_status")]
        public static extern int rs_status(int port);

        [DllImport("FCA32.dll", EntryPoint = "fca_setparam")]
        public static extern int fca_setparam(int port, [In, Out, MarshalAs(UnmanagedType.AsAny)] PortDefUser param);

        [DllImport("FCA32.dll", EntryPoint = "fca_bye")]
        public static extern int fca_bye(int port);

        [DllImport("FCA32.dll", EntryPoint = "fca_open")]
        public static extern int fca_open([In, Out, MarshalAs(UnmanagedType.AsAny)] object name, int mode);

        [DllImport("FCA32.dll", EntryPoint = "fca_close")]
        public static extern int fca_close();

        [DllImport("FCA32.dll", EntryPoint = "fca_read")]
        public static extern int fca_read([In, Out, MarshalAs(UnmanagedType.AsAny)] object buffer, int bytes);

        [DllImport("FCA32.dll", EntryPoint = "fca_write")]
        public static extern int fca_write([In, Out, MarshalAs(UnmanagedType.AsAny)] object buffer, int bytes);

        [DllImport("FCA32.dll", EntryPoint = "fca_fopen")]
        public static extern int fca_fopen([In, Out, MarshalAs(UnmanagedType.AsAny)] object name, [In, Out, MarshalAs(UnmanagedType.AsAny)] object mode);

        [DllImport("FCA32.dll", EntryPoint = "fca_fclose")]
        public static extern int fca_fclose();

        [DllImport("FCA32.dll", EntryPoint = "fca_getc")]
        public static extern int fca_getc();

        [DllImport("FCA32.dll", EntryPoint = "fca_putc")]
        public static extern int fca_putc(int c);

        [DllImport("FCA32.dll", EntryPoint = "fca_readdir")]
        public static extern int fca_readdir([In, Out, MarshalAs(UnmanagedType.AsAny)] FCA_DIRINFO buffer, int ndir, int nfile);

        [DllImport("FCA32.dll", EntryPoint = "fca_rename")]
        public static extern int fca_rename([In, Out, MarshalAs(UnmanagedType.AsAny)] object oldname, [In, Out, MarshalAs(UnmanagedType.AsAny)] object newname);

        [DllImport("FCA32.dll", EntryPoint = "fca_delete")]
        public static extern int fca_delete([In, Out, MarshalAs(UnmanagedType.AsAny)] object name);

        [DllImport("FCA32.dll", EntryPoint = "fca_remains")]
        public static extern int fca_remains(long remains);

        [DllImport("FCA32.dll", EntryPoint = "fca_status")]
        public static extern int fca_status([In, Out, MarshalAs(UnmanagedType.AsAny)] object buffer);

    }
}
