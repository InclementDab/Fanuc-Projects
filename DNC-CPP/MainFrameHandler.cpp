
#include "MainFrameHandler.h"

MainFrameHandler::MainFrameHandler() : MainFrame(nullptr)
{
	mMachineListView->AddRoot("");
	
	wxImageList* ImageList = new wxImageList(16, 16);
	ImageList->Add(wxICON(IDI_FOLDER));
	ImageList->Add(wxICON(IDI_MACHINE));
	mMachineListView->SetImageList(ImageList);
}

void MainFrameHandler::AddMachine(Machine m)
{
	mMachineListView->AppendItem(mMachineListView->GetRootItem(), m.Name, 1, 1); // change to InsertItem later
	
}

void MainFrameHandler::AddFolder()
{
	mMachineListView->AppendItem(mMachineListView->GetRootItem(), "Folder0", 0, 0);
}

void MainFrameHandler::OnAddMachineButtonClick(wxCommandEvent& event)
{
	CurrentDialog = new CreateMachineDialogHandler(this, wxID_ANY, L"Create Machine");
	CurrentDialog->Show();

	event.Skip();
}	