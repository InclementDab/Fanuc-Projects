

#include "MainWindow.h"

MainWindow::MainWindow() : wxTopLevelWindow(nullptr, wxID_ANY, "DNC Application")
{
	m_FrameHandler = new MainFrameHandler(this);
	m_FrameHandler->Show();
}


