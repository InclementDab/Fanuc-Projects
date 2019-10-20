
#include "MainFrameHandler.h"
#include <wx/windowptr.h>

MainFrameHandler::MainFrameHandler() : MainFrame(nullptr)
{

}


void MainFrameHandler::OnAddMachineButtonClick(wxCommandEvent& event)
{

	wxWindowPtr<CreateMachineDialogHandler> dlg(new CreateMachineDialogHandler(this, wxID_ANY, L"Create Machine")); //new CreateMachineDialog(this, wxID_ANY, L"Create Machine")
	
	dlg->ShowWindowModalThenDo([this, dlg](int retcode)
		{
			switch (retcode)
			{
			case(wxID_OK):
				objectTree->AddItem(dlg->GetResult());
				break;
			
			case(wxID_CANCEL):
				break;
			}
		});

	event.Skip();
}

void MainFrameHandler::OnAddFolderButtonClick(wxCommandEvent& event)
{
	Folder* folder = new Folder("Folder0");
	objectTree->AddItem(folder);
	event.Skip();
}




