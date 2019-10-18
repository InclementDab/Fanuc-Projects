#pragma once
#include "mWindow.h"
class mWindowContainer : public mWindow
{
public:
	mWindowContainer();
	~mWindowContainer();

	int OnAddMachineButtonClick()
	{
		mWindow::OnAddMachineButtonClick();
	}
};

