package jp.co.fanuc.focas2sample.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import jp.co.fanuc.focas2sample.R;
import jp.co.fanuc.focas2sample.manager.FileSaveManager;

/**
 * Local file (in smart device) list adapter.
 */
public class FileAdapter extends BaseAdapter {
    private LayoutInflater mInflater;

    public FileAdapter(Context context) {
        this.mInflater = LayoutInflater.from(context);
    }

    @Override
    public int getCount() {
        return FileSaveManager.sharedManager().countOfContents();
    }

    @Override
    public String getItem(int position) {
        return FileSaveManager.sharedManager().getContents().get(position);
    }

    /**
     * display icon (file or directory) and name as list
     */
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {

        FileAdapter.ViewHolder holder = null;

        if (convertView == null) {
            holder = new FileAdapter.ViewHolder();

            convertView = mInflater.inflate(R.layout.file_view, null);

            holder.iconView = (ImageView) convertView.findViewById(R.id.icon);
            holder.titleView = (TextView) convertView.findViewById(R.id.title);
            convertView.setTag(holder);
        } else {
            holder = (FileAdapter.ViewHolder) convertView.getTag();
        }
        String name = FileSaveManager.sharedManager().getContentsName(position);
        holder.titleView.setText(name);

        if (FileSaveManager.sharedManager().isDir(position)) {
            holder.iconView.setImageResource(R.drawable.folder_icon);
        } else {
            holder.iconView.setImageResource(R.drawable.file_icon);
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

