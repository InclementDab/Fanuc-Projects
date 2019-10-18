
#include "Machine.h"
#include <iostream>
#include <vector>

ModelBase::ModelBase(wxString name) : wxTreeItemData()
{
	Name = name;
}

Machine::Machine(wxString name, Controller* mcontrol) : ModelBase(name)
{
	mController = mcontrol;
}

wxMenu* Machine::GetContextMenu()
{
	wxMenu* menu = new wxMenu();
	menu->Append(new wxMenuItem(menu, wxID_CUT));
	menu->Append(new wxMenuItem(menu, wxID_COPY));
	menu->Append(new wxMenuItem(menu, wxID_PASTE));
	menu->AppendSeparator();
	menu->Append(new wxMenuItem(menu, wxID_DELETE));
	menu->Append(new wxMenuItem(menu, wxID_DEFAULT, wxString("Rename")));
	menu->Append(new wxMenuItem(menu, wxID_PROPERTIES));
	
	return menu;
}


Folder::Folder(wxString name) : ModelBase(name)
{

}

wxMenu* Folder::GetContextMenu()
{
	wxMenu* menu = new wxMenu();
	menu->Append(new wxMenuItem(menu, wxID_CUT));
	menu->Append(new wxMenuItem(menu, wxID_COPY));
	menu->Append(new wxMenuItem(menu, wxID_PASTE));
	menu->AppendSeparator();
	menu->Append(new wxMenuItem(menu, wxID_DELETE));
	menu->Append(new wxMenuItem(menu, wxID_ANY, wxString("Rename")));
	return menu;
}
