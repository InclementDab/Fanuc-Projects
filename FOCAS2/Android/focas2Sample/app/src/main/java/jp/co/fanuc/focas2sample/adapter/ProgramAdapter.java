package jp.co.fanuc.focas2sample.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import jp.co.fanuc.focas2sample.R;
import jp.co.fanuc.focas2sample.manager.NCDirectoryManager;
import jp.co.fanuc.focas2sample.model.NCDirInfo;
/**
 * Program list adapter
 */
public class ProgramAdapter extends BaseAdapter {
    private LayoutInflater mInflater;


    public ProgramAdapter(Context context) {
        this.mInflater = LayoutInflater.from(context);
    }

    @Override
    public int getCount() {
        return NCDirectoryManager.sharedManager().getNCDirInfoArray().size();
    }

    @Override
    public NCDirInfo getItem(int position) {
        return NCDirectoryManager.sharedManager().getNCDirInfoArray().get(position);
    }

    /**
     * Display NC program list including directory and files.
     */
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {

        ProgramAdapter.ViewHolder holder = null;

        if (convertView == null) {
            holder = new ProgramAdapter.ViewHolder();

            convertView = mInflater.inflate(R.layout.file_view, null);

            holder.iconView = (ImageView) convertView.findViewById(R.id.icon);
            holder.titleView = (TextView) convertView.findViewById(R.id.title);
            convertView.setTag(holder);
        } else {
            holder = (ProgramAdapter.ViewHolder) convertView.getTag();
        }
        NCDirInfo info = NCDirectoryManager.sharedManager().getNCDirInfoArray().get(position);
        holder.titleView.setText(info.getD_f());
        if (info.getData_kind() == 0) {
            holder.iconView.setImageResource(R.drawable.folder_icon);
        } else if (info.getData_kind() == 1) {
            holder.iconView.setImageResource(R.drawable.file_icon);
        } else {
            holder.iconView.setImageDrawable(null);
        }
        return convertView;
    }

    @Override
    public long getItemId(int position) {
        return position;
    }


    static class ViewHolder {
        public ImageView iconView;
        public TextView titleView;
    }
}

