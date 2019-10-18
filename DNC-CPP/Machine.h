#pragma once
#ifndef _MACHINE_H_
#define _MACHINE_H_


#include <wx/wx.h>
#include <30i/Fwlib32.h>
#include "Controller.h"

class Machine
{
public:
	Machine(wxString name, Controller* mcontrol);
	wxString Name;
	Controller* mController;
};

#endif _MACHINE_H_

