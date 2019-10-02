package jp.co.fanuc.focas2sample.adapter;

import android.content.Context;
import android.graphics.Color;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.List;

import jp.co.fanuc.focas2sample.R;
import jp.co.fanuc.focas2sample.model.NCConnectionSetting;
import jp.co.fanuc.focas2sample.model.StatusConverter;

import static jp.co.fanuc.fwlibe1.Fwlibe1.EW_OK;

/**
 * Get status activity adapter.
 */
public class GetStatusAdapter extends ListAdapter {

    public GetStatusAdapter(Context context, List<NCConnectionSetting> list) {
        super(context, list);
    }

    /**
     * Display contents as NC Status list.
     */
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {

        GetStatusAdapter.ViewHolder holder = null;

        if (convertView == null) {
            holder = new GetStatusAdapter.ViewHolder();

            convertView = mInflater.inflate(R.layout.get_status_list, null);

            holder.ncDataView = (TextView) convertView.findViewById(R.id.nc_get_list);
            holder.errImageView = (ImageView) convertView.findViewById(R.id.err_msg);
            holder.modeView = (TextView) convertView.findViewById(R.id.nc_mode);
            holder.emgView = (TextView) convertView.findViewById(R.id.nc_emg);
            holder.almView = (TextView) convertView.findViewById(R.id.nc_alm);
            convertView.setTag(holder);
        } else {
            holder = (GetStatusAdapter.ViewHolder) convertView.getTag();
        }
        NCConnectionSetting info = ncDataList.get(position);
        if (info instanceof NCConnectionSetting) {
            holder.ncDataView.setText(this.ncDataList.get(position).getNcName());
            // If occurring error, display error icon.
            if (info.ret != EW_OK) {
                holder.errImageView.setImageResource(R.drawable.err);
            } else {
                holder.errImageView.setImageDrawable(null);
            }
            if (info.converter != null) {
                holder.modeView.setText(StatusConverter.ereaseLowercase(info.converter.aut_status));
                holder.almView.setText(StatusConverter.ereaseLowercase(info.converter.alarm_status));
                holder.emgView.setText(StatusConverter.ereaseLowercase(info.converter.emergency_status));
                if (info.converter.alarm_status.equals("***")) {
                    holder.almView.setTextColor(Color.BLACK);
                } else {
                    holder.almView.setTextColor(Color.RED);
                }

                if (info.converter.emergency_status.equals("")) {
                    holder.emgView.setTextColor(Color.BLACK);
                    holder.emgView.setText("*** ***");
                } else {
                    holder.emgView.setTextColor(Color.RED);
                }
            } else {
                holder.modeView.setText("");
                holder.almView.setText("");
                holder.emgView.setText("");
            }
        }

        return convertView;
    }

    static class ViewHolder {
        public TextView ncDataView;
        public ImageView errImageView;
        public TextView modeView;
        public TextView emgView;
        public TextView almView;
    }
}