
#include "CreateMachineDialogHandler.h"

CreateMachineDialogHandler::CreateMachineDialogHandler(wxWindow* parent, wxWindowID id, const wxString& title) : CreateMachineDialog(parent, id, title)
{

}

void CreateMachineDialogHandler::OnCreateButtonClick(wxCommandEvent& event)
{
	wxString name = nameTextBox->GetLineText(0);
	std::vector<Controller> controllerList = Controller::GetControllerList();

	Machine MachineResult = Machine(name, &controllerList[1]);

	MainFrameHandler* mFrameHandler = wxDynamicCast(GetParent(), MainFrameHandler);
	mFrameHandler->AddMachine(MachineResult);

	DialogResult = true;
	Close();
	event.Skip();
}

void CreateMachineDialogHandler::OnCancelButtonClick(wxCommandEvent& event)
{
	DialogResult = false;
	Close();
	event.Skip();
}
