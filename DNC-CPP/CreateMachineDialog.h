#ifndef CREATEMACHINEDIALOG_H
#define CREATEMACHINEDIALOG_H

//(*Headers(CreateMachineDialog)
#include <wx/choice.h>
#include <wx/dialog.h>
#include <wx/sizer.h>
#include <wx/textctrl.h>
//*)

class CreateMachineDialog: public wxDialog
{
	public:

		CreateMachineDialog(wxWindow* parent,wxWindowID id=wxID_ANY,const wxPoint& pos=wxDefaultPosition,const wxSize& size=wxDefaultSize);
		virtual ~CreateMachineDialog();

		//(*Declarations(CreateMachineDialog)
		wxChoice* Choice1;
		wxTextCtrl* nameCtrl;
		//*)

	protected:

		//(*Identifiers(CreateMachineDialog)
		static const long ID_TEXTCTRL1;
		static const long ID_CHOICE1;
		//*)

	private:

		//(*Handlers(CreateMachineDialog)
		//*)

		DECLARE_EVENT_TABLE()
};

#endif
