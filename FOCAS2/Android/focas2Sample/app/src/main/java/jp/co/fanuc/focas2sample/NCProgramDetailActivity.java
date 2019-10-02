package jp.co.fanuc.focas2sample;

import android.app.ProgressDialog;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.EditText;

import jp.co.fanuc.focas2sample.manager.FileSaveManager;
import jp.co.fanuc.focas2sample.manager.NCConnectManager;
import jp.co.fanuc.focas2sample.model.NCConnectionSetting;
import jp.co.fanuc.focas2sample.model.NCGetProgram;

import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.GetNCProgram;
import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_OK;

/**
 * Program content page.
 */
public class NCProgramDetailActivity extends BaseActivity {
    private int mConnectid;
    private ProgressDialog progressDialog = null;
    private String progPath;
    private String progData;
    EditText mProgContext;
    Handler mHandler;

    /**
     * Initialize activity
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        mHandler = new Handler();
        setContentView(R.layout.activity_ncprogram_detail);
        mProgContext = (EditText) findViewById(R.id.content);
        mProgContext.setFocusable(false);
        if (selectedFunc == GetNCProgram) {
            mConnectid = getIntent().getIntExtra("connectid", -1);
            progPath = getIntent().getStringExtra("ProgPath");
            if (mConnectid == -1 || progPath == null) {
                Log.e(getClass().getSimpleName(), getString(R.string.error_dlag_message));
                finish();
                return;
            }
            setTitle(progPath.substring(progPath.lastIndexOf("/") + 1));
            mProgContext.setText("");
            updateNCProgram();
        } else {
            mProgContext.setText(FileSaveManager.sharedManager(getApplicationContext()).getProgDataFromFile());
            setTitle(FileSaveManager.sharedManager().progName);
            mProgContext.addTextChangedListener(new TextWatcher() {

                @Override
                public void beforeTextChanged(CharSequence s, int start, int count, int after) {

                }

                @Override
                public void onTextChanged(CharSequence s, int start, int before, int count) {
                    invalidateOptionsMenu();
                }

                @Override
                public void afterTextChanged(Editable s) {

                }
            });
        }
    }

    /**
     * Update NC program
     */
    private void updateNCProgram() {
        final NCConnectionSetting connect = NCConnectManager.sharedManager().getNcDataList().get(mConnectid);
        progressDialog = ProgressDialog.show(NCProgramDetailActivity.this, "", getString(R.string.now_collectiong));
        connect.runOnThread(new Runnable() {
            @Override
            public void run() {
                short ret = connect.connectNc();
                if (ret != EW_OK) {
                    doResult(null);
                    return;
                }
                NCGetProgram dirGetter = new NCGetProgram(connect);

                String progData[] = new String[1];
                ret = dirGetter.uploadNCProgram(progPath, progData);
                if (ret != EW_OK) {
                    doResult(null);
                    return;
                }

                doResult(progData[0]);
            }
        });
    }

    /**
     * Result processing
     */
    protected void doResult(final Object result) {
        mHandler.post(new Runnable() {
            @Override
            public void run() {
                progressDialog.dismiss();
                progressDialog = null;
                if (result != null && result instanceof String) {
                    mProgContext.setText((CharSequence) result);
                }
                invalidateOptionsMenu();
            }
        });
    }


    /**
     * get Activity Result
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (resultCode == RESULT_OK) {
            finish();
        }
    }

    /**
     * Create options menu
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        boolean ret = super.onCreateOptionsMenu(menu);
        if (selectedFunc == GetNCProgram) {
            menu.findItem(R.id.menu_save).setVisible(true);
            if (mProgContext.getText().toString().isEmpty()) {
                menu.findItem(R.id.menu_save).setEnabled(false);
            } else {
                menu.findItem(R.id.menu_save).setEnabled(true);
            }
        } else {
            menu.findItem(R.id.menu_reg).setVisible(true);
            if (mProgContext.getText().toString().isEmpty()) {
                menu.findItem(R.id.menu_reg).setEnabled(false);
            } else {
                menu.findItem(R.id.menu_reg).setEnabled(true);
            }
        }
        return ret;
    }

    /**
     * Select options item
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        switch (item.getItemId()) {
            case R.id.menu_save:
                if (selectedFunc == GetNCProgram) {
                    Intent intent = new Intent(NCProgramDetailActivity.this, NCProgramFileActivity.class);
                    FileSaveManager.sharedManager(getApplicationContext()).setDefaultPath();
                    FileSaveManager.sharedManager().setPrognameAndProgData(progPath.substring(progPath.lastIndexOf("/") + 1),
                            mProgContext.getText().toString());
                    intent.putExtra("func", selectedFunc);
                    startActivityForResult(intent, 0);
                    return true;
                } else {
                    FileSaveManager.sharedManager().saveChangeFile(mProgContext.getText().toString());
                    invalidateOptionsMenu();
                    return true;
                }
            case R.id.menu_reg:
                Intent intent = new Intent(NCProgramDetailActivity.this, ConnectionListActivity.class);
                intent.putExtra("func", selectedFunc);
                startActivity(intent);
                return true;

        }
        return super.onOptionsItemSelected(item);
    }

}
