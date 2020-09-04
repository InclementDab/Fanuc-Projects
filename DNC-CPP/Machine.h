#pragma once
#ifndef _MACHINE_H_
#define _MACHINE_H_

#include <wx/wx.h>
#include <wx/treectrl.h>
#include <30i/Fwlib32.h>
#include "Controller.h"

class ModelBase
{
	public:
		explicit ModelBase(const wxString& name = wxEmptyString) { Name = name; }
		virtual ~ModelBase();
		virtual wxMenu* GetContextMenu() { return new wxMenu(); }
		virtual int GetIcon() { return -1; }
		wxString Name;

	
};

class Machine : public ModelBase
{
	public:
		explicit Machine(const wxString& name, const Controller& mcontrol) : ModelBase(name)  { mController = mcontrol; }
		wxMenu* GetContextMenu() override;
		int GetIcon() override { return Icon; }


	protected:
		Controller mController;

	private:
		int Icon = 1;
};


class Folder : public ModelBase
{
	public:
		Folder(const wxString& name) : ModelBase(name) {}
		wxMenu* GetContextMenu() override;
		int GetIcon() override { return Icon; }

	private:
		int Icon = 0;
};

class ModelTreeItem : public wxTreeItemId
{
	public:
		ModelBase* Model;
		explicit ModelTreeItem(ModelBase* mBase) { Model = mBase; }
		~ModelTreeItem();
};



#endif _MACHINE_H_

