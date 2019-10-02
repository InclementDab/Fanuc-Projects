package jp.co.fanuc.focas2sample.model;


import android.content.Context;

import jp.co.fanuc.focas2sample.R;

/**
 * The entity class that is managed for displaying the got NC program.
 */
public class NCDirInfo {

    private short data_kind;
    private short year;
    private short mon;
    private short day;
    private short hour;
    private short min;
    private short sec;
    private int size;
    private int attr;
    private String d_f;
    private String comment;
    private String o_time;

    public short getData_kind() {
        return data_kind;
    }

    public void setData_kind(short data_kind) {
        this.data_kind = data_kind;
    }

    public short getYear() {
        return year;
    }

    public void setYear(short year) {
        this.year = year;
    }

    public short getMon() {
        return mon;
    }

    public void setMon(short mon) {
        this.mon = mon;
    }

    public short getDay() {
        return day;
    }

    public void setDay(short day) {
        this.day = day;
    }

    public short getHour() {
        return hour;
    }

    public void setHour(short hour) {
        this.hour = hour;
    }

    public short getMin() {
        return min;
    }

    public void setMin(short min) {
        this.min = min;
    }

    public short getSec() {
        return sec;
    }

    public void setSec(short sec) {
        this.sec = sec;
    }

    public int getSize() {
        return size;
    }

    public void setSize(int size) {
        this.size = size;
    }

    public int getAttr() {
        return attr;
    }

    public void setAttr(int attr) {
        this.attr = attr;
    }

    public String getD_f() {
        return d_f;
    }

    public void setD_f(String d_f) {
        this.d_f = d_f;
    }

    public String getComment() {
        return comment;
    }

    public void setComment(String comment) {
        this.comment = comment;
    }

    public String getO_time() {
        return o_time;
    }

    public void setO_time(String o_time) {
        this.o_time = o_time;
    }

    public void setUpFolder(Context context) {
        data_kind = 0;
        year = 0;
        mon = 0;
        day = 0;
        hour = 0;
        min = 0;
        sec = 0;
        size = 0;
        attr = 0;
        d_f = context.getString(R.string.upper_folder);
        comment = null;
        o_time = null;
    }
}
