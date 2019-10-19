#pragma once

#include <wx/wx.h>
#include "Machine.h"
#include "EditMachineDialog.h"

class EditMachineDialogHandler : public EditMachineDialog
{

	protected:
		Machine* CurrentMachine = nullptr;

	public:
		EditMachineDialogHandler(Machine* machine, wxWindow* parent, wxWindowID id, const wxString& title);



};




