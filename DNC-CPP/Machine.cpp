
#include "Machine.h"

ModelBase::ModelBase(wxString name) : wxTreeItemData()
{
	Name = name;
}

Machine::Machine(wxString name, Controller* mcontrol) : ModelBase(name)
{
	mController = mcontrol;
}


Folder::Folder(wxString name) : ModelBase(name)
{

}