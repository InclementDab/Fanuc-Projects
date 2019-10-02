package jp.co.fanuc.focas2sample.manager;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import jp.co.fanuc.focas2sample.model.NCDirInfo;

/**
 * NC guide program,such creating folder management.
 */
public class NCDirectoryManager {
    public String deviceName;
    public String dir;
    private Map<String, List<NCDirInfo>> ncDirInfoDic;
    private List<String> pathComponet;

    private static NCDirectoryManager _sharedInstance = null;

    /**
     * Create object as singleton object.
     */
    public static NCDirectoryManager sharedManager() {
        if (_sharedInstance == null) {
            _sharedInstance = new NCDirectoryManager();
        }
        return _sharedInstance;
    }

    /**
     * Constractor for creating root directory.
     */
    private NCDirectoryManager() {
        refresh();
    }

    /**
     * Initializing value.
     */
    public void refresh() {
        deviceName = null;
        dir = null;
        ncDirInfoDic = new HashMap<String, List<NCDirInfo>>();
        pathComponet = new ArrayList<String>();
    }

    /**
     * Set CNC directory
     */
    public void setCNCDir(String cdir) {
        // Add slash to directory path, if nothing slash.
        if (!cdir.endsWith("/")) {
            cdir = cdir + "/";
        }
        dir = cdir;
        pathComponet = new ArrayList<String>(Arrays.asList(cdir.split("/")));
        if (pathComponet.get(0).isEmpty()) {
            pathComponet.set(0, "/");
        }
        if (pathComponet.get(1).isEmpty()) {
            pathComponet.remove(1);
        }
        if (pathComponet.size() > 2) {
            deviceName = pathComponet.get(1);
        }
    }

    /**
     * Current is top judgement
     */
    public boolean isCurrentIsTop() {
        if (2 == pathComponet.size()) {
            return true;
        }
        return false;
    }

    public void addNCDirInfoArray(List<NCDirInfo> array) {
        ncDirInfoDic.put(dir, array);
    }

    public List<NCDirInfo> getNCDirInfoArray() {
        return ncDirInfoDic.get(dir) == null ? new ArrayList<NCDirInfo>() : ncDirInfoDic.get(dir);
    }

    /**
     * Get upper directory name.
     */
    public String getUpperDirName() {
        StringBuffer dirName = new StringBuffer("/");
        int count = pathComponet.size();
        for (int i = 0; i < count - 1; i++) {
            dirName.append(pathComponet.get(i));
            if (!pathComponet.get(i).equals("/")) {
                // Add slash to directory path, if nothing slash.
                dirName.append("/");
            }
        }
        return dirName.toString();
    }

    /**
     * Get directory name.
     */
    public String getDirName(String childDirName) {
        String dirName = dir + childDirName;
        if (!dirName.endsWith("/")) {
            // Add slash to directory path, if nothing slash.
            dirName += "/";
        }
        return dirName;
    }

    /**
     * Get dispose directory.
     */
    public String getDispDir() {
        String replaceString = dir.replace("/" + deviceName, "");
        return replaceString;
    }

}
