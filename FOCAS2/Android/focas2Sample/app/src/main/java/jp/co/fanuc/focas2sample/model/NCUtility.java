package jp.co.fanuc.focas2sample.model;

import jp.co.fanuc.fwlibe1.Fwlibe1;
import jp.co.fanuc.fwlibe1.IODBPMC;
import jp.co.fanuc.fwlibe1.ODBST2;

import static android.R.attr.handle;
import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_OK;
import static jp.co.fanuc.fwlibe1.Fwlibe1.pmc_rdpmcrng;

/*
* Tool class
* */
public class NCUtility {
    static Byte[] G42 = {0x7F, 0x7F, 0x7F, 0x7F, 0x7F, 0x7F, 0x7F, 0x7F, (byte) 0x80, 0x7F};
    static Byte[] G43 = {0x03, 0x01, 0x00, 0x04, 0x05, 0x07, 0x06, 0x21, 0x01, (byte) 0x85};
    static short[] aut = {3/*EDIT*/, 1/*MEM*/, 0/*MDI*/, 4/*HNDL*/, 5/*JOG*/, 7/*THNDL*/, 6/*TJOG*/, 10/*RMT*/, 10/*RMT*/, 9/*REF*/};

    /**
     * Thread name and file name and method name dump as tab of log .
     */
    public static String getLogTagWithMethod() {
        Throwable stack = new Throwable().fillInStackTrace();
        StackTraceElement[] trace = stack.getStackTrace();
        return Thread.currentThread().getName() + ":" + trace[1].getFileName() + "." + trace[1].getMethodName() + ":" + trace[1].getLineNumber();
    }
}
