#include "MainFrame.h"

//(*InternalHeaders(MainFrame)
#include <wx/intl.h>
#include <wx/string.h>
//*)


//(*IdInit(MainFrame)
const long MainFrame::ID_MACHINELIST = wxNewId();
const long MainFrame::ID_ADDFOLDER = wxNewId();
const long MainFrame::ID_ADDMACHINE = wxNewId();
const long MainFrame::ID_ANY = wxNewId();
//*)

BEGIN_EVENT_TABLE(MainFrame, wxFrame)
//(*EventTable(MainFrame)
//*)
END_EVENT_TABLE()

MainFrame::MainFrame(wxWindow* parent)
{
	//(*Initialize(MainFrame)
	wxBoxSizer* BoxSizer1;
	wxGridBagSizer* mSizer;

	Create(parent, wxID_ANY, _("DNC"), wxDefaultPosition, wxDefaultSize, wxDEFAULT_FRAME_STYLE, _T("wxID_ANY"));
	SetClientSize(wxSize(900,600));
	mSizer = new wxGridBagSizer(0, 0);
	mTreeCtrl = new wxTreeCtrl(this, ID_MACHINELIST, wxDefaultPosition, wxSize(300,450), wxTR_DEFAULT_STYLE, wxDefaultValidator, _T("ID_MACHINELIST"));
	mSizer->Add(mTreeCtrl, wxGBPosition(0, 0), wxDefaultSpan, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	BoxSizer1 = new wxBoxSizer(wxHORIZONTAL);
	AddFolderButton = new wxButton(this, ID_ADDFOLDER, _("+Folder"), wxDefaultPosition, wxDefaultSize, 0, wxDefaultValidator, _T("ID_ADDFOLDER"));
	BoxSizer1->Add(AddFolderButton, 1, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	AddMachineButton = new wxButton(this, ID_ADDMACHINE, _("+Machine"), wxDefaultPosition, wxDefaultSize, 0, wxDefaultValidator, _T("ID_ADDMACHINE"));
	BoxSizer1->Add(AddMachineButton, 1, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	mSizer->Add(BoxSizer1, wxGBPosition(1, 0), wxDefaultSpan, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	mProgramGrid = new wxGrid(this, ID_ANY, wxDefaultPosition, wxDefaultSize, 0, _T("ID_ANY"));
	mProgramGrid->CreateGrid(0,4);
	mProgramGrid->EnableEditing(true);
	mProgramGrid->EnableGridLines(true);
	mProgramGrid->SetDefaultCellFont( mProgramGrid->GetFont() );
	mProgramGrid->SetDefaultCellTextColour( mProgramGrid->GetForegroundColour() );
	mSizer->Add(mProgramGrid, wxGBPosition(0, 1), wxGBSpan(2, 1), wxALL|wxEXPAND, 5);
	SetSizer(mSizer);
	SetSizer(mSizer);
	Layout();

	Connect(ID_ADDFOLDER,wxEVT_COMMAND_BUTTON_CLICKED,(wxObjectEventFunction)&MainFrame::OnAddFolderButtonClick);
	Connect(ID_ADDMACHINE,wxEVT_COMMAND_BUTTON_CLICKED,(wxObjectEventFunction)&MainFrame::OnAddMachineButtonClick);
	//*)

	w_imageList = new wxImageList(16, 16);
	w_imageList->Add(wxICON(IDI_FOLDER));
	w_imageList->Add(wxICON(IDI_MACHINE));
	mTreeCtrl->SetImageList(w_imageList);

}

MainFrame::~MainFrame()
{
	//(*Destroy(MainFrame)
	delete w_imageList;
	//*)
}

void MainFrame::OnAddMachineButtonClick(wxCommandEvent& event)
{
	auto* dlg = new CreateMachineDialog(this);
	dlg->ShowWindowModal();

	auto ctrlList = Controller::GetControllerList();

	ModelTreeItem item = ModelTreeItem(new Machine(dlg->nameCtrl->GetLineText(0), ctrlList[dlg->Choice1->GetSelection()]));
	AddItem(item);

	delete dlg;
}

void MainFrame::OnAddFolderButtonClick(wxCommandEvent& event) const
{
	ModelTreeItem item = ModelTreeItem(new Folder("Folder0"));
	AddItem(item);
}

void MainFrame::AddItem(ModelTreeItem &item, ModelTreeItem&parent) const
{
	if (typeid(parent.Model) == typeid(Folder))
	{
		mTreeCtrl->InsertItem(parent, mTreeCtrl->GetLastChild(parent),
							  item.Model->Name, item.Model->GetIcon(),
							  item.Model->GetIcon());

		mTreeCtrl->Expand(parent);
		return;
	}
	if (typeid(parent.Model) == typeid(Machine))
	{
		mTreeCtrl->InsertItem(mTreeCtrl->GetItemParent(parent), parent,
							  item.Model->Name, item.Model->GetIcon(),
							  item.Model->GetIcon());
	}
	else
	{
		mTreeCtrl->AppendItem(mTreeCtrl->GetRootItem(), item.Model->Name,
							  item.Model->GetIcon(), item.Model->GetIcon());
	}


}


void MainFrame::AddItem(ModelTreeItem &item) const
{
	if (mTreeCtrl->GetSelection())
	{

		AddItem(item, dynamic_cast<ModelTreeItem>(mTreeCtrl->GetSelection()));
	}
	//mTreeCtrl->AppendItem(mTreeCtrl->GetRootItem(), item.Model->Name,
		//				  item.Model->GetIcon(), item.Model->GetIcon());

}



