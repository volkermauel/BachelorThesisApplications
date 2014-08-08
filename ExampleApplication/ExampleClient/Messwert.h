#pragma once
#include "stdafx.h"
#include "Connection.h"
#include <Windows.h>
class Messwert
{
private:
	int timestamp;
	float value1;
	float value2;
public:
	Messwert(int timestamp,float val1,float val2);
	void Messwert::printString();
	void Messwert::printToFile(FILE* file);
	~Messwert();
};

