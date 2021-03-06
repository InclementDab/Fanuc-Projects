#include "CreateMachineDialog.h"

//(*InternalHeaders(CreateMachineDialog)
#include <wx/button.h>
#include <wx/intl.h>
#include <wx/string.h>
//*)

#include <iostream>
#include "Controller.h"
#include <vector>

//(*IdInit(CreateMachineDialog)
const long CreateMachineDialog::ID_TEXTCTRL1 = wxNewId();
const long CreateMachineDialog::ID_CHOICE1 = wxNewId();
//*)

BEGIN_EVENT_TABLE(CreateMachineDialog,wxDialog)
	//(*EventTable(CreateMachineDialog)
	//*)
END_EVENT_TABLE()

CreateMachineDialog::CreateMachineDialog(wxWindow* parent,wxWindowID id,const wxPoint& pos,const wxSize& size)
{
	//(*Initialize(CreateMachineDialog)
	wxGridSizer* GridSizer1;
	wxStaticBoxSizer* StaticBoxSizer1;
	wxStaticBoxSizer* StaticBoxSizer2;
	wxStdDialogButtonSizer* StdDialogButtonSizer1;

	Create(parent, wxID_ANY, _("Create Machine"), wxDefaultPosition, wxDefaultSize, wxDEFAULT_DIALOG_STYLE, _T("wxID_ANY"));
	SetClientSize(wxSize(250,300));
	GridSizer1 = new wxGridSizer(3, 1, 0, 0);
	StaticBoxSizer1 = new wxStaticBoxSizer(wxHORIZONTAL, this, _("Name"));
	nameCtrl = new wxTextCtrl(this, ID_TEXTCTRL1, wxEmptyString, wxDefaultPosition, wxSize(150,-1), 0, wxDefaultValidator, _T("ID_TEXTCTRL1"));
	StaticBoxSizer1->Add(nameCtrl, 1, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	GridSizer1->Add(StaticBoxSizer1, 1, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	StaticBoxSizer2 = new wxStaticBoxSizer(wxHORIZONTAL, this, _("Controller"));
	Choice1 = new wxChoice(this, ID_CHOICE1, wxDefaultPosition, wxSize(150,-1), 0, 0, 0, wxDefaultValidator, _T("ID_CHOICE1"));
	StaticBoxSizer2->Add(Choice1, 1, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	GridSizer1->Add(StaticBoxSizer2, 1, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	StdDialogButtonSizer1 = new wxStdDialogButtonSizer();
	StdDialogButtonSizer1->AddButton(new wxButton(this, wxID_OK, _("Create")));
	StdDialogButtonSizer1->AddButton(new wxButton(this, wxID_CANCEL, wxEmptyString));
	StdDialogButtonSizer1->Realize();
	GridSizer1->Add(StdDialogButtonSizer1, 1, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	SetSizer(GridSizer1);
	SetSizer(GridSizer1);
	Layout();
	//*)

	auto ctrlList = Controller::GetControllerList();
	for(auto ctrl : ctrlList)
	{
		Choice1->Append(ctrl.Name);
	}

	Choice1->SetSelection(0);
	
}

CreateMachineDialog::~CreateMachineDialog()
{
	//(*Destroy(CreateMachineDialog)
	//*)
}

