// Test-Bench.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Fwlib32.h>
#include <FwSymbol.h>


int main()
{
	unsigned short h;
	short ret;
	ODBST info;
	ret = cnc_allclibhndl3("192.168.128.63", 8193, 10, &h);

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