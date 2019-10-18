#pragma once
#ifndef _MACHINE_H_
#define _MACHINE_H_

#include <wx/wx.h>
#include <wx/treectrl.h>
#include <30i/Fwlib32.h>
#include "Controller.h"

class ModelBase : public wxTreeItemData
{
public:
	ModelBase(wxString name);
	wxString Name;

	virtual wxMenu* GetContextMenu() { return new wxMenu(); };
};

class Machine : public ModelBase
{
public:
	Machine(wxString name, Controller* mcontrol);
	Controller* mController;
	wxMenu* GetContextMenu() override;
};


class Folder : public ModelBase
{
public:
	Folder(wxString name);
	wxMenu* GetContextMenu();
};



#endif _MACHINE_H_

