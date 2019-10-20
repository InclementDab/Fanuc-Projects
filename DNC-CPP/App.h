#pragma once

#include <wx/wx.h>
#include <30i/Fwlib32.h>
#include "MainFrame.h"

class App : public wxApp
{
public:
	bool OnInit() wxOVERRIDE;

protected:
	MainFrame* mFrame = nullptr;
};


wxIMPLEMENT_APP(App);

