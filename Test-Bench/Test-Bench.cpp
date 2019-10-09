// Test-Bench.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Fwlib32.h>
#include <FwSymbol.h>
#include <FCA32.H>

int main()
{
	unsigned short h;
	short ret;
	ODBST info;
	ser_t* cominfo = nullptr;

	
	//ret = cnc_allclibhndl3("192.168.128.63", 8193, 10, &h);
	/*cominfo->baud = 9600;
	cominfo->data_bit = 8;
	cominfo->parity = 0;
	cominfo->stop_bit = 1;
	cominfo->hardflow = 0;*/

	ret = rs_open(1, nullptr, (char*)"rw");

	printf("RS (%d)\n", ret);

	return 0;

	if (!ret)
	{
		cnc_statinfo(h, &info);

		IN_DSFILE in_file;
		OUT_DSFILE out_file;
		OUT_DSINFO out_info;
		char* intype = (char*)"DATA_SV";
		cnc_rddsfile(h, intype, &in_file, &out_info, &out_file);
		

		cnc_freelibhndl(h);
	}
	else
	{
		printf("ERROR!(%d)\n", ret);
	}
	

	
	
}