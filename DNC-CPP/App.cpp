#include "App.h"
#include "resource.h"


bool App::OnInit()
{
	if (!wxApp::OnInit())
		return false;


	m_FrameHandler = new MainFrameHandler();	
	m_FrameHandler->Show();
	return true;
}

