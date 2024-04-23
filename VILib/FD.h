#pragma once

#include <vector>
#include <map>
#include <string>

class CFeature;
class CFD
{

public:
	CFD();
	~CFD();


public:
	int Insert(std::string skey, double dVal);

public:
	std::vector<CFeature> m_vFeature;
};

