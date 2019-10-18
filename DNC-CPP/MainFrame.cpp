///////////////////////////////////////////////////////////////////////////
// C++ code generated with wxFormBuilder (version Oct 26 2018)
// http://www.wxformbuilder.org/
//
// PLEASE DO *NOT* EDIT THIS FILE!
///////////////////////////////////////////////////////////////////////////

#include "MainFrame.h"

///////////////////////////////////////////////////////////////////////////

MainFrame::MainFrame( wxWindow* parent, wxWindowID id, const wxString& title, const wxPoint& pos, const wxSize& size, long style ) : wxFrame( parent, id, title, pos, size, style )
{
	this->SetSizeHints( wxDefaultSize, wxDefaultSize );

	mMenuBar = new wxMenuBar( 0 );
	newMenu = new wxMenu();
	mMenuBar->Append( newMenu, wxT("New") );

	editMenu = new wxMenu();
	mMenuBar->Append( editMenu, wxT("Edit") );

	viewMenu = new wxMenu();
	mMenuBar->Append( viewMenu, wxT("View") );

	this->SetMenuBar( mMenuBar );

	wxGridBagSizer* mSizer;
	mSizer = new wxGridBagSizer( 0, 0 );
	mSizer->SetFlexibleDirection( wxBOTH );
	mSizer->SetNonFlexibleGrowMode( wxFLEX_GROWMODE_SPECIFIED );

	mMachineListView = new wxTreeCtrl( this, wxID_ANY, wxDefaultPosition, wxSize( -1,-1 ), wxTR_DEFAULT_STYLE|wxTR_EDIT_LABELS|wxTR_HIDE_ROOT|wxTR_NO_LINES );
	mMachineListView->SetMinSize( wxSize( 300,450 ) );

	mSizer->Add( mMachineListView, wxGBPosition( 0, 0 ), wxGBSpan( 1, 1 ), wxALL, 10 );

	wxGridSizer* mAddButtonContainer;
	mAddButtonContainer = new wxGridSizer( 0, 2, 0, 0 );

	mAddFolderButton = new wxButton( this, wxID_ANY, wxT("+Folder"), wxDefaultPosition, wxDefaultSize, 0 );
	mAddButtonContainer->Add( mAddFolderButton, 0, wxALL, 5 );

	mAddMachineButton = new wxButton( this, wxID_ANY, wxT("+Machine"), wxDefaultPosition, wxDefaultSize, 0 );
	mAddButtonContainer->Add( mAddMachineButton, 0, wxALL, 5 );


	mSizer->Add( mAddButtonContainer, wxGBPosition( 1, 0 ), wxGBSpan( 1, 1 ), wxALIGN_RIGHT|wxRIGHT, 10 );

	mProgramGrid = new wxGrid( this, wxID_ANY, wxDefaultPosition, wxSize( -1,-1 ), 0 );

	// Grid
	mProgramGrid->CreateGrid( 5, 3 );
	mProgramGrid->EnableEditing( true );
	mProgramGrid->EnableGridLines( true );
	mProgramGrid->EnableDragGridSize( false );
	mProgramGrid->SetMargins( 0, 0 );

	// Columns
	mProgramGrid->EnableDragColMove( false );
	mProgramGrid->EnableDragColSize( true );
	mProgramGrid->SetColLabelSize( 30 );
	mProgramGrid->SetColLabelAlignment( wxALIGN_CENTER, wxALIGN_BOTTOM );

	// Rows
	mProgramGrid->EnableDragRowSize( true );
	mProgramGrid->SetRowLabelSize( 80 );
	mProgramGrid->SetRowLabelAlignment( wxALIGN_CENTER, wxALIGN_CENTER );

	// Label Appearance

	// Cell Defaults
	mProgramGrid->SetDefaultCellAlignment( wxALIGN_LEFT, wxALIGN_TOP );
	mProgramGrid->SetMinSize( wxSize( 550,200 ) );

	mSizer->Add( mProgramGrid, wxGBPosition( 0, 1 ), wxGBSpan( 2, 1 ), wxALL|wxEXPAND|wxTOP, 10 );


	mSizer->AddGrowableCol( 1 );
	mSizer->AddGrowableRow( 0 );

	this->SetSizer( mSizer );
	this->Layout();
	mStatusBar = this->CreateStatusBar( 1, wxSTB_SIZEGRIP, wxID_ANY );

	this->Centre( wxBOTH );

	// Connect Events
	mMachineListView->Connect( wxEVT_COMMAND_TREE_END_LABEL_EDIT, wxTreeEventHandler( MainFrame::OnEndLabelEdit ), NULL, this );
	mAddFolderButton->Connect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( MainFrame::OnAddFolderButtonClick ), NULL, this );
	mAddMachineButton->Connect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( MainFrame::OnAddMachineButtonClick ), NULL, this );
}

MainFrame::~MainFrame()
{
	// Disconnect Events
	mMachineListView->Disconnect( wxEVT_COMMAND_TREE_END_LABEL_EDIT, wxTreeEventHandler( MainFrame::OnEndLabelEdit ), NULL, this );
	mAddFolderButton->Disconnect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( MainFrame::OnAddFolderButtonClick ), NULL, this );
	mAddMachineButton->Disconnect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( MainFrame::OnAddMachineButtonClick ), NULL, this );

}

EditMachineWindow::EditMachineWindow( wxWindow* parent, wxWindowID id, const wxString& title, const wxPoint& pos, const wxSize& size, long style ) : wxFrame( parent, id, title, pos, size, style )
{
	this->SetSizeHints( wxDefaultSize, wxDefaultSize );


	this->Centre( wxBOTH );
}

EditMachineWindow::~EditMachineWindow()
{
}

CreateMachineDialog::CreateMachineDialog( wxWindow* parent, wxWindowID id, const wxString& title, const wxPoint& pos, const wxSize& size, long style ) : wxDialog( parent, id, title, pos, size, style )
{
	this->SetSizeHints( wxDefaultSize, wxDefaultSize );

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
	buttonContainer->Add( createButton, 0, wxALL, 5 );

	cancelButton = new wxButton( this, wxID_ANY, wxT("Cancel"), wxDefaultPosition, wxDefaultSize, 0 );
	buttonContainer->Add( cancelButton, 0, wxALL, 5 );


	dSizer->Add( buttonContainer, 1, wxALIGN_CENTER|wxALIGN_CENTER_HORIZONTAL|wxALIGN_CENTER_VERTICAL, 5 );


	this->SetSizer( dSizer );
	this->Layout();
	dSizer->Fit( this );

	this->Centre( wxBOTH );

	// Connect Events
	this->Connect( wxEVT_CLOSE_WINDOW, wxCloseEventHandler( CreateMachineDialog::OnCreateMachineDialogClose ) );
	createButton->Connect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( CreateMachineDialog::OnCreateButtonClick ), NULL, this );
	cancelButton->Connect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( CreateMachineDialog::OnCancelButtonClick ), NULL, this );
}

CreateMachineDialog::~CreateMachineDialog()
{
	// Disconnect Events
	this->Disconnect( wxEVT_CLOSE_WINDOW, wxCloseEventHandler( CreateMachineDialog::OnCreateMachineDialogClose ) );
	createButton->Disconnect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( CreateMachineDialog::OnCreateButtonClick ), NULL, this );
	cancelButton->Disconnect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( CreateMachineDialog::OnCancelButtonClick ), NULL, this );

}
