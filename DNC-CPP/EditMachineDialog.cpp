///////////////////////////////////////////////////////////////////////////
// C++ code generated with wxFormBuilder (version Oct 26 2018)
// http://www.wxformbuilder.org/
//
// PLEASE DO *NOT* EDIT THIS FILE!
///////////////////////////////////////////////////////////////////////////

#include "EditMachineDialog.h"

///////////////////////////////////////////////////////////////////////////

EditMachineDialog::EditMachineDialog( wxWindow* parent, wxWindowID id, const wxString& title, const wxPoint& pos, const wxSize& size, long style ) : wxDialog( parent, id, title, pos, size, style )
{
	this->SetSizeHints( wxSize( -1,-1 ), wxSize( -1,-1 ) );

	wxWrapSizer* wSizer2;
	wSizer2 = new wxWrapSizer( wxVERTICAL, wxWRAPSIZER_DEFAULT_FLAGS );

	wxFlexGridSizer* fgSizer2;
	fgSizer2 = new wxFlexGridSizer( 0, 2, 0, 0 );
	fgSizer2->SetFlexibleDirection( wxBOTH );
	fgSizer2->SetNonFlexibleGrowMode( wxFLEX_GROWMODE_SPECIFIED );

	listCtrl = new ListCtrl( this, wxID_ANY, wxDefaultPosition, wxSize( 250,-1 ), 0 );
	listCtrl->SetMinSize( wxSize( 150,-1 ) );

	fgSizer2->Add( listCtrl, 0, wxALL|wxEXPAND, 5 );

	optionHost = new wxPanel( this, wxID_ANY, wxDefaultPosition, wxSize( 350,300 ), wxTAB_TRAVERSAL );
	fgSizer2->Add( optionHost, 1, wxALL|wxEXPAND, 5 );


	wSizer2->Add( fgSizer2, 1, wxEXPAND, 5 );

	wxGridSizer* gSizer8;
	gSizer8 = new wxGridSizer( 0, 2, 0, 0 );

	helpButton = new wxStdDialogButtonSizer();
	helpButtonHelp = new wxButton( this, wxID_HELP );
	helpButton->AddButton( helpButtonHelp );
	helpButton->Realize();

	gSizer8->Add( helpButton, 1, wxALIGN_LEFT, 5 );

	okCancelButtons = new wxStdDialogButtonSizer();
	okCancelButtonsOK = new wxButton( this, wxID_OK );
	okCancelButtons->AddButton( okCancelButtonsOK );
	okCancelButtonsCancel = new wxButton( this, wxID_CANCEL );
	okCancelButtons->AddButton( okCancelButtonsCancel );
	okCancelButtons->Realize();

	gSizer8->Add( okCancelButtons, 1, wxALIGN_RIGHT, 5 );


	wSizer2->Add( gSizer8, 1, wxEXPAND|wxLEFT|wxRIGHT, 10 );


	this->SetSizer( wSizer2 );
	this->Layout();

	this->Centre( wxBOTH );
}

EditMachineDialog::~EditMachineDialog()
{
}


