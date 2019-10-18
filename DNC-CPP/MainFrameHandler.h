#pragma once

#ifndef _MAINFRAMEHANDLER_H_
#define _MAINFRAMEHANDLER_H_

#include <iostream>
#include <wx/wx.h>
#include "Machine.h"
#include "MainFrame.h"
#include "CreateMachineDialogHandler.h"
#include "resource.h"

// this class exists to override all the virtual events created by MainFrame
// it simply acts as a logical buffer between MainWindow and MainFrame
class MainFrameHandler : public MainFrame
{

public:
	MainFrameHandler();
	void AddMachine(Machine m);
	void AddFolder();


protected:
	void OnAddMachineButtonClick(wxCommandEvent& event) wxOVERRIDE;
	wxDialog* CurrentDialog = nullptr;
	
};

#endif