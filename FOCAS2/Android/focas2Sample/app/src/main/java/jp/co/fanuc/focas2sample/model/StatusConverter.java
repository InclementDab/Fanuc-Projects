package jp.co.fanuc.focas2sample.model;

import android.content.Context;

import java.io.Serializable;

import jp.co.fanuc.fwlibe1.ODBST2;

/**
 * Class for converting state values to wording
 */
public class StatusConverter implements Serializable {

    private static final long serialVersionUID = 1L;

    public String aut_status;
    public String emergency_status;
    public String alarm_status;

    final String AUT_STATUS[] = {"MDI", "MEMory", "****", "EDIT", "HaNDle", "JOG", "Teach in JOG", "Teach in HaNDle", "INC･feed", "REFerence", "ReMoTe"};
    final String EMERGENCY_STATUS[] = {"", "EMerGency", "ReSET", "WAIT"};
    final String ALARM_STATUS[] = {"***", "ALarM", "BATtery low", "FAN", "PS Warning", "FSsB warning", "LeaKaGe warning", "ENCoder warning", "PMC alarm"};

    /**
     * Status converter
     */
    public StatusConverter(Context context, ODBST2 statinfo) {
        aut_status = AUT_STATUS[statinfo.getAut()];
        emergency_status = EMERGENCY_STATUS[statinfo.getEmergency()];
        alarm_status = ALARM_STATUS[statinfo.getAlarm()];
    }

    /**
     * Erease lowercase
     */
    public static String ereaseLowercase(String str) {
        String ret = "";
        for (int index = 0; index < str.length(); index++) {
            if (Character.isUpperCase(str.charAt(index))
                    || str.charAt(index) == '*') {
                ret += str.charAt(index);
            }
        }
        return ret;
    }
}