conn_TypeTCP::conn_TypeTCP(wxWindow* parent, wxWindowID id, const wxPoint& pos, const wxSize& size, long style, const wxString& name) : wxPanel(parent, id, pos, size, style, name)
{
	wxWrapSizer* wSizer7;
	wSizer7 = new wxWrapSizer(wxVERTICAL, wxWRAPSIZER_DEFAULT_FLAGS);

	wxWrapSizer* wSizer3;
	wSizer3 = new wxWrapSizer(wxHORIZONTAL, wxWRAPSIZER_DEFAULT_FLAGS);

	wSizer3->SetMinSize(wxSize(225, 75));
	m_staticText6 = new wxStaticText(this, wxID_ANY, wxT("Host Name (or IP Address)"), wxDefaultPosition, wxSize(-1, -1), 0);
	m_staticText6->Wrap(-1);
	wSizer3->Add(m_staticText6, 0, 0, 5);

	ipAddressBox = new wxTextCtrl(this, wxID_ANY, wxEmptyString, wxDefaultPosition, wxSize(200, -1), 0);
#ifdef __WXGTK__
	if (!ipAddressBox->HasFlag(wxTE_MULTILINE))
	{
		ipAddressBox->SetMaxLength(16);
	}
#else
	ipAddressBox->SetMaxLength(16);
#endif
	wSizer3->Add(ipAddressBox, 0, 0, 5);


	wSizer7->Add(wSizer3, 1, wxALL, 5);

	wxWrapSizer* wSizer31;
	wSizer31 = new wxWrapSizer(wxHORIZONTAL, wxWRAPSIZER_DEFAULT_FLAGS);

	wSizer31->SetMinSize(wxSize(100, -1));
	m_staticText61 = new wxStaticText(this, wxID_ANY, wxT("Port"), wxDefaultPosition, wxSize(-1, -1), 0);
	m_staticText61->Wrap(-1);
	wSizer31->Add(m_staticText61, 0, 0, 5);

	portBox = new wxTextCtrl(this, wxID_ANY, wxEmptyString, wxDefaultPosition, wxSize(65, -1), 0);
#ifdef __WXGTK__
	if (!portBox->HasFlag(wxTE_MULTILINE))
	{
		portBox->SetMaxLength(6);
	}
#else
	portBox->SetMaxLength(6);
#endif
	wSizer31->Add(portBox, 0, 0, 5);


	wSizer7->Add(wSizer31, 1, wxALL, 5);


	this->SetSizer(wSizer7);
	this->Layout();
}

conn_TypeTCP::~conn_TypeTCP()
{
}

conn_TypeSerial::conn_TypeSerial( wxWindow* parent, wxWindowID id, const wxPoint& pos, const wxSize& size, long style, const wxString& name ) : wxPanel( parent, id, pos, size, style, name )
{
	wxWrapSizer* wSizer7;
	wSizer7 = new wxWrapSizer( wxVERTICAL, wxWRAPSIZER_DEFAULT_FLAGS );

	wxWrapSizer* wSizer3;
	wSizer3 = new wxWrapSizer( wxVERTICAL, wxWRAPSIZER_DEFAULT_FLAGS );

	wSizer3->SetMinSize( wxSize( 200,75 ) );
	m_staticText6 = new wxStaticText( this, wxID_ANY, wxT("Serial Port"), wxDefaultPosition, wxSize( -1,-1 ), 0 );
	m_staticText6->Wrap( -1 );
	wSizer3->Add( m_staticText6, 0, 0, 5 );

	m_comboBox1 = new wxComboBox( this, wxID_ANY, wxT("Com1"), wxDefaultPosition, wxSize( 175,-1 ), 0, NULL, 0 );
	wSizer3->Add( m_comboBox1, 0, 0, 5 );


	wSizer7->Add( wSizer3, 1, wxALL|wxEXPAND, 5 );

	wxWrapSizer* wSizer31;
	wSizer31 = new wxWrapSizer( wxHORIZONTAL, wxWRAPSIZER_DEFAULT_FLAGS );

	wSizer31->SetMinSize( wxSize( 100,-1 ) );
	m_staticText61 = new wxStaticText( this, wxID_ANY, wxT("Speed"), wxDefaultPosition, wxSize( -1,-1 ), 0 );
	m_staticText61->Wrap( -1 );
	wSizer31->Add( m_staticText61, 0, 0, 5 );

	baudBox = new wxTextCtrl( this, wxID_ANY, wxEmptyString, wxDefaultPosition, wxSize( 65,-1 ), 0 );
	#ifdef __WXGTK__
	if ( !baudBox->HasFlag( wxTE_MULTILINE ) )
	{
	baudBox->SetMaxLength( 6 );
	}
	#else
	baudBox->SetMaxLength( 6 );
	#endif
	wSizer31->Add( baudBox, 0, 0, 5 );


	wSizer7->Add( wSizer31, 1, wxALL, 5 );


	this->SetSizer( wSizer7 );
	this->Layout();
}

conn_TypeSerial::~conn_TypeSerial()
{
}
