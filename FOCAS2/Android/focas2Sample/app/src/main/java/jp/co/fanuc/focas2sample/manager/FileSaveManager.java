package jp.co.fanuc.focas2sample.manager;

import android.content.Context;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;

import jp.co.fanuc.focas2sample.R;

/**
 * Management of read and write internal memory contents in device.
 */
public class FileSaveManager {

    private String currentDir;
    private ArrayList<String> contents = new ArrayList<String>();

    public String progName;
    public String progData;
    private String getDocumentsPath;
    private String upperFolderName;
    private static FileSaveManager _sharedInstance = null;

    /**
     * Create object as singleton object.
     */
    public static FileSaveManager sharedManager(Context context) {
        if (_sharedInstance == null) {
            _sharedInstance = new FileSaveManager(context);
        }
        return _sharedInstance;
    }

    public static FileSaveManager sharedManager() {
        return _sharedInstance;
    }

    /**
     * Constractor for creating root directory.
     */
    private FileSaveManager(Context context) {
        getDocumentsPath = context.getExternalFilesDir("").getAbsolutePath();
        upperFolderName = context.getString(R.string.upper_folder);
        File root = new File(getDocumentsPath);
        if (!root.exists()) {
            root.mkdir();
        }
        contents = new ArrayList<String>();
        setDefaultPath();
    }

    /**
    * Set default path.
    */
    public void setDefaultPath() {
        currentDir = getDocumentsPath;
        progName = null;
        progData = null;
    }

    /**
    * Display path.
    */
    public String getCurDir() {
        String replacedStr = currentDir.replace(getDocumentsPath, " ");
        if (replacedStr.startsWith(" /")) {
            replacedStr = replacedStr.substring(2);
        }
        return replacedStr;
    }

    /**
    * Get directory files list.
    */
    public ArrayList<String> getContents() {
        return getContentsOfDirectoryAtPath(currentDir);
    }

    /**
    * Get directory files list.
    */
    public ArrayList<String> getContentsOfDirectoryAtPath(String filePath) {
        ArrayList<String> items = new ArrayList<String>();
        File file = new File(filePath);
        File[] files = file.listFiles();
        if (!filePath.equals(getDocumentsPath)) {
            items.add(upperFolderName);
        }

        // Creating file list except for CSV file.
        if (files != null) {
            for (File fileTemp : files) {
                if (!fileTemp.getName().toLowerCase().endsWith(".csv")) {
                    items.add(fileTemp.getName());
                }
            }
        }
        return items;
    }

    /**
    * Get files count.
    */
    public int countOfContents() {
        contents = getContentsOfDirectoryAtPath(currentDir);
        return contents.size();
    }

    /**
    * Get files name.
    */
    public String getContentsName(int row) {
        return contents.get(row);
    }

    /**
     * Check directory or file.
     */
    public boolean isDir(int row) {
        boolean isDir = false;
        if (getContentsName(row).equals(upperFolderName)) {
            isDir = true;
        } else {
            File item = new File(currentDir, getContentsName(row));
            isDir = item.isDirectory();
        }
        return isDir;
    }

    /**
    * Set program name and data.
    */
    public void setPrognameAndProgData(String pgName, String pgData) {
        progName = pgName;
        /**
        * If double-byte characters are found in progData, remove them.
        */
        progData = deleteFullWidth(pgData);
    }

    /**
    * Remove double-byte characters.
    */
    public String deleteFullWidth(String str) {
        StringBuffer string = new StringBuffer();
        for (int i = 0; i < str.length(); i++) {
            String encodedChar = str.substring(i, i + 1);
            try {
                if (encodedChar.getBytes("UTF-8").length == encodedChar.length()) {
                    string.append(encodedChar);
                }
            } catch (UnsupportedEncodingException e) {
                e.printStackTrace();
            }
        }
        return string.toString();
    }

    /**
    * Save file.
    */
    public void saveFile() {
        String path = currentDir + "/" + progName;
        BufferedWriter out = null;
        try {
            out = new BufferedWriter(new OutputStreamWriter(
                    new FileOutputStream(path), "UTF-8"));
            out.write(progData);
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            if (out != null) {
                try {
                    out.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }

    }

    /**
    * Save change file.
    */
    public void saveChangeFile(String text) {
        String path = currentDir + "/" + progName;
        BufferedWriter out = null;
        try {
            out = new BufferedWriter(new OutputStreamWriter(
                    new FileOutputStream(path), "UTF-8"));
            out.write(text);
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            if (out != null) {
                try {
                    out.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }
    }

    /**
    * Make directory.
    */
    public void makeDir(String folderName) {
        String path = currentDir + "/" + folderName;

        File file = new File(path);
        file.mkdir();
    }


    /**
    * Move folder.
    */
    public void moveDir(int row) {
        String name = getContentsName(row);
        if (name.compareTo(upperFolderName) == 0) {
            if (currentDir.endsWith("/")) {
                currentDir = currentDir.substring(0, currentDir.length() - 1);
            }
            currentDir = currentDir.substring(0, currentDir.lastIndexOf("/"));
        } else {
            currentDir = currentDir + "/" + getContentsName(row);
        }

    }

    /**
    * Determination of deletion availability.
    */
    public boolean canDelete(int row) {
        String name = getContentsName(row);
        if (name.compareTo(upperFolderName) == 0) {
            return false;
        }
        return true;
    }

    /**
    * Delete file.
    */
    public void deleteFile(int row) {
        File file = new File(currentDir, getContentsName(row));
        if (file.isDirectory()) {
            deleteFolder(file);
        } else {
            file.delete();
        }
    }

    /**
     * Delete folder.
     */
    public void deleteFolder(File file) {

        File[] files = file.listFiles();
        if (files != null) {
            for (File subFile : files) {
                if (subFile.isDirectory()) {
                    deleteFolder(subFile);
                } else {
                    subFile.delete();
                }
            }
        }
    }

    public void setSelectedFileName(int row) {
        progName = getContentsName(row);
    }

    /**
     * Get program name
     */
    public String getProgName() {
        String str = getCurDir() + "/" + progName;
        str = str.trim();
        if (str.startsWith("/")) {
            str = str.substring(1);
        }
        return str;
    }

    public String getProgData() {
        return progData;
    }


    /**
     * Read program data from file.
     */
    public String getProgDataFromFile() {
        File file = new File(currentDir, progName);
        BufferedReader reader = null;
        StringBuffer buf = new StringBuffer();
        try {
            reader = new BufferedReader(new InputStreamReader(new FileInputStream(file), "UTF8"));

            String lineString = null;
            while ((lineString = reader.readLine()) != null) {
                buf.append(lineString);
                buf.append("\n");
            }
            if (buf.length() >= 1) {
                buf.deleteCharAt(buf.length() - 1);
            }
            reader.close();
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            if (reader != null) {
                try {
                    reader.close();
                } catch (IOException e1) {
                    e1.printStackTrace();
                }
            }
        }
        progData = buf.toString();
        return progData;
    }

}
