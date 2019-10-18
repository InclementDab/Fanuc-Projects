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

	objectTree = new ObjectTree( this, wxID_ANY, wxDefaultPosition, wxDefaultSize, 0 );
	objectTree->SetMinSize( wxSize( 300,450 ) );

	mSizer->Add( objectTree, wxGBPosition( 0, 0 ), wxGBSpan( 1, 1 ), wxALL, 5 );

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
	mAddFolderButton->Connect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( MainFrame::OnAddFolderButtonClick ), NULL, this );
	mAddMachineButton->Connect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( MainFrame::OnAddMachineButtonClick ), NULL, this );
}

MainFrame::~MainFrame()
{
	// Disconnect Events
	mAddFolderButton->Disconnect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( MainFrame::OnAddFolderButtonClick ), NULL, this );
	mAddMachineButton->Disconnect( wxEVT_COMMAND_BUTTON_CLICKED, wxCommandEventHandler( MainFrame::OnAddMachineButtonClick ), NULL, this );

}
