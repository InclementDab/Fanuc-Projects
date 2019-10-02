package jp.co.fanuc.focas2sample;

import android.app.ProgressDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AlertDialog;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

import jp.co.fanuc.focas2sample.adapter.ProgramAdapter;
import jp.co.fanuc.focas2sample.manager.FileSaveManager;
import jp.co.fanuc.focas2sample.manager.NCConnectManager;
import jp.co.fanuc.focas2sample.manager.NCDirectoryManager;
import jp.co.fanuc.focas2sample.model.NCConnectionSetting;
import jp.co.fanuc.focas2sample.model.NCDirInfo;
import jp.co.fanuc.focas2sample.model.NCGetProgram;

import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.GetNCProgram;
import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.SetNCProgram;
import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_NUMBER;
import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_OK;

/**
 * Promgram list page
 */
public class NCProgramActivity extends BaseActivity {
    TextView mNCMessage;
    ListView mListView;
    ProgramAdapter fileAdapter;
    int mConnectid;
    private ProgressDialog progressDialog = null;
    Handler mHandler;

    /**
     * Initialize activity
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_nc_program);
        mHandler = new Handler();
        mNCMessage = (TextView) findViewById(R.id.nc_message);
        mConnectid = getIntent().getIntExtra("connectid", -1);
        if (mConnectid == -1) {
            Log.e(getClass().getSimpleName(), getString(R.string.error_dlag_message));
            finish();
            return;
        }
        NCConnectionSetting connect = NCConnectManager.sharedManager().getNcDataList().get(mConnectid);
        setTitle(connect.getNcName());
        mNCMessage.setText("");
        mListView = (ListView) findViewById(R.id.ncfile_view);
        fileAdapter = new ProgramAdapter(this);
        mListView.setAdapter(fileAdapter);
        moveCNCDir("");

        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                NCDirectoryManager dicManager = NCDirectoryManager.sharedManager();
                NCDirInfo dirinfo = dicManager.getNCDirInfoArray().get(position);
                /**
                 * Return to upper folder
                 */
                if (dirinfo.getData_kind() == 0) {
                    String dir_name;
                    if (dirinfo.getD_f().equals(getString(R.string.upper_folder))) {
                        dir_name = dicManager.getUpperDirName();
                    } else {
                        dir_name = dicManager.getDirName(dirinfo.getD_f());
                    }
                    moveCNCDir(dir_name);
                } else {
                    if (selectedFunc == GetNCProgram) {
                        Intent intent = new Intent(NCProgramActivity.this, NCProgramDetailActivity.class);
                        intent.putExtra("func", selectedFunc);
                        intent.putExtra("ProgPath", dicManager.dir + dirinfo.getD_f());
                        intent.putExtra("connectid", mConnectid);
                        startActivity(intent);
                    }
                }
            }
        });
    }

    /**
     * Move CNC directory
     */
    private void moveCNCDir(final String dirname) {
        progressDialog = ProgressDialog.show(NCProgramActivity.this, "", getString(R.string.now_collectiong));
        final NCConnectionSetting connect = NCConnectManager.sharedManager().getNcDataList().get(mConnectid);
        connect.runOnThread(new Runnable() {
            @Override
            public void run() {

                short ret = connect.connectNc();
                if (ret != EW_OK) {
                    doMoveCNCDirResult(getString(R.string.connect_failed, ret, 0, 0));
                    return;
                }
                NCGetProgram dirGetter = new NCGetProgram(connect);

                if (!dirname.isEmpty()) {
                    dirGetter.moveCNCdir(dirname);
                }
                String dir_name[] = new String[1];
                dirGetter.getCNCdir(dir_name);


                if (dir_name[0] != null && dir_name[0].equals("'")) {
                    dir_name[0] = "//CNC_MEM/";
                }

                NCDirectoryManager dicManager = NCDirectoryManager.sharedManager();
                dicManager.setCNCDir(dir_name[0]);
                List<NCDirInfo> dicArray = new ArrayList<NCDirInfo>();
                ret = dirGetter.getAllDir(NCProgramActivity.this, dir_name[0], dicManager.isCurrentIsTop(), selectedFunc != GetNCProgram, dicArray);
                if (ret != EW_OK && ret != EW_NUMBER) {
                    doMoveCNCDirResult(getString(R.string.collect_failed, ret, dirGetter.err.getErr_no(), dirGetter.err.getErr_dtno()));
                    return;
                }
                dicManager.addNCDirInfoArray(dicArray);
                doMoveCNCDirResult(dicManager);
            }
        });
    }

    /**
     * Move current directory result.
     */
    protected void doMoveCNCDirResult(final Object result) {
        mHandler.post(new Runnable() {
            @Override
            public void run() {
                progressDialog.dismiss();
                progressDialog = null;
                if (result != null && result instanceof String) {
                    mNCMessage.setText((CharSequence) result);
                    NCDirectoryManager.sharedManager().refresh();
                } else if (result != null && result instanceof NCDirectoryManager) {
                    NCDirectoryManager dicManager = (NCDirectoryManager) result;
                    mNCMessage.setText(getString(R.string.device_name_current_dir, dicManager.deviceName, dicManager.getDispDir()));
                }
                fileAdapter.notifyDataSetChanged();
            }
        });
    }

    /**
     * Download NC program.
     */
    private void downloadNCProgram() {
        progressDialog = ProgressDialog.show(NCProgramActivity.this, "", getString(R.string.now_collectiong));
        final NCConnectionSetting connect = NCConnectManager.sharedManager().getNcDataList().get(mConnectid);
        connect.runOnThread(new Runnable() {
            @Override
            public void run() {
                short ret = connect.connectNc();
                if (ret != EW_OK) {
                    doMoveCNCDirResult(getString(R.string.regist_error, ret, 0, 0));
                    return;
                }
                NCGetProgram dirGetter = new NCGetProgram(connect);
                FileSaveManager fileManager = FileSaveManager.sharedManager();
                NCDirectoryManager dicManager = NCDirectoryManager.sharedManager();
                ret = dirGetter.downloadNCProgram(dicManager.dir, fileManager.progName, fileManager.getProgDataFromFile());
                if (ret != EW_OK) {
                    doDownloadNCProgramResult(getString(R.string.regist_error, ret, dirGetter.err.getErr_no(), dirGetter.err.getErr_dtno()));
                    return;
                }

                doDownloadNCProgramResult(null);
            }
        });
    }

    /**
     * Download NC program result.
     */
    protected void doDownloadNCProgramResult(final Object result) {
        mHandler.post(new Runnable() {
            @Override
            public void run() {
                progressDialog.dismiss();
                progressDialog = null;
                if (result != null && result instanceof String) {
                    AlertDialog.Builder builder = new AlertDialog.Builder(NCProgramActivity.this);
                    builder.setMessage((CharSequence) result);
                    builder.setTitle(R.string.regist_failed);
                    builder.setPositiveButton(R.string.ok, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            Intent intent = new Intent();
                            setResult(RESULT_OK, intent);
                            finish();
                        }
                    });
                    builder.create().show();
                } else {
                    AlertDialog.Builder builder = new AlertDialog.Builder(NCProgramActivity.this);
                    builder.setMessage(R.string.regist_complete);
                    builder.setTitle(FileSaveManager.sharedManager().getCurDir() + FileSaveManager.sharedManager().progName);
                    builder.setPositiveButton(R.string.ok, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            Intent intent = new Intent();
                            setResult(RESULT_OK, intent);
                            finish();
                        }
                    });
                    builder.create().show();
                }
            }
        });
    }

    /**
     * Destroy activity
     */
    @Override
    protected void onDestroy() {
        super.onDestroy();
        NCDirectoryManager.sharedManager().refresh();
    }

    /**
     * Create option menu
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        boolean ret = super.onCreateOptionsMenu(menu);
        if (selectedFunc == SetNCProgram) {
            menu.findItem(R.id.menu_reg).setVisible(true);
        }
        return ret;
    }

    /**
     * Select option item.
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        switch (item.getItemId()) {
            case R.id.menu_reg:
                downloadNCProgram();
                return true;
            case android.R.id.home:
                Intent intent = new Intent();
                setResult(RESULT_OK, intent);
                finish();
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

}
