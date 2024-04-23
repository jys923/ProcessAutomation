#include "pch.h"

#include "Feature.h"

#include "FD.h"

using namespace std;

CFD::CFD()
{

}


CFD::~CFD()
{
	m_vFeature.clear();
}

int CFD::Insert(string skey, double dVal)
{
	CFeature feature;
	feature.m_sName = skey;
	feature.m_dVal = dVal;

	m_vFeature.push_back(feature);

	return 1;
}
