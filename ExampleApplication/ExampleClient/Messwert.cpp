#include "Messwert.h"


Messwert::Messwert(int timeStamp,float val1,float val2)
{
	this->value1 = val1;
	this->value2 = val2;
	this->timestamp = timeStamp;
}

void Messwert::printString(){
	printf("%d\t\t%f\t\t%f\r\n",timestamp,value1,value2);
}

void Messwert::printToFile(FILE* file){
	fprintf(file, "%d\t\t%f\t\t%f\n", timestamp, value1, value2);
}

Messwert::~Messwert()
{
}
