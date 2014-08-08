#include "stdafx.h"
#include "Connection.h"
#include "Messwert.h"
#include <Windows.h>
#include <list>

typedef std::list<Messwert*> MesswertListe;


MesswertListe liste;
int i = 0;


int __cdecl main(int argc, char *argv[])
{
	Sleep(500);//wait for the server to be available
	
	Connection* con;
	int number = 100;
	if (argc == 2){ //if there is a commandline argument, we should use that as number of measurements
		number = atoi(argv[1]);
	}
	printf("Measuring %d values", number);
	try{
		con = new Connection("127.0.0.1", "1234");
		printf("Successfully connected!\r\nFetching Values!\r\n");
	}
	catch (int n){
		printf("error %d occured", n);
		return n;
	}

	char* sendbuffer = (char*) calloc(100, 1); //create a buffer to request values from the device
	sprintf(sendbuffer, "measure %d\n", number); // write the "number" variable into the buffer so the server doesn't send too many values
	con->Send(sendbuffer, strlen(sendbuffer)); // send the buffer to the server
	free(sendbuffer); //free the buffer to avoid memory leaks

	char* recvbuf = (char*)calloc(1,12); //get the buffer to receive 12 bytes (1 int and 2 floats)
	while (i <= number){
		con->Receive(recvbuf, 12);
		int timestamp = *((int*) recvbuf);
		float val1 = *((float*) recvbuf + 1);
		float val2 = *((float*) recvbuf + 2);
		auto value = new Messwert(timestamp, val1, val2);
		value->printString();
		liste.push_back(value);
		i++;
	}
	free(recvbuf);
	delete con;

	//save to file now
	auto file = fopen("./output.txt", "w");
	for (auto i = liste.begin(); i != liste.end(); ++i){
		(*i)->printToFile(file);
	}
	int error = fclose(file);
	if (error){
		printf("Failed to save file!");
	}
	
	return 0;
}

