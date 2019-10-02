package jp.co.fanuc.focas2sample;

import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.graphics.Bitmap;
import android.net.Uri;
import android.os.Bundle;
import android.provider.Settings;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AlertDialog;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.google.zxing.EncodeHintType;
import com.google.zxing.integration.android.IntentIntegrator;
import com.google.zxing.integration.android.IntentResult;

import java.util.HashMap;
import java.util.Map;
import java.util.regex.Pattern;

import jp.co.fanuc.focas2sample.manager.NCConnectManager;
import jp.co.fanuc.focas2sample.model.NCConnectionSetting;
import jp.co.fanuc.focas2sample.model.NCUtility;

/**
 * Create connection destinations and edit activity
 */
public class ConnectionInfoActivity extends BaseActivity {

    private TextView ncConnectName;

    private TextView ncIPAddress;

    private TextView ncPortalNumber;

    private TextView ncTimeoutTime;

    private EditText connectName;

    private EditText ipAddress;

    private EditText portalNumber;

    private EditText timeoutTime;

    private ImageView qrImageView;

    private int mConnectid;

    boolean isCreate;

    boolean isError;

    /**
     * Initialize activity
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_connection_infomation);

        connectName = (EditText) findViewById(R.id.connect_name);

        ipAddress = (EditText) findViewById(R.id.ip_address);

        portalNumber = (EditText) findViewById(R.id.portal_number);

        timeoutTime = (EditText) findViewById(R.id.timeout_time);

        ncConnectName = (TextView) findViewById(R.id.connect_nc_name);

        ncIPAddress = (TextView) findViewById(R.id.connect_nc_ipaddress);

        ncPortalNumber = (TextView) findViewById(R.id.connect_portal_number);

        ncTimeoutTime = (TextView) findViewById(R.id.connect_timeout);


        ncConnectName.setText(R.string.connect_name);
        ncIPAddress.setText(R.string.ip_address);
        ncPortalNumber.setText(R.string.portal_number);
        ncTimeoutTime.setText(R.string.timeout_time);

        int mConnectid = getIntent().getIntExtra("connectid", -1);
        if (mConnectid == -1) {
            // Set Default value if new connection information.
            connectName.setEnabled(true);
            isCreate = true;
            connectName.setText("CNC_01");
            ipAddress.setText("192.168.0.10");
            portalNumber.setText("8193");
            timeoutTime.setText("100");
        } else {
            // Set saving value if already existing connection information.
            NCConnectionSetting connect = NCConnectManager.sharedManager().getNcDataList().get(mConnectid);
            connectName.setEnabled(false);
            isCreate = false;
            connectName.setText(connect.getNcName());
            ipAddress.setText(connect.getIpAddr());
            portalNumber.setText("" + connect.getPortNo());
            timeoutTime.setText("" + connect.getTimeoutVal());
        }


    }

    /**
     * Create option menu (adding connection information, QR code function).
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        boolean ret = super.onCreateOptionsMenu(menu);
        if (isCreate) {
            menu.findItem(R.id.menu_add).setVisible(true);
            menu.findItem(R.id.menu_qrcode_read).setVisible(true);
        } else {
            menu.findItem(R.id.menu_edit).setVisible(true);
            NCConnectionSetting connect = NCConnectManager.sharedManager().getNcDataList().get(mConnectid);
            //Create QR code.
            String qrCode = "name=" + connect.getNcName() + ":ipa=" + connect.getIpAddr() + ":port=" + String.valueOf(connect.getPortNo()) + ":timeout=" + String.valueOf(connect.getTimeoutVal());
            qrImageView = (ImageView) findViewById(R.id.qr_code);
            Bitmap qrBitmap = generateBitmap(qrCode, 400, 400);
            qrImageView.setImageBitmap(qrBitmap);
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
                String ncConnectName = connectName.getText().toString();
                isError = inputCheck();
                if (!isError) {
                    if (NCConnectManager.sharedManager().hasName(ncConnectName)) {
                        AlertDialog.Builder builder = new AlertDialog.Builder(ConnectionInfoActivity.this);
                        builder.setMessage(R.string.same_name_set);
                        builder.setPositiveButton(R.string.cancel, null);
                        builder.setNegativeButton(R.string.ok, new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {
                                menuEdit();
                            }
                        });
                        builder.create().show();
                        return true;
                    }
                }

            case R.id.menu_edit:
                isError = inputCheck();
                if (!isError) {
                    menuEdit();
                }
                return true;

            case R.id.menu_qrcode_read:
                if (ContextCompat.checkSelfPermission(
                        ConnectionInfoActivity.this, android.Manifest.permission.CAMERA) == PackageManager.PERMISSION_GRANTED) {
                    // Launch camera with zxing library.
                    startCamera();
                } else {
                    // Processing when it is not permitted.
                    if (ActivityCompat.shouldShowRequestPermissionRationale(ConnectionInfoActivity.this, android.Manifest.permission.CAMERA)) {
                        warningCameraPermission();
                    } else {
                        // When asking for permission before, display a dialog requesting permission.
                        ActivityCompat.requestPermissions(ConnectionInfoActivity.this, new String[]{android.Manifest.permission.CAMERA}, 0);
                    }
                }
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

    /**
     * Configuration changed
     */
    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);

        if (newConfig.orientation == Configuration.ORIENTATION_PORTRAIT) {
            Toast.makeText(ConnectionInfoActivity.this, "-------", Toast.LENGTH_SHORT).show();
        }
        if (newConfig.orientation == Configuration.ORIENTATION_LANDSCAPE) {
            Toast.makeText(ConnectionInfoActivity.this, "|||||||", Toast.LENGTH_SHORT).show();
            qrImageView.setMaxWidth(50);
            qrImageView.setMaxHeight(50);
        }
    }

    /**
     * Request permissions result
     */
    @Override
    public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults) {
        switch (requestCode) {
            case 0: {
                if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    // Launch camera with zxing library.
                    startCamera();
                } else {
                    // Cannot get Camera Permission
                }
                break;
            }
        }
    }

    /**
     * Warn camera permission.
     */
    void warningCameraPermission() {

        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setMessage(R.string.need_camera);
        builder.setPositiveButton(R.string.no_change, null);
        builder.setNegativeButton(R.string.setting_change, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                goPermission();
            }
        });
        builder.create().show();
    }

    /**
     * Launch camera.
     */
    void startCamera() {
        IntentIntegrator integrator = new IntentIntegrator(ConnectionInfoActivity.this);
        integrator.setDesiredBarcodeFormats(IntentIntegrator.QR_CODE_TYPES);
        integrator.setCameraId(0);
        integrator.initiateScan();
    }

    /**
     * The event that is notified after capturing qr code from camera.
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {

        IntentResult result = IntentIntegrator.parseActivityResult(requestCode, resultCode, data);
        if (result != null) {
            if (result.getContents() != null) {
                Log.d(NCUtility.getLogTagWithMethod(), "scan result:" + result.getContents());
                String[] split = result.getContents().split(":");
                if (split.length != 4) {
                    return;
                }
                for (String settingString : split) {
                    String[] settingAttr = settingString.split("=");
                    if (settingAttr.length != 2) {
                        return;
                    }
                    // Get connection information from qr code.
                    if (settingAttr[0].equals("name")) {
                        connectName.setText(settingAttr[1]);
                        connectName.setError(null);
                    } else if (settingAttr[0].equals("ipa")) {
                        ipAddress.setText(settingAttr[1]);
                        ipAddress.setError(null);
                    } else if (settingAttr[0].equals("port")) {
                        portalNumber.setText(settingAttr[1]);
                        portalNumber.setError(null);
                    } else if (settingAttr[0].equals("timeout")) {
                        timeoutTime.setText(settingAttr[1]);
                        timeoutTime.setError(null);
                    } else {
                        return;
                    }
                }
            }
        } else {
            super.onActivityResult(requestCode, resultCode, data);
        }
    }

    /**
     * Create QRCord
     */
    private Bitmap generateBitmap(String qrContent, int width, int height) {
        com.google.zxing.qrcode.QRCodeWriter qrCodeWriter = new com.google.zxing.qrcode.QRCodeWriter();
        Map<EncodeHintType, String> hints = new HashMap<>();
        hints.put(com.google.zxing.EncodeHintType.CHARACTER_SET, "utf-8");
        try {
            com.google.zxing.common.BitMatrix encode = qrCodeWriter.encode(qrContent, com.google.zxing.BarcodeFormat.QR_CODE, width, height, hints);
            int[] pixels = new int[width * height];
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    if (encode.get(j, i)) {
                        pixels[i * width + j] = 0x00000000;
                    } else {
                        pixels[i * width + j] = 0xffffffff;
                    }
                }
            }
            return Bitmap.createBitmap(pixels, 0, width, width, height, Bitmap.Config.RGB_565);
        } catch (com.google.zxing.WriterException e) {
            e.printStackTrace();
        }
        return null;
    }

    /**
     * Validation check and return error.
     */
    private boolean inputCheck() {
        boolean isErr = false;
        if (connectName.getText().toString() == null || connectName.getText().toString().isEmpty()) {
            connectName.setError(getString(R.string.error_need_input));
            isErr = true;
        }
        if (ipAddress.getText().toString() == null || ipAddress.getText().toString().isEmpty()) {
            ipAddress.setError(getString(R.string.error_need_input));
            isErr = true;
        }
        if (!checkIP(ipAddress.getText().toString()) || ipAddress.getText().toString().equals("0.0.0.0") || ipAddress.getText().toString().equals("127.0.0.1")) {
            ipAddress.setError(getString(R.string.error_need_input));
            isErr = true;
        }
        try {
            if (portalNumber.getText().toString() == null || portalNumber.getText().toString().isEmpty() || Short.valueOf(portalNumber.getText().toString()) < 1 || Short.valueOf(portalNumber.getText().toString()) > 65535) {
                portalNumber.setError(getString(R.string.error_need_input));
                isErr = true;
            }
        } catch (Exception e) {
            portalNumber.setError(getString(R.string.error_need_input));
            isErr = true;
        }
        try {
            if (timeoutTime.getText().toString() == null || timeoutTime.getText().toString().isEmpty() || Short.valueOf(timeoutTime.getText().toString()) < 1 || Short.valueOf(timeoutTime.getText().toString()) > 65535) {
                timeoutTime.setError(getString(R.string.error_need_input));
                isErr = true;
            }
        } catch (Exception e) {
            timeoutTime.setError(getString(R.string.error_need_input));
            isErr = true;
        }

        return isErr;

    }

    /**
     * Add or Edit connection information to connection list.
     */
    private void menuEdit() {
        NCConnectionSetting connect = new NCConnectionSetting(connectName.getText().toString(),
                ipAddress.getText().toString(),
                Short.parseShort(portalNumber.getText().toString()),
                Integer.parseInt(timeoutTime.getText().toString()));

        if (!NCConnectManager.sharedManager().hasName(connectName.getText().toString())) {
            NCConnectManager.sharedManager().addConnect(connect);
        } else {
            int pos = -1;
            for (NCConnectionSetting con : NCConnectManager.sharedManager().getNcDataList()) {
                if (connectName.getText().toString().equals(con.getNcName())) {
                    pos = NCConnectManager.sharedManager().getNcDataList().indexOf(con);
                }
            }
            NCConnectManager.sharedManager().replaceConnect(pos, connect);
        }
        Intent intentBack = new Intent();
        setResult(RESULT_OK, intentBack);
        finish();
    }

    /**
     * Check IP address.
     */
    private boolean checkIP(String ipAddress) {
        Pattern pattern = Pattern
                .compile("^((\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5]"
                        + "|[*])\\.){3}(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5]|[*])$");
        return pattern.matcher(ipAddress).matches();
    }

    /**
     * Jump to camera permission setting on setting application.
     */
    private void goPermission() {
        Intent intent = new Intent(Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
        Uri uri = Uri.fromParts("package", getPackageName(), null); // For Fragment, getContext().getPackageName()
        intent.setData(uri);
        startActivity(intent);
    }
}

