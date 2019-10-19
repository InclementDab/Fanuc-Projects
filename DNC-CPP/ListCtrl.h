#pragma once

#include <wx/wx.h>
#include <wx/listctrl.h>
#include <iostream>
#include <map>
#include "ListCtrlItem.h"


//////////////////////////////////////////////

class ListCtrl : public wxListCtrl
{
public:

	//void OnSelectedItem(wxListEvent& event);

	ListCtrl(wxWindow *parent, wxWindowID id, const wxPoint &pos, const wxSize &size, long style) : wxListCtrl(parent, id, pos, size, style)
	{
		//Connect(wxEVT_LIST_ITEM_SELECTED, wxListEventHandler(ListCtrl::OnSelectedItem));
		//"General" "Connection" "Controller" "Advanced"

		
		ListCtrlItem item = ListCtrlItem("General", new listCtrlItem_General(this));
		InsertItem(item);
		delete &item;
	}
};

/*
void ListCtrl::OnSelectedItem(wxListEvent& event)
{
	//LogEvent(event, "OnSelected"); future implement?
	
	long item = static_cast<long>(event.GetData());
	// left off here
}*/