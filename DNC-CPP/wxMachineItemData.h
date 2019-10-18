#pragma once

#include <wx/wx.h>
#include <wx/treectrl.h>
#include "Machine.h"

class wxMachineItemData : public wxTreeItemData
{
public:
	wxMachineItemData(Machine* m);
	Machine* machine;
};

