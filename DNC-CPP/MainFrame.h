#ifndef MAINFRAME_H
#define MAINFRAME_H

//(*Headers(MainFrame)
#include <wx/button.h>
#include <wx/frame.h>
#include <wx/gbsizer.h>
#include <wx/sizer.h>
#include <wx/treectrl.h>
//*)


//(*YeetHeaders(MainFrame)
#include "Machine.h"
//*

class ModelBase;

class MainFrame: public wxFrame
{
	public:

		MainFrame(wxWindow* parent);
		virtual ~MainFrame();

		//(*Declarations(MainFrame)
		wxButton* AddFolderButton;
		wxButton* AddMachineButton;
		wxTreeCtrl* mTreeCtrl;
		//*)

		//(*Yeet(MainFrame)
		wxTreeItemId AddItem(ModelBase* item);
		wxTreeItemId AddItem(ModelBase* item, ModelBase* parent);
		//*)

	protected:

		//(*Identifiers(MainFrame)
		static const long ID_TREECTRL1;
		static const long ID_ADDFOLDER;
		static const long ID_ADDMACHINE;
		//*)

	private:

		//(*Handlers(MainFrame)
		void OnAddFolderButtonClick(wxCommandEvent& event);
		//*)

		DECLARE_EVENT_TABLE()
};

#endif
