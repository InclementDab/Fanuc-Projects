package jp.co.fanuc.focas2sample;

import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AlertDialog;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.ListView;

import java.util.Timer;
import java.util.TimerTask;

import jp.co.fanuc.focas2sample.adapter.GetStatusAdapter;
import jp.co.fanuc.focas2sample.adapter.ListAdapter;
import jp.co.fanuc.focas2sample.manager.NCConnectManager;
import jp.co.fanuc.focas2sample.model.NCConnectionSetting;
import jp.co.fanuc.focas2sample.model.NCGetStatus;
import jp.co.fanuc.focas2sample.model.StatusConverter;
import jp.co.fanuc.fwlibe1.ODBERR;
import jp.co.fanuc.fwlibe1.ODBST2;

import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.GetNCState;
import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.NCConnection;
import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.SetNCProgram;
import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_OK;

/**
 * Connection destination list activity.
 */
public class ConnectionListActivity extends BaseActivity {

    private ListView listView;

    NCConnectionSetting ncDataInfo;
    private Handler mHandler;

    private Timer mTimer = null;
    private boolean isForeground = false;

    /**
     * Initialize activity
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_connection_list);
        mHandler = new Handler();
        listView = (ListView) findViewById(R.id.mListView);
        if (selectedFunc == GetNCState) {
            listView.setAdapter(new GetStatusAdapter(this, NCConnectManager.sharedManager().getNcDataList()));
        } else {
            listView.setAdapter(new ListAdapter(this, NCConnectManager.sharedManager().getNcDataList()));
        }
        listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view,
                                    int position, long id) {
                NCConnectionSetting connect = NCConnectManager.sharedManager().getNcDataList().get(position);
                if (selectedFunc == NCConnection) {
                    // Transit to ConnectionInfoActivity.
                    Intent intent = new Intent(ConnectionListActivity.this, ConnectionInfoActivity.class);
                    intent.putExtra("connectid", position);
                    intent.putExtra("func", selectedFunc);
                    startActivityForResult(intent, 1);
                } else if (selectedFunc == GetNCState) {
                    // Transit to MessageActivity.
                    Intent intent = new Intent(ConnectionListActivity.this, MessageActivity.class);
                    intent.putExtra("connectid", position);
                    intent.putExtra("func", selectedFunc);
                    startActivity(intent);
                } else {
                    // Transit to NCProgramActivity.
                    Intent intent = new Intent(ConnectionListActivity.this, NCProgramActivity.class);
                    intent.putExtra("connectid", position);
                    intent.putExtra("func", selectedFunc);
                    if (selectedFunc == SetNCProgram) {
                        startActivityForResult(intent, 2);
                    } else {
                        startActivity(intent);
                    }
                }
            }
        });

        /**
         * Delete data
         */
        if (selectedFunc == NCConnection) {
            listView.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
                                                    @Override
                                                    public boolean onItemLongClick(AdapterView<?> arg0, View view,
                                                                                   final int position, long id) {


                                                        AlertDialog.Builder builder = new AlertDialog.Builder(ConnectionListActivity.this);
                                                        builder.setMessage(R.string.delete_nclist_info);
                                                        builder.setTitle(R.string.delete_data);
                                                        builder.setPositiveButton(R.string.cancel, null);
                                                        builder.setNegativeButton(R.string.delete, new DialogInterface.OnClickListener() {
                                                            @Override
                                                            public void onClick(DialogInterface dialog, int which) {
                                                                NCConnectManager.sharedManager().removeConnect(position);
                                                                invalidateOptionsMenu();
                                                                ((BaseAdapter) listView.getAdapter()).notifyDataSetChanged();
                                                            }
                                                        });
                                                        builder.create().show();
                                                        return true;
                                                    }
                                                }

            );
        }
    }

    /**
     * Create option menu
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        boolean ret = super.onCreateOptionsMenu(menu);
        if (selectedFunc == NCConnection) {
            menu.findItem(R.id.menu_add).setVisible(true);
            if (NCConnectManager.sharedManager().getNcDataList().size() > 9) {
                menu.findItem(R.id.menu_add).setEnabled(false);
            } else {
                menu.findItem(R.id.menu_add).setEnabled(true);
            }
        }
        return ret;
    }

    /**
     * Select option item.
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        switch (item.getItemId()) {
            case R.id.menu_add:
                Intent intent = new Intent(ConnectionListActivity.this, ConnectionInfoActivity.class);
                startActivityForResult(intent, 1);
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

    /**
     * Get Activity Result
     */
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == 1 && data != null) {
            invalidateOptionsMenu();
            ((BaseAdapter) listView.getAdapter()).notifyDataSetChanged();

        } else if (requestCode == 2 && resultCode == RESULT_OK) {
            finish();
        }
    }

    /**
     * Processing when Activity is displayed
     */
    @Override
    protected void onResume() {
        isForeground = true;
        super.onResume();
        if (selectedFunc == GetNCState) {
            startTimer();
        }
    }

    /**
     * Pause Activity
     */
    @Override
    protected void onPause() {
        isForeground = false;
        super.onPause();
        if (selectedFunc == GetNCState) {
            stopTimer();
        }
    }

    public class GetStatus implements Runnable {

        NCConnectionSetting mConnect;

        /**
         * Get status
         */
        public GetStatus(NCConnectionSetting connect) {
            mConnect = connect;
        }

        /**
         * Run get status
         */
        @Override
        public void run() {
            if (mConnect.connecting) {
                return;
            }
            NCGetStatus status = new NCGetStatus(mConnect);
            ODBST2 statinfo = new ODBST2();
            ODBERR err = new ODBERR();
            mConnect.connecting = true;
            mConnect.ret = status.getStatus(statinfo, err);
            mConnect.connecting = false;
            if (mConnect.ret == EW_OK) {
                mConnect.converter = new StatusConverter(ConnectionListActivity.this, statinfo);
            } else {
                mConnect.converter = null;
            }
            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    if (listView != null && isForeground) {
                        ((BaseAdapter) listView.getAdapter()).notifyDataSetChanged();
                    }
                }
            });
        }
    }

    /**
     * Start Timer
     */
    private void startTimer() {
        mTimer = new Timer(true);
        SharedPreferences preferences = getSharedPreferences("ncInfoData", Context.MODE_PRIVATE);
        double timerInterval = preferences.getFloat("timerInterval", (float) 10.0);
        mTimer.schedule(new TimerTask() {
            @Override
            public void run() {
                for (NCConnectionSetting connect : NCConnectManager.sharedManager().getNcDataList()) {
                    if(!connect.connecting) {
                        connect.runOnThread(new GetStatus(connect));
                    }
                }
            }
        }, 0, (long) (timerInterval * 1000));
    }

    /**
     * Stop timer
     */
    private void stopTimer() {
        mTimer.cancel();
        mTimer = null;
    }

}

