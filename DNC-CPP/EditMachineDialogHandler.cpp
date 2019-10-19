#include "EditMachineDialogHandler.h"


EditMachineDialogHandler::EditMachineDialogHandler(Machine* machine, wxWindow* parent, wxWindowID id, const wxString& title) : EditMachineDialog(parent, id, title)
{
	CurrentMachine = machine;
}
