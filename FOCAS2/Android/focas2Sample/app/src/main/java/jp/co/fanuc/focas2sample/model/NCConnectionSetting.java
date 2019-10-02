package jp.co.fanuc.focas2sample.model;

import android.os.Handler;
import android.os.HandlerThread;
import android.util.Log;

import java.io.Serializable;

import jp.co.fanuc.fwlibe1.Fwlibe1;

import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_OK;
import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_SOCKET;

/**
 * The entity class that is managed connection settings with NC.
 */
public class NCConnectionSetting implements Serializable {

    private static final long serialVersionUID = 1L;

    private String ncName;
    private String ipAddr;
    private short portNo;
    private int timeoutVal;

    private Short hndl;
    private HandlerThread thread;

    public StatusConverter converter;
    public short ret;
    public boolean connecting;

    /**
     * Connection setting
     */
    public NCConnectionSetting(String name, String ipAddress, short port, int timeout) {
        ncName = name;
        ipAddr = ipAddress;
        portNo = port;
        timeoutVal = timeout;
        thread = new HandlerThread("Thread:" + ncName);
        thread.start();
    }

    public int getTimeoutVal() {
        return timeoutVal;
    }

    public String getNcName() {
        return ncName;
    }

    public String getIpAddr() {
        return ipAddr;
    }

    public short getPortNo() {
        return portNo;
    }

    @Override
    public String toString() {
        return "NCConnectionSetting{" +
                "ncName='" + ncName + '\'' +
                ", ipAddr='" + ipAddr + '\'' +
                ", portNo=" + portNo +
                ", timeoutVal=" + timeoutVal +
                '}';
    }

    public Short getHndl() {
        return hndl;
    }

    public void setHndl(Short hndl) {
        this.hndl = hndl;
    }

    /**
     * Connect to NC and get handle.
     */
    public short connectNc() {
        if (hndl == null || ret == EW_SOCKET) {
            if (ret == EW_SOCKET && hndl != null) {
                Fwlibe1.cnc_freelibhndl(hndl);
            }
            hndl = new Short((short) 0);
            ret = Fwlibe1.cnc_allclibhndl3(ipAddr, portNo, timeoutVal, hndl);
            Log.d(NCUtility.getLogTagWithMethod(), "cnc_allclibhndl3 ret:" + ret + " hndl:" + hndl);
            if (ret != EW_OK) {
                hndl = null;
            }
        }else {
            ret = EW_OK;
        }
        return ret;
    }

    /**
     * Connect to NC.
     */
    public void active() {
        runOnThread(new Runnable() {

            @Override
            public void run() {
                connectNc();
            }
        });
    }

    /**
     * Release handle.
     */
    public void inactive() {
        runOnThread(new Runnable() {

            @Override
            public void run() {
                if (hndl != null) {
                    short ret = Fwlibe1.cnc_freelibhndl(hndl);
                    Log.d(NCUtility.getLogTagWithMethod(), "cnc_freelibhndl ret:" + ret);
                    hndl = null;
                }
                Fwlibe1.cnc_exitthread();
            }
        });
    }

    public void runOnThread(Runnable runnable) {
        Handler handler = new Handler(thread.getLooper());
        handler.post(runnable);
    }

    /**
     * Final dispose.
     */
    @Override
    protected void finalize() throws Throwable {
        try {
            super.finalize();
        } finally {
            if (thread != null) {
                Thread moribund = thread;
                thread = null;
                moribund.interrupt();
            }
        }
    }
}

