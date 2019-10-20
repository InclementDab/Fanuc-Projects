#include "App.h"

bool App::OnInit()
{
	if (!wxApp::OnInit())
		return false;

	mFrame = new MainFrame(nullptr);
	mFrame->Show();
	
	return true;
}

