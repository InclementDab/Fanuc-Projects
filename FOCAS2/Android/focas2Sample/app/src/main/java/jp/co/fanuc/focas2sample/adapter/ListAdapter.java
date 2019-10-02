package jp.co.fanuc.focas2sample.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import java.util.List;

import jp.co.fanuc.focas2sample.R;
import jp.co.fanuc.focas2sample.model.NCConnectionSetting;

/**
 * Connection destination list adapter.
 */
public class ListAdapter extends BaseAdapter {
    protected LayoutInflater mInflater;
    protected Context mContext;
    List<NCConnectionSetting> ncDataList;

    public ListAdapter(Context context, List<NCConnectionSetting> list) {
        this.mInflater = LayoutInflater.from(context);
        this.mContext = context;
        this.ncDataList = list;
    }

    @Override
    public int getCount() {
        return this.ncDataList.size();
    }

    @Override
    public NCConnectionSetting getItem(int position) {
        return ncDataList.get(position);
    }

    /**
     * Display NC settings list.
     */
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {

        ViewHolder holder = null;

        if (convertView == null) {
            holder = new ViewHolder();

            convertView = mInflater.inflate(R.layout.vlist, null);

            holder.ncDataView = (TextView) convertView.findViewById(R.id.nv_name_list);
            convertView.setTag(holder);
        } else {
            holder = (ViewHolder) convertView.getTag();
        }

        holder.ncDataView.setText(this.ncDataList.get(position).getNcName());


        return convertView;
    }

    @Override
    public long getItemId(int position) {
        return position;
    }


    static class ViewHolder {
        public TextView ncDataView;
    }
}
