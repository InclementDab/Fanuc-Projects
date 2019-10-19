
#pragma once

#include <wx/wx.h>
#include <wx/listctrl.h>

//////////////////////////////////////////////

class ListCtrlItem : public wxListItem
{
public:
	ListCtrlItem(wxString text, wxPanel* panel) : wxListItem()
	{
		m_text = text;
		//m_panel = panel;
		SetData(panel);
	}

	wxString m_text;
	//wxPanel* m_panel;
};



//////////////////////////////////////////////

class listCtrlItem_General : public wxPanel
{
private:

protected:
	wxStaticText* m_staticName;
	wxTextCtrl* nameTextBox;
	wxStaticText* m_staticDesc;
	wxTextCtrl* descTextBox;

public:

	listCtrlItem_General(wxWindow* parent, wxWindowID id = wxID_ANY, const wxPoint& pos = wxDefaultPosition, const wxSize& size = wxSize(350, 300), long style = wxTAB_TRAVERSAL, const wxString& name = wxEmptyString);
	~listCtrlItem_General();

};


//////////////////////////////////////////////

class listCtrlItem_Connection : public wxPanel
{
private:

protected:
	wxPanel* connectionTypeHost;
	wxRadioButton* tcpBtn;
	wxRadioButton* serialBtn;
	wxRadioButton* sshBtn;
	wxButton* testConnection;
	wxCheckBox* conOnStart;

	// Virtual event handlers, overide them in your derived class
	virtual void OnRadioButtonClick(wxCommandEvent& event) { event.Skip(); }


public:

	listCtrlItem_Connection(wxWindow* parent, wxWindowID id = wxID_ANY, const wxPoint& pos = wxDefaultPosition, const wxSize& size = wxSize(350, 300), long style = wxTAB_TRAVERSAL, const wxString& name = wxEmptyString);
	~listCtrlItem_Connection();

};
