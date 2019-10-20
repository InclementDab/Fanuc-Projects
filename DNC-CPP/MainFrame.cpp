#include "MainFrame.h"

//(*InternalHeaders(MainFrame)
#include <wx/intl.h>
#include <wx/string.h>
//*)


//(*IdInit(MainFrame)
const long MainFrame::ID_TREECTRL1 = wxNewId();
const long MainFrame::ID_ADDFOLDER = wxNewId();
const long MainFrame::ID_ADDMACHINE = wxNewId();
//*)

BEGIN_EVENT_TABLE(MainFrame,wxFrame)
	//(*EventTable(MainFrame)
	//*)
END_EVENT_TABLE()

MainFrame::MainFrame(wxWindow* parent)
{
	//(*Initialize(MainFrame)
	wxBoxSizer* BoxSizer1;
	wxGridBagSizer* mSizer;

	Create(parent, wxID_ANY, wxEmptyString, wxDefaultPosition, wxDefaultSize, wxDEFAULT_FRAME_STYLE, _T("wxID_ANY"));
	SetClientSize(wxSize(900,600));
	mSizer = new wxGridBagSizer(0, 0);
	mTreeCtrl = new wxTreeCtrl(this, ID_TREECTRL1, wxDefaultPosition, wxSize(300,450), wxTR_DEFAULT_STYLE, wxDefaultValidator, _T("ID_TREECTRL1"));
	mSizer->Add(mTreeCtrl, wxGBPosition(0, 0), wxDefaultSpan, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	BoxSizer1 = new wxBoxSizer(wxHORIZONTAL);
	AddFolderButton = new wxButton(this, ID_ADDFOLDER, _("+Folder"), wxDefaultPosition, wxDefaultSize, 0, wxDefaultValidator, _T("ID_ADDFOLDER"));
	BoxSizer1->Add(AddFolderButton, 1, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	AddMachineButton = new wxButton(this, ID_ADDMACHINE, _("+Machine"), wxDefaultPosition, wxDefaultSize, 0, wxDefaultValidator, _T("ID_ADDMACHINE"));
	BoxSizer1->Add(AddMachineButton, 1, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	mSizer->Add(BoxSizer1, wxGBPosition(1, 0), wxDefaultSpan, wxALL|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5);
	SetSizer(mSizer);
	SetSizer(mSizer);
	Layout();

	Connect(ID_ADDFOLDER,wxEVT_COMMAND_BUTTON_CLICKED,(wxObjectEventFunction)&MainFrame::OnAddFolderButtonClick);
	//*)
}

MainFrame::~MainFrame()
{
	//(*Destroy(MainFrame)
	//*)
}


void MainFrame::OnAddFolderButtonClick(wxCommandEvent& event)
{
	Folder* folder = new Folder("Folder0");
	AddItem(folder);
}

wxTreeItemId MainFrame::AddItem(ModelBase* item, ModelBase* parent)
{
	int icon = (typeid(*item) == typeid(Machine)) ? 1 : 0;
	wxTreeItemId parentId = parent->GetId();

	if (parentId != 0)
	{
		if (typeid(*parent) == typeid(Folder))
		{
			auto r = mTreeCtrl->InsertItem(parentId, mTreeCtrl->GetLastChild(parentId), item->Name, icon, icon, item);
			mTreeCtrl->Expand(parentId);
			return r;
		}
		else if (typeid(*parent) == typeid(Machine))
		{
			return mTreeCtrl->InsertItem(mTreeCtrl->GetItemParent(parentId), parentId, item->Name, icon, icon, item);
		}
	}
	else
	{
		return mTreeCtrl->AppendItem(mTreeCtrl->GetRootItem(), item->Name, icon, icon, item);
	}

	return NULL;
}


wxTreeItemId MainFrame::AddItem(ModelBase* item)
{
	int icon = (typeid(*item) == typeid(Machine)) ? 1 : 0;
	return mTreeCtrl->AppendItem(mTreeCtrl->GetRootItem(), item->Name, icon, icon, item);
}