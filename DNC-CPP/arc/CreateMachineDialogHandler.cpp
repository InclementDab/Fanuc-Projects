
#include "CreateMachineDialogHandler.h"

CreateMachineDialogHandler::CreateMachineDialogHandler(wxWindow* parent, wxWindowID id, const wxString& title) : CreateMachineDialog(parent, id, title)
{

}

Machine* CreateMachineDialogHandler::GetResult()
{
	return MachineResult;
}

void CreateMachineDialogHandler::OnCreateButtonClick(wxCommandEvent& event)
{
	wxString name = nameTextBox->GetLineText(0);

	if (name.Length() <= 1)
	{
		wxColour c = wxColour(L"Red");
		nameTextPanel->SetBackgroundColour(c);
		event.Skip();
		return;
	}

	std::vector<Controller> controllerList = Controller::GetControllerList();
	MachineResult = new Machine(name, controllerList[1]);

	event.Skip();
	EndModal(wxID_OK);
}

void CreateMachineDialogHandler::OnCancelButtonClick(wxCommandEvent& event)
{
	event.Skip();
	EndModal(wxID_CANCEL);
}
