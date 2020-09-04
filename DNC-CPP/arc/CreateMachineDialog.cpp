///////////////////////////////////////////////////////////////////////////
// C++ code generated with wxFormBuilder (version Oct 26 2018)
// http://www.wxformbuilder.org/
//
// PLEASE DO *NOT* EDIT THIS FILE!
///////////////////////////////////////////////////////////////////////////

#include "CreateMachineDialog.h"

///////////////////////////////////////////////////////////////////////////

CreateMachineDialog::CreateMachineDialog( wxWindow* parent, wxWindowID id, const wxString& title, const wxPoint& pos, const wxSize& size, long style ) : wxDialog( parent, id, title, pos, size, style )
{
	this->SetSizeHints( wxSize( 250,300 ), wxDefaultSize );

	wxGridSizer* dSizer;
	dSizer = new wxGridSizer( 3, 1, 0, 0 );

	wxStaticBoxSizer* nameContainer;
	nameContainer = new wxStaticBoxSizer( new wxStaticBox( this, wxID_ANY, wxT("Name") ), wxHORIZONTAL );

	nameTextPanel = new wxPanel( nameContainer->GetStaticBox(), wxID_ANY, wxDefaultPosition, wxSize( -1,-1 ), wxTAB_TRAVERSAL );
	nameTextPanel->SetBackgroundColour( wxSystemSettings::GetColour( wxSYS_COLOUR_WINDOW ) );

	nameTextSizer = new wxBoxSizer( wxVERTICAL );

	nameTextBox = new wxTextCtrl( nameTextPanel, wxID_ANY, wxEmptyString, wxDefaultPosition, wxSize( 150,-1 ), 0 );
	nameTextBox->SetForegroundColour( wxSystemSettings::GetColour( wxSYS_COLOUR_CAPTIONTEXT ) );
	nameTextBox->SetBackgroundColour( wxSystemSettings::GetColour( wxSYS_COLOUR_WINDOW ) );

	nameTextSizer->Add( nameTextBox, 0, wxALL, 2 );


	nameTextPanel->SetSizer( nameTextSizer );
	nameTextPanel->Layout();
	nameTextSizer->Fit( nameTextPanel );
	nameContainer->Add( nameTextPanel, 1, wxALL|wxEXPAND, 8 );


	dSizer->Add( nameContainer, 1, wxALIGN_CENTER, 5 );

	wxStaticBoxSizer* ctrlContainer;
	ctrlContainer = new wxStaticBoxSizer( new wxStaticBox( this, wxID_ANY, wxT("Controller") ), wxVERTICAL );

	wxString mControllerTypeChoices[] = { wxT("Fanuc 16i"), wxT("Fanuc 30i") };
	int mControllerTypeNChoices = sizeof( mControllerTypeChoices ) / sizeof( wxString );
	mControllerType = new wxChoice( ctrlContainer->GetStaticBox(), wxID_ANY, wxDefaultPosition, wxSize( 150,-1 ), mControllerTypeNChoices, mControllerTypeChoices, wxCB_SORT );
	mControllerType->SetSelection( 0 );
	ctrlContainer->Add( mControllerType, 0, wxALL, 5 );


	dSizer->Add( ctrlContainer, 1, wxALIGN_CENTER, 5 );

	wxBoxSizer* buttonContainer;
	buttonContainer = new wxBoxSizer( wxHORIZONTAL );

	createButton = new wxButton( this, wxID_ANY, wxT("Create"), wxDefaultPosition, wxDefaultSize, 0 );

	createButton->SetDefault();
	buttonContainer->Add( createButton, 0, wxALL, 5 );

	cancelButton = new wxButton( this, wxID_ANY, wxT("Cancel"), wxDefaultPosition, wxDefaultSize, 0 );
	buttonContainer->Add( cancelButton, 0, wxALL, 5 );


	dSizer->Add( buttonContainer, 1, wxALIGN_CENTER|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5 );


	this->SetSizer( dSizer );
	this->Layout();
	dSizer->Fit( this );

	this->Centre( wxBOTH );

	// Connect Events
	createButton->Connect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( CreateMachineDialog::OnCreateButtonClick ), NULL, this );
	cancelButton->Connect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( CreateMachineDialog::OnCancelButtonClick ), NULL, this );
}

CreateMachineDialog::~CreateMachineDialog()
{
	// Disconnect Events
	createButton->Disconnect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( CreateMachineDialog::OnCreateButtonClick ), NULL, this );
	cancelButton->Disconnect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( CreateMachineDialog::OnCancelButtonClick ), NULL, this );

}
