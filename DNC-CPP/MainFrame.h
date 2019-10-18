///////////////////////////////////////////////////////////////////////////
// C++ code generated with wxFormBuilder (version Oct 26 2018)
// http://www.wxformbuilder.org/
//
// PLEASE DO *NOT* EDIT THIS FILE!
///////////////////////////////////////////////////////////////////////////

#pragma once

#include <wx/artprov.h>
#include <wx/xrc/xmlres.h>
#include <wx/string.h>
#include <wx/menu.h>
#include <wx/gdicmn.h>
#include <wx/font.h>
#include <wx/colour.h>
#include <wx/settings.h>
#include <wx/treectrl.h>
#include <wx/bitmap.h>
#include <wx/image.h>
#include <wx/icon.h>
#include <wx/button.h>
#include <wx/sizer.h>
#include <wx/grid.h>
#include <wx/gbsizer.h>
#include <wx/statusbr.h>
#include <wx/frame.h>
#include <wx/textctrl.h>
#include <wx/panel.h>
#include <wx/statbox.h>
#include <wx/choice.h>
#include <wx/dialog.h>

///////////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////////////
/// Class MainFrame
///////////////////////////////////////////////////////////////////////////////
class MainFrame : public wxFrame
{
	private:

	protected:
		wxMenuBar* mMenuBar;
		wxMenu* newMenu;
		wxMenu* editMenu;
		wxMenu* viewMenu;
		wxTreeCtrl* mMachineListView;
		wxButton* mAddFolderButton;
		wxButton* mAddMachineButton;
		wxGrid* mProgramGrid;
		wxStatusBar* mStatusBar;

		// Virtual event handlers, overide them in your derived class
		virtual void OnEndLabelEdit( wxTreeEvent& event ) { event.Skip(); }
		virtual void OnAddFolderButtonClick( wxCommandEvent& event ) { event.Skip(); }
		virtual void OnAddMachineButtonClick( wxCommandEvent& event ) { event.Skip(); }


	public:

		MainFrame( wxWindow* parent, wxWindowID id = wxID_ANY, const wxString& title = wxT("DNC Application"), const wxPoint& pos = wxDefaultPosition, const wxSize& size = wxSize( 900,600 ), long style = wxDEFAULT_FRAME_STYLE|wxTAB_TRAVERSAL );

		~MainFrame();

};

///////////////////////////////////////////////////////////////////////////////
/// Class EditMachineWindow
///////////////////////////////////////////////////////////////////////////////
class EditMachineWindow : public wxFrame
{
	private:

	protected:

	public:

		EditMachineWindow( wxWindow* parent, wxWindowID id = wxID_ANY, const wxString& title = wxT("Edit Machine Properties"), const wxPoint& pos = wxDefaultPosition, const wxSize& size = wxSize( 500,300 ), long style = wxDEFAULT_FRAME_STYLE|wxTAB_TRAVERSAL );

		~EditMachineWindow();

};

///////////////////////////////////////////////////////////////////////////////
/// Class CreateMachineDialog
///////////////////////////////////////////////////////////////////////////////
class CreateMachineDialog : public wxDialog
{
	private:

	protected:
		wxPanel* nameTextPanel;
		wxBoxSizer* nameTextSizer;
		wxTextCtrl* nameTextBox;
		wxChoice* mControllerType;
		wxButton* createButton;
		wxButton* cancelButton;

		// Virtual event handlers, overide them in your derived class
		virtual void OnCreateMachineDialogClose( wxCloseEvent& event ) { event.Skip(); }
		virtual void OnCreateButtonClick( wxCommandEvent& event ) { event.Skip(); }
		virtual void OnCancelButtonClick( wxCommandEvent& event ) { event.Skip(); }


	public:

		CreateMachineDialog( wxWindow* parent, wxWindowID id = wxID_ANY, const wxString& title = wxEmptyString, const wxPoint& pos = wxDefaultPosition, const wxSize& size = wxDefaultSize, long style = wxDEFAULT_DIALOG_STYLE );
		~CreateMachineDialog();

};

