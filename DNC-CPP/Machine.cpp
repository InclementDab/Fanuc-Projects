
#include "Machine.h"
#include <iostream>
#include <vector>


ModelBase::~ModelBase()
{
}

wxMenu* Machine::GetContextMenu()
{
	wxMenu menu = wxMenu();
	menu.Append(new wxMenuItem(&menu, wxID_CUT));
	menu.Append(new wxMenuItem(&menu, wxID_COPY));
	menu.Append(new wxMenuItem(&menu, wxID_PASTE));
	menu.AppendSeparator();
	menu.Append(new wxMenuItem(&menu, wxID_DELETE));
	menu.Append(new wxMenuItem(&menu, wxID_DEFAULT, wxString("Rename")));
	menu.Append(new wxMenuItem(&menu, wxID_PROPERTIES));
	
	return &menu;
}


wxMenu* Folder::GetContextMenu()
{
	wxMenu menu = wxMenu();
	menu.Append(new wxMenuItem(&menu, wxID_CUT));
	menu.Append(new wxMenuItem(&menu, wxID_COPY));
	menu.Append(new wxMenuItem(&menu, wxID_PASTE));
	menu.AppendSeparator();
	menu.Append(new wxMenuItem(&menu, wxID_DELETE));
	menu.Append(new wxMenuItem(&menu, wxID_ANY, wxString("Rename")));
	return &menu;
}

ModelTreeItem::~ModelTreeItem()
{
	delete Model;
}
