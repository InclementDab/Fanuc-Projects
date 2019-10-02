package jp.co.fanuc.focas2sample;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;

import jp.co.fanuc.focas2sample.manager.CSVFileManager;
import jp.co.fanuc.focas2sample.manager.NCConnectManager;

import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.GetNCProgram;
import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.GetNCState;
import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.NCConnection;
import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.SetNCProgram;

/**
 * Menu page.
 */
public class MainActivity extends AppCompatActivity {
    /**
     * Page transition
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        CSVFileManager.sharedManager(this);
        NCConnectManager.sharedManager();
        findViewById(R.id.connectAddress).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, ConnectionListActivity.class);
                intent.putExtra("func", NCConnection);
                startActivity(intent);
            }
        });

        findViewById(R.id.getStatus).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, ConnectionListActivity.class);
                intent.putExtra("func", GetNCState);
                startActivity(intent);
            }
        });

        findViewById(R.id.getProgram).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, ConnectionListActivity.class);
                intent.putExtra("func", GetNCProgram);
                startActivity(intent);
            }
        });

        findViewById(R.id.programManagement).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, NCProgramFileActivity.class);
                intent.putExtra("func", SetNCProgram);
                startActivity(intent);
            }
        });
    }

    /**
     * Destroy activity
     */
    @Override
    protected void onDestroy() {
        super.onDestroy();
    }
}

