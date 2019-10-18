
#include "Controller.h"

Controller::Controller(wxString name, wxString libFile)
{
	Name = name;
	LibFile = (wxString)std::filesystem::path((std::string)libFile).append(L"Fwlib32.h");
}

std::vector<Controller> Controller::GetControllerList()
{
	return std::vector<Controller>
	{
		Controller("Fanuc 0i", "0i"),
			Controller("Fanuc 30i", "30i")
	};
}