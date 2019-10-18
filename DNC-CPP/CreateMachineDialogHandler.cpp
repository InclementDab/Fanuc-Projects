
#include "CreateMachineDialogHandler.h"

CreateMachineDialogHandler::CreateMachineDialogHandler(wxWindow* parent, wxWindowID id, const wxString& title) : CreateMachineDialog(parent, id, title)
{

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

	Machine* MachineResult = new Machine(name, &controllerList[1]);
	
	MainFrameHandler* mFrameHandler = wxDynamicCast(GetParent(), MainFrameHandler);
	mFrameHandler->AddModelBase(MachineResult);

	Close();
	event.Skip();
}

void CreateMachineDialogHandler::OnCancelButtonClick(wxCommandEvent& event)
{
	Close();
	event.Skip();
}
