///////////////////////////////////////////////////////////////////////////
// C++ code generated with wxFormBuilder (version Oct 26 2018)
// http://www.wxformbuilder.org/
//
// PLEASE DO *NOT* EDIT THIS FILE!
///////////////////////////////////////////////////////////////////////////

#pragma once

#include <wx/artprov.h>
#include <wx/xrc/xmlres.h>
#include "ListCtrl.h"
#include <wx/gdicmn.h>
#include <wx/font.h>
#include <wx/colour.h>
#include <wx/settings.h>
#include <wx/string.h>
#include <wx/panel.h>
#include <wx/sizer.h>
#include <wx/button.h>
#include <wx/wrapsizer.h>
#include <wx/dialog.h>
#include <wx/stattext.h>
#include <wx/textctrl.h>
#include <wx/statbox.h>
#include <wx/radiobut.h>
#include <wx/bitmap.h>
#include <wx/image.h>
#include <wx/icon.h>
#include <wx/checkbox.h>
#include <wx/combobox.h>

///////////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////////////
/// Class EditMachineDialog
///////////////////////////////////////////////////////////////////////////////
class EditMachineDialog : public wxDialog
{
	private:

	protected:
		ListCtrl* listCtrl;
		wxPanel* optionHost;
		wxStdDialogButtonSizer* helpButton;
		wxButton* helpButtonHelp;
		wxStdDialogButtonSizer* okCancelButtons;
		wxButton* okCancelButtonsOK;
		wxButton* okCancelButtonsCancel;

	public:

		EditMachineDialog( wxWindow* parent, wxWindowID id = wxID_ANY, const wxString& title = wxEmptyString, const wxPoint& pos = wxDefaultPosition, const wxSize& size = wxSize( 500,400 ), long style = wxDEFAULT_DIALOG_STYLE );
		~EditMachineDialog();

};

///////////////////////////////////////////////////////////////////////////////
/// Class listCtrlItem_General
///////////////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////////////
/// Class listCtrlItem_Connection
///////////////////////////////////////////////////////////////////////////////





///////////////////////////////////////////////////////////////////////////////
/// Class conn_TypeTCP
///////////////////////////////////////////////////////////////////////////////
class conn_TypeTCP : public wxPanel
{
	private:

	protected:
		wxStaticText* m_staticText6;
		wxTextCtrl* ipAddressBox;
		wxStaticText* m_staticText61;
		wxTextCtrl* portBox;

	public:

		conn_TypeTCP( wxWindow* parent, wxWindowID id = wxID_ANY, const wxPoint& pos = wxDefaultPosition, const wxSize& size = wxSize( 325,75 ), long style = wxTAB_TRAVERSAL, const wxString& name = wxEmptyString );
		~conn_TypeTCP();

};

///////////////////////////////////////////////////////////////////////////////
/// Class conn_TypeSerial
///////////////////////////////////////////////////////////////////////////////
class conn_TypeSerial : public wxPanel
{
	private:

	protected:
		wxStaticText* m_staticText6;
		wxComboBox* m_comboBox1;
		wxStaticText* m_staticText61;
		wxTextCtrl* baudBox;

	public:

		conn_TypeSerial( wxWindow* parent, wxWindowID id = wxID_ANY, const wxPoint& pos = wxDefaultPosition, const wxSize& size = wxSize( 325,75 ), long style = wxTAB_TRAVERSAL, const wxString& name = wxEmptyString );
		~conn_TypeSerial();

};

