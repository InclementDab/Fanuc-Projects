
#include "MainFrameHandler.h"

MainFrameHandler::MainFrameHandler() : MainFrame(nullptr)
{
	mMachineListView->AddRoot("");

	wxImageList* ImageList = new wxImageList(16, 16);
	ImageList->Add(wxICON(IDI_FOLDER));
	ImageList->Add(wxICON(IDI_MACHINE));
	mMachineListView->SetImageList(ImageList);
}



void MainFrameHandler::AddModelBase(ModelBase* mBase)
{
	int icon = (typeid(*mBase) == typeid(Machine)) ? 0 : 1;
	wxTreeItemId SelectedItem = mMachineListView->GetSelection();
	
	if (SelectedItem != 0) 
	{
		ModelBase* SelectedModelBase = (ModelBase*)mMachineListView->GetItemData(SelectedItem);

		if (typeid(*SelectedModelBase) == typeid(Folder))
		{
			mMachineListView->InsertItem(SelectedItem, mMachineListView->GetLastChild(SelectedItem), mBase->Name, icon, icon, mBase);
			mMachineListView->Expand(SelectedItem);
		}
		else if (typeid(*SelectedModelBase) == typeid(Machine))
		{
			mMachineListView->InsertItem(mMachineListView->GetItemParent(SelectedItem), SelectedItem, mBase->Name, icon, icon, mBase);
		}
	}
	else
	{
		mMachineListView->AppendItem(mMachineListView->GetRootItem(), mBase->Name, 0, 0, mBase);
	}
}


void MainFrameHandler::OnAddMachineButtonClick(wxCommandEvent& event)
{
	CurrentDialog = new CreateMachineDialogHandler(this, wxID_ANY, L"Create Machine");
	CurrentDialog->Show();
	event.Skip();
}

void MainFrameHandler::OnAddFolderButtonClick(wxCommandEvent& event)
{
	Folder* folder = new Folder("Folder0");
	AddModelBase(folder);
	event.Skip();
}

void MainFrameHandler::OnEndLabelEdit(wxTreeEvent& event)
{
	ModelBase* mBase = (ModelBase*)mMachineListView->GetItemData(event.GetItem());
	mBase->Name = event.GetLabel();
	event.Skip();
}
