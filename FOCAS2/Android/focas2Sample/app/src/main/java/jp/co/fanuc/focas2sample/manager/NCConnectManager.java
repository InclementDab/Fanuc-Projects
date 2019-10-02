package jp.co.fanuc.focas2sample.manager;

import java.util.ArrayList;
import java.util.List;

import jp.co.fanuc.focas2sample.model.NCConnectionSetting;

/**
 * Connection destination management.
 */
public class NCConnectManager {
    public List<NCConnectionSetting> getNcDataList() {
        return ncDataList;
    }

    private List<NCConnectionSetting> ncDataList;

    private static NCConnectManager _sharedInstance = null;

    /**
     * Create object as singleton object.
     */
    public static NCConnectManager sharedManager() {
        if (_sharedInstance == null) {
            _sharedInstance = new NCConnectManager();
        }
        return _sharedInstance;
    }

    /**
     * Constractor for connecting to NC.
     */
    private NCConnectManager() {
        ncDataList = new ArrayList<NCConnectionSetting>();
        loadNCConnect();
    }

    /**
     * Load NC connect from CSV file.
     */
    private void loadNCConnect() {
        CSVFileManager.sharedManager().readCsv(ncDataList);
    }

    /**
     * Save NC connect to CSV file.
     */
    private void saveNCConnect() {
        CSVFileManager.sharedManager().writeCsv(ncDataList);
    }

    /**
     * Existing check NC name;
     */
    public boolean hasName(String name) {
        for (NCConnectionSetting connect : ncDataList) {
            if (name.equals(connect.getNcName())) {
                return true;
            }
        }
        return false;
    }

    /**
     * Add connection information to NC list.
     */
    public void addConnect(NCConnectionSetting connect) {
        connect.active();
        ncDataList.add(connect);
        saveNCConnect();
    }

    /**
     * Remove connection information from NC list.
     */
    public void removeConnect(int pos) {
        NCConnectionSetting connect = ncDataList.get(pos);
        connect.inactive();
        ncDataList.remove(connect);
        saveNCConnect();
    }

    /**
     * Replace connection information to NC list.
     */
    public void replaceConnect(int pos, NCConnectionSetting connect) {
        if (pos != -1) {
            NCConnectionSetting cnet = ncDataList.get(pos);
            cnet.inactive();
            ncDataList.remove(cnet);
            ncDataList.add(pos, connect);
        } else {
            ncDataList.add(connect);
        }
        connect.active();
        saveNCConnect();
    }

    /**
     * Remove all connection information from NC list.
     */
    public void removeAllConnect() {
        for (NCConnectionSetting connect : ncDataList) {
            connect.inactive();
        }
        ncDataList.clear();
    }
}
