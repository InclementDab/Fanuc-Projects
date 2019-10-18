#pragma once

#ifndef _MAINFRAMEHANDLER_H_
#define _MAINFRAMEHANDLER_H_

#include <iostream>
#include <locale>
#include <codecvt>
#include <string>
#include <typeinfo>
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
	ObjectTree* TreeList = objectTree;

protected:
	void OnAddMachineButtonClick(wxCommandEvent& event) override;
	void OnAddFolderButtonClick(wxCommandEvent& event) override;
	
	wxDialog* CurrentDialog = nullptr;
};

#endif