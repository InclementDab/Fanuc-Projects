#pragma once

#include <iostream>
#include <map>
#include <wx/wx.h>
#include <wx/treectrl.h>
#include "Machine.h"

class ObjectTree : public wxPanel
{

public:
	ObjectTree(wxWindow *parent, wxWindowID id, const wxPoint& pos = wxDefaultPosition, const wxSize& size = wxSize(-1, -1), long style = 2621440L);
	~ObjectTree();

	void RebuildTree();
	wxTreeItemId AddItem(ModelBase* item, ModelBase* parent);
	wxTreeItemId AddItem(ModelBase* item);
	void RemoveItem(ModelBase* item);
	
	ModelBase* GetObjectFromItemTree(wxTreeItemId item);

	wxTreeCtrl* w_treeCtrl = nullptr;

protected:
	
	wxImageList* w_imageList = nullptr;

private:

	void OnEndLabelEdit(wxTreeEvent& event);
	void OnItemContextMenu(wxTreeEvent& event);

	void OnCut(wxCommandEvent& event);
	void OnCopy(wxCommandEvent& event);
	void OnPaste(wxCommandEvent& event);
	void OnDelete(wxCommandEvent& event);
	void OnRename(wxCommandEvent& event);
	void OnProperties(wxCommandEvent& event);


};

