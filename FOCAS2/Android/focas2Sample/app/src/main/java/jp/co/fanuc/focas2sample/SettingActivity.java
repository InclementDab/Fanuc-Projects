package jp.co.fanuc.focas2sample;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.EditText;

import java.text.DecimalFormat;

/**
 * Activity for setting connect interval.
 */
public class SettingActivity extends BaseActivity {

    private EditText timeInterval;
    
    /**
     * Initialize activity
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_setting);
        setTitle(R.string.setting);

        timeInterval = (EditText) findViewById(R.id.time_interval);
        SharedPreferences preferences = getSharedPreferences("ncInfoData", Context.MODE_PRIVATE);
        double timerInterval = preferences.getFloat("timerInterval", (float) 10.0);
        DecimalFormat df = new DecimalFormat("##0.0");
        timeInterval.setText(df.format(timerInterval));
    }

    /**
     * Create options menu
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        boolean ret = super.onCreateOptionsMenu(menu);
        menu.findItem(R.id.menu_save).setVisible(true);
        return ret;
    }

    /**
     * Select options item
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        switch (item.getItemId()) {
            case R.id.menu_save:
                double interval = 0.0;
                if (timeInterval.getText().toString() == null ||  timeInterval.getText().toString().isEmpty()) {
                    timeInterval.setError(getString(R.string.error_need_input));
                    return true;
                }
                try {
                    interval = Double.parseDouble(timeInterval.getText().toString());
                }catch (NumberFormatException e){
                    timeInterval.setError(getString(R.string.error_need_number));
                    return true;
                }
                SharedPreferences preferences = getSharedPreferences("ncInfoData", Context.MODE_PRIVATE);
                SharedPreferences.Editor editor = preferences.edit();
                editor.putFloat("timerInterval", (float) interval);
                editor.commit();
                finish();
                return true;

        }
        return super.onOptionsItemSelected(item);
    }
}
