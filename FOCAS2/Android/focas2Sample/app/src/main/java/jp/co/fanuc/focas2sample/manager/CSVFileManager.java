package jp.co.fanuc.focas2sample.manager;

import android.content.Context;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.List;

import jp.co.fanuc.focas2sample.model.NCConnectionSetting;

/**
 * Management of read and write CSVFile.
 */
public class CSVFileManager {
    public final String CSV_HEADER = "NCName,IPAddress,Port,Timeout";
    public final String COMMA = ",";
    public final String CSV_FILENAME = "NCConnectData.csv";
    private String mFileName = null;
    private static CSVFileManager _sharedInstance = null;

    public static CSVFileManager sharedManager(Context context) {
        if (_sharedInstance == null) {
            _sharedInstance = new CSVFileManager(context);
        }
        return _sharedInstance;
    }

    public static CSVFileManager sharedManager() {
        return _sharedInstance;
    }

    /*
    * Get path and make file
    * */
    private CSVFileManager(Context context) {
        mFileName = context.getExternalFilesDir("").getAbsolutePath() + "/" + CSV_FILENAME;
    }

    /*
    * Write connection informatin to CSV file.
    * */
    public void writeCsv(List<NCConnectionSetting> datas) {
        File csvFile = new File(mFileName);
        if (csvFile.exists()) {
            csvFile.delete();
        }
        StringBuffer sb = new StringBuffer();
        sb.append(CSV_HEADER);
        sb.append("\n");
        for (NCConnectionSetting setting : datas) {
            sb.append(setting.getNcName());
            sb.append(COMMA);
            sb.append(setting.getIpAddr());
            sb.append(COMMA);
            sb.append(setting.getPortNo());
            sb.append(COMMA);
            sb.append(setting.getTimeoutVal());
            sb.append("\n");
        }
        FileWriter writer = null;
        BufferedWriter bw = null;
        try {
            writer = new FileWriter(csvFile);
            bw = new BufferedWriter(writer);
            bw.write(sb.toString());
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            try {
                bw.close();
                writer.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

    /*
     * Read connection information from CSV file.
     * */
    public void readCsv(List<NCConnectionSetting> datas) {
        File csvFile = new File(mFileName);
        if (!csvFile.exists()) {
            return;
        }
        datas.clear();
        FileReader reader = null;
        BufferedReader br = null;
        try {
            reader = new FileReader(csvFile);
            br = new BufferedReader(reader);
            String str = null;
            int index = 0;
            while ((str = br.readLine()) != null) {
                if (str.equals(CSV_HEADER)) {
                    continue;
                }
                String[] strArray = null;
                strArray = str.split(COMMA);
                try {
                    NCConnectionSetting ncDataInfo = new NCConnectionSetting(strArray[0], strArray[1], Short.valueOf(strArray[2]), Integer.valueOf(strArray[3]));
                    ncDataInfo.active();
                    datas.add(ncDataInfo);
                    if (index >= 10) {
                        break;
                    }
                    index++;
                } catch (Exception e) {
                    e.printStackTrace();
                    continue;
                }
            }

        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            try {
                br.close();
                reader.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
}
