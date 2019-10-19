
#include "ListCtrlItem.h"


//////////////////////////////////////////////

listCtrlItem_General::listCtrlItem_General(wxWindow* parent, wxWindowID id, const wxPoint& pos, const wxSize& size, long style, const wxString& name) : wxPanel(parent, id, pos, size, style, name)
{
	wxGridSizer* generalGrid;
	generalGrid = new wxGridSizer(0, 1, 0, 0);

	wxStaticBoxSizer* nameContainer;
	nameContainer = new wxStaticBoxSizer(new wxStaticBox(this, wxID_ANY, wxT("General Machine Settings")), wxVERTICAL);

	m_staticName = new wxStaticText(nameContainer->GetStaticBox(), wxID_ANY, wxT("Machine Name"), wxDefaultPosition, wxDefaultSize, 0);
	m_staticName->Wrap(-1);
	nameContainer->Add(m_staticName, 0, wxALL, 5);

	nameTextBox = new wxTextCtrl(nameContainer->GetStaticBox(), wxID_ANY, wxEmptyString, wxDefaultPosition, wxDefaultSize, 0);
#ifdef __WXGTK__
	if (!nameTextBox->HasFlag(wxTE_MULTILINE))
	{
		nameTextBox->SetMaxLength(20);
	}
#else
	nameTextBox->SetMaxLength(20);
#endif
	nameTextBox->SetMinSize(wxSize(175, -1));

	nameContainer->Add(nameTextBox, 0, wxALIGN_LEFT | wxLEFT, 5);

	m_staticDesc = new wxStaticText(nameContainer->GetStaticBox(), wxID_ANY, wxT("Description"), wxDefaultPosition, wxDefaultSize, 0);
	m_staticDesc->Wrap(-1);
	nameContainer->Add(m_staticDesc, 0, wxALL, 5);

	descTextBox = new wxTextCtrl(nameContainer->GetStaticBox(), wxID_ANY, wxEmptyString, wxDefaultPosition, wxDefaultSize, 0);
#ifdef __WXGTK__
	if (!descTextBox->HasFlag(wxTE_MULTILINE))
	{
		descTextBox->SetMaxLength(20);
	}
#else
	descTextBox->SetMaxLength(20);
#endif
	descTextBox->SetMinSize(wxSize(175, -1));

	nameContainer->Add(descTextBox, 0, wxALIGN_LEFT | wxLEFT, 5);


	generalGrid->Add(nameContainer, 1, wxEXPAND, 5);


	this->SetSizer(generalGrid);
	this->Layout();
}

listCtrlItem_General::~listCtrlItem_General()
{
}

//////////////////////////////////////////////

listCtrlItem_Connection::listCtrlItem_Connection(wxWindow* parent, wxWindowID id, const wxPoint& pos, const wxSize& size, long style, const wxString& name) : wxPanel(parent, id, pos, size, style, name)
{
	wxGridSizer* connectionGrid;
	connectionGrid = new wxGridSizer(0, 1, 0, 0);

	wxStaticBoxSizer* connContainer;
	connContainer = new wxStaticBoxSizer(new wxStaticBox(this, wxID_ANY, wxT("Connection Options")), wxVERTICAL);

	wxStaticBoxSizer* connType;
	connType = new wxStaticBoxSizer(new wxStaticBox(connContainer->GetStaticBox(), wxID_ANY, wxT("Type")), wxVERTICAL);

	wxFlexGridSizer* fgSizer3;
	fgSizer3 = new wxFlexGridSizer(2, 1, 0, 0);
	fgSizer3->SetFlexibleDirection(wxBOTH);
	fgSizer3->SetNonFlexibleGrowMode(wxFLEX_GROWMODE_SPECIFIED);

	connectionTypeHost = new wxPanel(connType->GetStaticBox(), wxID_ANY, wxDefaultPosition, wxSize(325, 75), wxTAB_TRAVERSAL);
	fgSizer3->Add(connectionTypeHost, 1, wxEXPAND | wxALL, 5);

	wxGridSizer* gSizer10;
	gSizer10 = new wxGridSizer(1, 3, 0, 0);

	tcpBtn = new wxRadioButton(connType->GetStaticBox(), wxID_ANY, wxT("TCP/IP"), wxDefaultPosition, wxDefaultSize, wxRB_GROUP);
	tcpBtn->SetValue(true);
	gSizer10->Add(tcpBtn, 0, wxALIGN_CENTER_HORIZONTAL, 5);

	serialBtn = new wxRadioButton(connType->GetStaticBox(), wxID_ANY, wxT("Serial"), wxDefaultPosition, wxDefaultSize, 0);
	gSizer10->Add(serialBtn, 0, wxALIGN_CENTER_HORIZONTAL, 5);

	sshBtn = new wxRadioButton(connType->GetStaticBox(), wxID_ANY, wxT("SSH"), wxDefaultPosition, wxDefaultSize, 0);
	sshBtn->Enable(false);

	gSizer10->Add(sshBtn, 0, wxALIGN_CENTER_HORIZONTAL, 5);


	fgSizer3->Add(gSizer10, 1, wxEXPAND, 5);


	connType->Add(fgSizer3, 1, wxEXPAND, 5);


	connContainer->Add(connType, 1, wxEXPAND, 5);

	testConnection = new wxButton(connContainer->GetStaticBox(), wxID_ANY, wxT("Test Connection"), wxDefaultPosition, wxDefaultSize, 0);
	connContainer->Add(testConnection, 0, wxALL, 5);

	conOnStart = new wxCheckBox(connContainer->GetStaticBox(), wxID_ANY, wxT("Connect on Startup"), wxDefaultPosition, wxDefaultSize, 0);
	connContainer->Add(conOnStart, 0, wxALL, 5);


	connectionGrid->Add(connContainer, 1, wxEXPAND, 5);


	this->SetSizer(connectionGrid);
	this->Layout();

	// Connect Events
	tcpBtn->Connect(wxEVT_COMMAND_RADIOBUTTON_SELECTED, wxCommandEventHandler(listCtrlItem_Connection::OnRadioButtonClick), NULL, this);
	serialBtn->Connect(wxEVT_COMMAND_RADIOBUTTON_SELECTED, wxCommandEventHandler(listCtrlItem_Connection::OnRadioButtonClick), NULL, this);
	sshBtn->Connect(wxEVT_COMMAND_RADIOBUTTON_SELECTED, wxCommandEventHandler(listCtrlItem_Connection::OnRadioButtonClick), NULL, this);
}

listCtrlItem_Connection::~listCtrlItem_Connection()
{
	// Disconnect Events
	tcpBtn->Disconnect(wxEVT_COMMAND_RADIOBUTTON_SELECTED, wxCommandEventHandler(listCtrlItem_Connection::OnRadioButtonClick), NULL, this);
	serialBtn->Disconnect(wxEVT_COMMAND_RADIOBUTTON_SELECTED, wxCommandEventHandler(listCtrlItem_Connection::OnRadioButtonClick), NULL, this);
	sshBtn->Disconnect(wxEVT_COMMAND_RADIOBUTTON_SELECTED, wxCommandEventHandler(listCtrlItem_Connection::OnRadioButtonClick), NULL, this);

}
