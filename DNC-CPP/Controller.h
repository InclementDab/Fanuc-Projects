#pragma once

#ifndef _CONTROLLER_H_
#define _CONTROLLER_H_

#include <iostream>
#include <wx/wx.h>
#include <string>
#include <vector>
#include <filesystem>

struct Controller
{
public:
	Controller(wxString name, wxString libFile);
	static std::vector<Controller> GetControllerList();

	wxString Name;
	wxString LibFile;
};

#endif

