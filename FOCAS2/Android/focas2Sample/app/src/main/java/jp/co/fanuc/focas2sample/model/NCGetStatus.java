package jp.co.fanuc.focas2sample.model;

import android.util.Log;

import java.io.UnsupportedEncodingException;
import java.util.Arrays;

import jp.co.fanuc.fwlibe1.Fwlibe1;
import jp.co.fanuc.fwlibe1.ODBALMMSG2;
import jp.co.fanuc.fwlibe1.ODBDGN;
import jp.co.fanuc.fwlibe1.ODBERR;
import jp.co.fanuc.fwlibe1.ODBST2;
import jp.co.fanuc.fwlibe1.ODBSYSEX;
import jp.co.fanuc.fwlibe1.OPMSG3;

import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_OK;

/**
 * The entity class that acquire and save the connected state.
 */
public class NCGetStatus {
    private NCConnectionSetting mConnect;

    public NCGetStatus(NCConnectionSetting con) {
        mConnect = con;
    }

    final String ALM_TYPE_TEXT[] = {"SW", "PW", "IO", "PS", "OT", "OH", "SV", "SR", "MC", "SP", "DS", "IE", "BG", "SN", "", "EX", "", "", "", "PC"};

    /**
     * Get system and status information with CNC API.
     */
    public short getStatus(ODBST2 statinfo, ODBERR err) {
        mConnect.ret = mConnect.connectNc();
        if (mConnect.ret != EW_OK) {
            return mConnect.ret;
        }
        // Get status information from CNC by using CNC API.
        mConnect.ret = Fwlibe1.cnc_statinfo2(mConnect.getHndl(), statinfo);
        Log.d("cnc_statinfo2", "ret:" + mConnect.ret + " statinfo:" + statinfo);
        if (mConnect.ret != EW_OK) {
            Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
            Log.e(NCUtility.getLogTagWithMethod(), "cnc_statinfo2 ret:" + mConnect.ret + " err:" + err);
        }
        return mConnect.ret;
    }

    /**
     * Get alarm message and operator message.
     */
    public short getAlarmMessageAndOperatorMessage(ODBERR err, String[] almmsg, String[] opemsg) {
        mConnect.ret = mConnect.connectNc();
        if (mConnect.ret != EW_OK) {
            return mConnect.ret;
        }
        // Get language
        short num = 43;
        short axis = 0;
        short length = 4 + 1;
        ODBDGN diag = new ODBDGN();
        // Get diagnosis data from CNC by using CNC API.
        mConnect.ret = Fwlibe1.cnc_diagnoss(mConnect.getHndl(), num, axis, length, diag);
        if (mConnect.ret != EW_OK) {
            Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
            Log.e(NCUtility.getLogTagWithMethod(), "cnc_diagnoss ret:" + mConnect.ret + " err:" + err);
            return mConnect.ret;
        }

        Short path_no = new Short((short) 0);
        Short maxpath_no = new Short((short) 0);

        short type = -1;
        String language = getLanguage(diag.getIdata());
        // Get path information from CNC by using CNC API.
        mConnect.ret = Fwlibe1.cnc_getpath(mConnect.getHndl(), path_no, maxpath_no);
        if (mConnect.ret != EW_OK) {
            Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
            Log.e(NCUtility.getLogTagWithMethod(), "cnc_getpath ret:" + mConnect.ret + " err:" + err);
            return mConnect.ret;
        }
        StringBuffer almmsgbuf = new StringBuffer();
        for (short i = 1; i <= maxpath_no.shortValue(); i++) {
            if (i != 1 || path_no.shortValue() != 1) {
                mConnect.ret = Fwlibe1.cnc_setpath(mConnect.getHndl(), i);
                if (mConnect.ret != EW_OK) {
                    Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
                    Log.e(NCUtility.getLogTagWithMethod(), "cnc_getpath ret:" + mConnect.ret + " err:" + err);
                    break;
                }
            }

            Short almnum = new Short((short) 30);
            ODBALMMSG2 odbalmmsg2[] = new ODBALMMSG2[almnum];
            // Get alarm message from CNC by using CNC API.
            mConnect.ret = Fwlibe1.cnc_rdalmmsg2(mConnect.getHndl(), type, almnum, odbalmmsg2);
            if (mConnect.ret != EW_OK) {
                Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
                Log.e(NCUtility.getLogTagWithMethod(), "cnc_rdalmmsg2 ret:" + mConnect.ret + " err:" + err);
                break;
            }
            for (short msgno = 0; msgno < almnum.shortValue(); msgno++) {
                almmsgbuf.append(String.format("PATH%02d\n", i));
                try {
                    byte msg[] = Arrays.copyOfRange(odbalmmsg2[msgno].getAlm_msg(), 0, odbalmmsg2[msgno].getMsg_len());
                    almmsgbuf.append(String.format("%s%4d %s\n", ALM_TYPE_TEXT[odbalmmsg2[msgno].getType()],
                            odbalmmsg2[msgno].getAlm_no(),
                            new String(msg, language)));
                } catch (UnsupportedEncodingException e) {
                    e.printStackTrace();
                }
            }
        }
        almmsg[0] = almmsgbuf.toString();

        mConnect.ret = Fwlibe1.cnc_setpath(mConnect.getHndl(), path_no);
        StringBuffer opemsgbuf = new StringBuffer();
        Short openum = new Short((short) 5);
        OPMSG3 opmsg[] = new OPMSG3[openum];

        // Get operator message from CNC by using CNC API.
        mConnect.ret = Fwlibe1.cnc_rdopmsg3(mConnect.getHndl(), type, openum, opmsg);
        if (mConnect.ret != EW_OK) {
            Fwlibe1.cnc_getdtailerr(mConnect.getHndl(), err);
            Log.e(NCUtility.getLogTagWithMethod(), "cnc_rdopmsg3 ret:" + mConnect.ret + " err:" + err);
            return mConnect.ret;
        }
        for (short msgno = 0; msgno < openum.shortValue(); msgno++) {
            if (opmsg[msgno].getDatano() > -1) {
                opemsgbuf.append(String.format("%s%04d\n", "No.", opmsg[msgno].getDatano()));
                try {
                    byte msg[] = Arrays.copyOfRange(opmsg[msgno].getData(), 0, opmsg[msgno].getChar_num());
                    opemsgbuf.append(String.format("%s\n\n", new String(msg, language)));
                } catch (UnsupportedEncodingException e) {
                    e.printStackTrace();
                }
            }
        }
        opemsg[0] = opemsgbuf.toString();
        return mConnect.ret;
    }

    /**
     * Return language code.
     */
    public String getLanguage(short ret) {
        switch (ret) {
            case 0:
                // ASCII(English)
                return "ASCII";
            case 1:
            case 4:
                // ShiftJIS(Japanese, Chinese)
                return "Shift-JIS";
            case 15:
                // GB2312(Chinese（Simplified characters））
                return "GB2312";
            case 6:
                //Code page 949:Korean
                return "x-windows-949";
            case 16:
                // Code page 1251：Russian
                return "windows-1251";
            case 17:
                // Code page 1254：Turkish
                return "windows-1254";
            default:
                // European character code
                return "ISO-8859-2";
        }
    }
}