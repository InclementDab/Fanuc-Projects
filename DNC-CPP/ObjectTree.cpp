#include "ObjectTree.h"


ObjectTree::ObjectTree(wxWindow *parent, wxWindowID id, const wxPoint &pos, const wxSize &size, long style) : wxPanel(parent, id, pos, size, style)
{
	w_treeCtrl = new wxTreeCtrl(this, wxID_ANY, pos, size, wxTR_DEFAULT_STYLE | wxTR_EDIT_LABELS | wxTR_HIDE_ROOT | wxTR_NO_LINES);
	w_treeCtrl->SetSize(wxSize(300, 450));
	w_treeCtrl->AddRoot("");

	w_imageList = new wxImageList(16, 16);
	w_imageList->Add(wxICON(IDI_FOLDER));
	w_imageList->Add(wxICON(IDI_MACHINE));

	w_treeCtrl->SetImageList(w_imageList);

	w_treeCtrl->Connect(wxEVT_COMMAND_TREE_END_LABEL_EDIT, wxTreeEventHandler(ObjectTree::OnEndLabelEdit), NULL, this);
	w_treeCtrl->Connect(wxEVT_COMMAND_TREE_ITEM_MENU, wxTreeEventHandler(ObjectTree::OnItemContextMenu), NULL, this);
	

}

ObjectTree::~ObjectTree()
{
	w_treeCtrl->Disconnect(wxEVT_COMMAND_TREE_END_LABEL_EDIT, wxTreeEventHandler(ObjectTree::OnEndLabelEdit), NULL, this);
	w_treeCtrl->Disconnect(wxEVT_COMMAND_TREE_ITEM_MENU, wxTreeEventHandler(ObjectTree::OnItemContextMenu), NULL, this);
}

void ObjectTree::RebuildTree()
{

}

wxTreeItemId ObjectTree::AddItem(ModelBase* item, ModelBase* parent)
{
	int icon = (typeid(*item) == typeid(Machine)) ? 1 : 0;
	wxTreeItemId parentId = parent->GetId();

	if (parentId != 0)
	{
		if (typeid(*parent) == typeid(Folder))
		{
			auto r = w_treeCtrl->InsertItem(parentId, w_treeCtrl->GetLastChild(parentId), item->Name, icon, icon, item);
			w_treeCtrl->Expand(parentId);
			return r;
		}
		else if (typeid(*parent) == typeid(Machine))
		{
			return w_treeCtrl->InsertItem(w_treeCtrl->GetItemParent(parentId), parentId, item->Name, icon, icon, item);
		}
	}
	else
	{
		return w_treeCtrl->AppendItem(w_treeCtrl->GetRootItem(), item->Name, icon, icon, item);
	}
}

wxTreeItemId ObjectTree::AddItem(ModelBase* item)
{
	int icon = (typeid(*item) == typeid(Machine)) ? 1 : 0;
	return w_treeCtrl->AppendItem(w_treeCtrl->GetRootItem(), item->Name, icon, icon, item);
}

void ObjectTree::RemoveItem(ModelBase* item)
{

}

ModelBase* ObjectTree::GetObjectFromItemTree(wxTreeItemId item)
{
	return (ModelBase*)w_treeCtrl->GetItemData(item);
}


void ObjectTree::OnEndLabelEdit(wxTreeEvent& event)
{
	ModelBase* mBase = (ModelBase*)w_treeCtrl->GetItemData(event.GetItem()); 
	
	mBase->Name = event.GetLabel();
	event.Skip();
}

void ObjectTree::OnItemContextMenu(wxTreeEvent& event)
{
	wxMenu* contextMenu = GetObjectFromItemTree(event.GetItem())->GetContextMenu();
	
	Connect(wxID_CUT, wxEVT_COMMAND_MENU_SELECTED, wxCommandEventHandler(ObjectTree::OnCut));
	Connect(wxID_COPY, wxEVT_COMMAND_MENU_SELECTED, wxCommandEventHandler(ObjectTree::OnCopy));
	Connect(wxID_PASTE, wxEVT_COMMAND_MENU_SELECTED, wxCommandEventHandler(ObjectTree::OnPaste));
	Connect(wxID_DELETE, wxEVT_COMMAND_MENU_SELECTED, wxCommandEventHandler(ObjectTree::OnDelete));
	Connect(wxID_DEFAULT, wxEVT_COMMAND_MENU_SELECTED, wxCommandEventHandler(ObjectTree::OnRename));
	Connect(wxID_PROPERTIES, wxEVT_COMMAND_MENU_SELECTED, wxCommandEventHandler(ObjectTree::OnProperties));
	
	PopupMenu(contextMenu, event.GetPoint());


	event.Skip();
}

void ObjectTree::OnCut(wxCommandEvent& event)
{
	ModelBase mBase = *GetObjectFromItemTree(w_treeCtrl->GetSelection());
	OutputDebugString(mBase.Name);
	event.Skip();
}

void ObjectTree::OnCopy(wxCommandEvent& event)
{
	event.Skip();
}

void ObjectTree::OnPaste(wxCommandEvent& event)
{

	event.Skip();
}

void ObjectTree::OnDelete(wxCommandEvent& event)
{
	w_treeCtrl->Delete(w_treeCtrl->GetSelection());
	event.Skip();
}

void ObjectTree::OnRename(wxCommandEvent& event)
{
	w_treeCtrl->EditLabel(w_treeCtrl->GetSelection());
	event.Skip();
}

void ObjectTree::OnProperties(wxCommandEvent& event)
{
	event.Skip();
}
