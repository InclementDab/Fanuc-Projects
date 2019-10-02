package jp.co.fanuc.focas2sample;

import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AlertDialog;
import android.text.InputType;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;

import jp.co.fanuc.focas2sample.adapter.FileAdapter;
import jp.co.fanuc.focas2sample.manager.FileSaveManager;

import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.GetNCProgram;
import static jp.co.fanuc.focas2sample.BaseActivity.NCFUNC_ENUM.SetNCProgram;
import static jp.co.fanuc.focas2sample.manager.FileSaveManager.sharedManager;

/**
 * Local file list page.
 */
public class NCProgramFileActivity extends BaseActivity {
    TextView mNCMessage;
    ListView mListView;
    FileAdapter fileAdapter;

    /**
     * Initialize activity
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_ncprogram_file);
        FileSaveManager fileManager = FileSaveManager.sharedManager(getApplicationContext());
        mNCMessage = (TextView) findViewById(R.id.nc_message);
        if (fileManager.progName != null) {
            setTitle(fileManager.progName);
        } else {
            setTitle("");
        }

        mNCMessage.setText(sharedManager().getCurDir());
        mListView = (ListView) findViewById(R.id.ncfile_view);
        fileAdapter = new FileAdapter(this);
        mListView.setAdapter(fileAdapter);
        //Item onclick event.
        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                FileSaveManager fileManager = FileSaveManager.sharedManager();
                if (fileManager.isDir(position)) {
                    fileManager.moveDir(position);
                    mNCMessage.setText(sharedManager().getCurDir());
                    fileAdapter.notifyDataSetChanged();
                } else {
                    if (selectedFunc == SetNCProgram) {
                        fileManager.setSelectedFileName(position);
                        Intent intent = new Intent(NCProgramFileActivity.this, NCProgramDetailActivity.class);
                        intent.putExtra("func", selectedFunc);
                        startActivity(intent);
                    }
                }
            }
        });

        // Delete data item dialog
        if (selectedFunc == SetNCProgram) {
            mListView.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
                @Override
                public boolean onItemLongClick(AdapterView<?> arg0, View view,
                                               final int position, long id) {

                    AlertDialog.Builder builder = new AlertDialog.Builder(NCProgramFileActivity.this);
                    builder.setMessage(R.string.delete_nclist_info);
                    builder.setTitle(R.string.delete_data);
                    builder.setPositiveButton(R.string.cancel, null);
                    builder.setNegativeButton(R.string.delete, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            FileSaveManager fileManager = sharedManager();
                            fileManager.deleteFile(position);
                            fileAdapter.notifyDataSetChanged();
                        }
                    });
                    builder.create().show();
                    return true;
                }
            });
        }
    }

    /**
     * Create option menu
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        boolean ret = super.onCreateOptionsMenu(menu);
        if (selectedFunc == GetNCProgram) {
            menu.findItem(R.id.menu_save_file).setVisible(true);
            menu.findItem(R.id.menu_mkdir).setVisible(true);
        }
        return ret;
    }

    /**
     * Button onclick events
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        switch (item.getItemId()) {
            case R.id.menu_save_file:
                FileSaveManager.sharedManager().saveFile();
                fileAdapter.notifyDataSetChanged();
                AlertDialog.Builder builder = new AlertDialog.Builder(NCProgramFileActivity.this);
                builder.setMessage(R.string.save_complete);
                builder.setTitle(getTitle());
                builder.setPositiveButton(R.string.ok, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        Intent intent = new Intent();
                        setResult(RESULT_OK, intent);
                        finish();
                    }
                });
                builder.create().show();
                return true;
            case R.id.menu_mkdir:
                final EditText input = new EditText(NCProgramFileActivity.this);
                input.setInputType(InputType.TYPE_CLASS_TEXT);
                //input.settext
                input.setSingleLine();
                builder = new AlertDialog.Builder(NCProgramFileActivity.this);
                builder.setMessage(R.string.input_new_folder_name);
                builder.setView(input);
                builder.setTitle(R.string.new_folder);
                builder.setPositiveButton(R.string.create_folder, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        if (!input.getText().toString().isEmpty()) {
                            FileSaveManager.sharedManager().makeDir(input.getText().toString());
                            fileAdapter.notifyDataSetChanged();
                        }
                    }
                });
                builder.setNegativeButton(R.string.cancel, null);
                builder.create().show();
                return true;
            case android.R.id.home:
                Intent intent = new Intent();
                setResult(RESULT_CANCELED, intent);
                finish();
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

    /**
     * Destroy activity
     */
    @Override
    protected void onDestroy() {
        super.onDestroy();
        FileSaveManager.sharedManager().setDefaultPath();
    }
}
