package jp.co.fanuc.focas2sample;

import android.os.Bundle;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;

/**
 * The super class of all activities.
 */
public class BaseActivity extends AppCompatActivity {

    public enum NCFUNC_ENUM {
        NCConnection,
        GetNCState,
        GetNCProgram,
        SetNCProgram
    }

    protected NCFUNC_ENUM selectedFunc;

    /**
     * Initialize activity
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);
        selectedFunc = (NCFUNC_ENUM) getIntent().getSerializableExtra("func");
    }

    /**
     * Select option item.
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        switch (item.getItemId()) {
            case android.R.id.home:
                finish();
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

    /**
     * Create option menu.
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_actionbar, menu);
        menu.findItem(R.id.menu_add).setVisible(false);
        menu.findItem(R.id.menu_edit).setVisible(false);
        menu.findItem(R.id.menu_save).setVisible(false);
        menu.findItem(R.id.menu_mkdir).setVisible(false);
        menu.findItem(R.id.menu_reg).setVisible(false);
        menu.findItem(R.id.menu_qrcode_create).setVisible(false);
        menu.findItem(R.id.menu_qrcode_read).setVisible(false);
        menu.findItem(R.id.menu_save_file).setVisible(false);
        return true;
    }
}
