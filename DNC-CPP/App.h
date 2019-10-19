#pragma once

#include <wx/wx.h>
#include <30i/Fwlib32.h>
#include "MainFrameHandler.h"

class App : public wxApp
{
public:
	virtual bool OnInit() wxOVERRIDE;

protected:
	MainFrameHandler* m_FrameHandler = nullptr;
};


wxIMPLEMENT_APP(App);

