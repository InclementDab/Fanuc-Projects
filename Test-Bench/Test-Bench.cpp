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
		if (info.alarm > 0)
		{
			long alm;
			cnc_alarm2(h, &alm);
			if (alm > 0)
				printf("Alarm: (%d)\n", alm);


		}

		cnc_freelibhndl(h);
	}
	else
	{
		printf("ERROR!(%d)\n", ret);
	}
	

	
	
}