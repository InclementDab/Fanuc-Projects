#pragma once

#ifndef _CREATEMACHINEDIALOGHANDLER_H_
#define _CREATEMACHINEDIALOGHANDLER_H_

#include <iostream>
#include <vector>
#include <wx/wx.h>
#include "Machine.h"
#include "MainFrame.h"
#include "Controller.h"
#include "MainFrameHandler.h"

class CreateMachineDialogHandler : public CreateMachineDialog
{
public:
	CreateMachineDialogHandler(wxWindow* parent, wxWindowID id, const wxString& title);

protected:
	void OnCreateButtonClick(wxCommandEvent& event) wxOVERRIDE;
	void OnCancelButtonClick(wxCommandEvent& event) wxOVERRIDE;
};


#endif