#ifndef MAINFRAME_H
#define MAINFRAME_H

//(*Headers(MainFrame)
#include <wx/button.h>
#include <wx/frame.h>
#include <wx/gbsizer.h>
#include <wx/grid.h>
#include <wx/sizer.h>
#include <wx/treectrl.h>
//*)

#include "Machine.h"
#include "resource.h"
#include <wx/treebase.h>
#include "CreateMachineDialog.h"


class MainFrame: public wxFrame
{
	public:

		MainFrame(wxWindow* parent);
		virtual ~MainFrame();

		//(*Declarations(MainFrame)
		wxButton* AddFolderButton;
		wxButton* AddMachineButton;
		wxGrid* mProgramGrid;
		wxTreeCtrl* mTreeCtrl;
		//*)

		//(*Yeet(MainFrame)
		void AddItem(ModelTreeItem &item) const;
		void AddItem(ModelTreeItem &item, ModelTreeItem &parent) const;
		wxImageList* w_imageList;
		//*)

	protected:

		//(*Identifiers(MainFrame)
		static const long ID_MACHINELIST;
		static const long ID_ADDFOLDER;
		static const long ID_ADDMACHINE;
		static const long ID_ANY;
		//*)

	private:

		//(*Handlers(MainFrame)
		void OnAddFolderButtonClick(wxCommandEvent& event) const;
		void OnAddMachineButtonClick(wxCommandEvent& event);
		//*)


		DECLARE_EVENT_TABLE()
};




#endif
