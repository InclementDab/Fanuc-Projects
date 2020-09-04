#pragma once

#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>

#include <wx/wx.h>
#include <30i/Fwlib32.h>
#include "MainFrame.h"

class App final : public wxApp
{
public:
	bool OnInit() override;
	int OnExit() override;

protected:
	MainFrame* mFrame = nullptr;
};


wxIMPLEMENT_APP(App);

