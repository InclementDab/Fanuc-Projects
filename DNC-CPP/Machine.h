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
};

class Machine : public ModelBase
{
public:
	Machine(wxString name, Controller* mcontrol);
	Controller* mController;
};


class Folder : public ModelBase
{
public:
	Folder(wxString name);
};



#endif _MACHINE_H_

