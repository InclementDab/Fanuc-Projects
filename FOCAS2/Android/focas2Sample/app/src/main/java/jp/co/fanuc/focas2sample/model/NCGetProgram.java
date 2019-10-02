package jp.co.fanuc.focas2sample.model;

import android.content.Context;
import android.util.Log;

import java.util.ArrayList;
import java.util.List;

import jp.co.fanuc.fwlibe1.Fwlibe1;
import jp.co.fanuc.fwlibe1.IDBPDFADIR;
import jp.co.fanuc.fwlibe1.ODBERR;
import jp.co.fanuc.fwlibe1.ODBPDFADIR;

import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_BUFFER;
import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_NUMBER;
import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_OK;

/**
 * The entity class that is NC program information.
 */
public class NCGetProgram {
    private short EDIT = 0;
    private short MAX_ALL_DIR_COUNT = 10;

    public ODBERR err = new ODBERR();
    private NCConnectionSetting mConnect;
    private String viewDir[] = new String[1];

    public NCGetProgram(NCConnectionSetting connect) {
        mConnect = connect;
        viewDir[0] = "//CNC_MEM/";
        short dir_kind = 1;
    }

    /**
     * Get CNC directory.
     */
    public void getCNCdir(String dir_name[]) {
        dir_name[0] = viewDir[0];
        return;
    }

    /**
     * Get below folder information.
     */
    public short getAllDir(Context context, String curDir, boolean bisTop, boolean isOnlyDir, List<NCDirInfo> dicArray) {
        Short num_dir = new Short(MAX_ALL_DIR_COUNT);
        IDBPDFADIR pin = new IDBPDFADIR();
        ODBPDFADIR[] pout = new ODBPDFADIR[num_dir];
        pin.setPath(curDir);
        pin.setSize_kind((short) 2);
        pin.setType((short) 1);

        // Get file list with CNC API.
        mConnect.ret = Fwlibe1.cnc_rdpdf_alldir(mConnect.getHndl(), num_dir, pin, pout);
        if (mConnect.ret != EW_OK && mConnect.ret != EW_NUMBER) {
            Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
            Log.e(NCUtility.getLogTagWithMethod(), "cnc_rdpdf_alldir ret:" + mConnect.ret + " err:" + err);
            return mConnect.ret;
        }

        if (mConnect.ret == EW_NUMBER) {
            // There is no subfolder for req_num.
            num_dir = Short.valueOf((short) 0);
        }

        // If no top directory, initialize directory information as "Return to upper folder".
        if (!bisTop) {
            NCDirInfo info = new NCDirInfo();
            info.setUpFolder(context);
            dicArray.add(info);
        }
        // Create dir information by getting date.
        for (int i = 0; i < num_dir; i++) {
            ODBPDFADIR dirInfo = pout[i];
            // For Directory only
            if (isOnlyDir) {
                if (0 != dirInfo.getData_kind()) {
                    continue;
                }
            }
            NCDirInfo info = new NCDirInfo();
            info.setData_kind(dirInfo.getData_kind());
            info.setYear(dirInfo.getYear());
            info.setMon(dirInfo.getMon());
            info.setDay(dirInfo.getDay());
            info.setHour(dirInfo.getHour());
            info.setMin(dirInfo.getMin());
            info.setSec(dirInfo.getSec());
            info.setSize(dirInfo.getSize());
            info.setAttr(dirInfo.getAttr());
            info.setD_f(dirInfo.getD_f());
            info.setComment(dirInfo.getComment());
            info.setO_time(dirInfo.getO_time());

            dicArray.add(info);
        }

        return mConnect.ret;
    }

    /**
     * Set specified directory as current directory.
     */
    public void moveCNCdir(String dir) {
        viewDir[0] = dir;
        return;
    }

    /**
     * Upload NC program by specified program name.
     */
    public short uploadNCProgram(String progName, String[] progData) {
        String buf[] = new String[1];
        Integer len;
        short type = 0;
        StringBuffer prog = new StringBuffer();
        // Start uploading NC program with CNC API.
        mConnect.ret = Fwlibe1.cnc_upstart4(mConnect.getHndl(), type, progName);

        if (mConnect.ret != EW_OK) {
            Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
            Log.e(NCUtility.getLogTagWithMethod(), "cnc_upstart4 ret:" + mConnect.ret + " err:" + err);
            return mConnect.ret;
        }

        do {
            len = new Integer(1280);
            // Uploading NC program with CNC API.
            mConnect.ret = Fwlibe1.cnc_upload4(mConnect.getHndl(), len, buf);

            if (mConnect.ret == EW_BUFFER) {
                continue;
            }
            if (mConnect.ret == EW_OK) {
                if (len > 0) {
                    prog.append(buf[0],0,len);
                    // End of NC program if existing "%".
                    if (buf[0].substring(len-1).startsWith("%")) {
                        break;
                    }
                }
            }

        } while ((mConnect.ret == EW_OK) || (mConnect.ret == EW_BUFFER));

        // End uploading NC program with CNC API.
        mConnect.ret = Fwlibe1.cnc_upend4(mConnect.getHndl());
        if (mConnect.ret != EW_OK) {
            Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
            Log.e(NCUtility.getLogTagWithMethod(), "cnc_upend4 ret:" + mConnect.ret + " err:" + err);
            return mConnect.ret;
        }
        progData[0] = prog.toString();
        return mConnect.ret;
    }

    /**
     * Download NC program by specified program name.
     */
    public short downloadNCProgram(String dir, String progName, String progData) {
        boolean isSelect[] = new boolean[1];

        String path = dir + progName;

        short type = 0;
        // Start Downloading NC program with CNC API.
        mConnect.ret = Fwlibe1.cnc_dwnstart4(mConnect.getHndl(), type, dir);
        if (mConnect.ret != EW_OK) {
            // Download failure.
            Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
            Log.e(NCUtility.getLogTagWithMethod(), "cnc_dwnstart4 ret:" + mConnect.ret + " err:" + err);
            return mConnect.ret;
        }
        String prg = progData;
        int n = 0;
        int len = prg.length();

        // Continue downloading NC program until nothing them.
        while (len > 0) {
            n = Integer.valueOf(len);
            // Downloading NC program with CNC API.
            mConnect.ret = Fwlibe1.cnc_download4(mConnect.getHndl(), n, prg);
            if (mConnect.ret == EW_BUFFER) {
                continue;
            }

            if (mConnect.ret == EW_OK) {
                prg = prg.substring(n);
                len = len - n;
            }
            if (mConnect.ret != EW_OK) {
                break;
            }
        }
        // End Downloading NC program with CNC API.
        mConnect.ret = Fwlibe1.cnc_dwnend4(mConnect.getHndl());
        if (mConnect.ret != EW_OK) {
            // Download start failure
            Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
            Log.e(NCUtility.getLogTagWithMethod(), "cnc_dwnend4 ret:" + mConnect.ret + " err:" + err);
            return mConnect.ret;
        }

        return mConnect.ret;
    }

}

