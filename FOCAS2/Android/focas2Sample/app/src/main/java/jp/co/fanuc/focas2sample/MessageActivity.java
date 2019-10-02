package jp.co.fanuc.focas2sample;

import android.app.ProgressDialog;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.widget.TextView;

import jp.co.fanuc.focas2sample.manager.NCConnectManager;
import jp.co.fanuc.focas2sample.model.NCConnectionSetting;
import jp.co.fanuc.focas2sample.model.NCGetStatus;
import jp.co.fanuc.fwlibe1.ODBERR;

import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_OK;
/**
 * Activity class for showing alarm message and operator message.
 */
public class MessageActivity extends BaseActivity {

    private TextView almmsgText;
    private TextView opemsgText;
    private ProgressDialog progressDialog = null;
    Handler mHandler;

    /**
     * Initialize activity
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_message);
        mHandler = new Handler();

        int connectid = getIntent().getIntExtra("connectid", -1);
        if (connectid == -1) {
            Log.e(getClass().getSimpleName(), getString(R.string.error_dlag_message));
            finish();
            return;
        }
        // If found connectID, get connection setting.
        final NCConnectionSetting connect = NCConnectManager.sharedManager().getNcDataList().get(connectid);

        setTitle(connect.getNcName());

        almmsgText = (TextView) findViewById(R.id.almmsgText);
        opemsgText = (TextView) findViewById(R.id.opemsgText);
        almmsgText.setTextColor(Color.RED);

        progressDialog = ProgressDialog.show(MessageActivity.this, "", getString(R.string.now_collectiong));
        // Get alarm and operator message in thread.
        connect.runOnThread(new Runnable() {
            @Override
            public void run() {
                NCGetStatus status = new NCGetStatus(connect);
                String almmsg[] = new String[1];
                String opemsg[] = new String[1];
                ODBERR err = new ODBERR();
                short ret = status.getAlarmMessageAndOperatorMessage(err, almmsg, opemsg);
                if (ret != EW_OK) {
                    doResult(getString(R.string.collect_failed, ret, err.getErr_no(), err.getErr_dtno()));
                } else {
                    String result[] = new String[2];
                    result[0] = almmsg[0];
                    result[1] = opemsg[0];
                    doResult(result);
                }
            }
        });
    }

    /**
     * Showing message has been got.
     */
    protected void doResult(final Object result) {
        mHandler.post(new Runnable() {
            @Override
            public void run() {
                progressDialog.dismiss();
                progressDialog = null;
                if (result instanceof String) {
                    almmsgText.setText((String) result);
                    opemsgText.setText("");
                } else if (result instanceof String[]) {
                    //value
                    almmsgText.setText(((String[])result)[0]);
                    opemsgText.setText(((String[])result)[1]);
                }
            }
        });
    }

}
