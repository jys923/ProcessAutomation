#include "pch.h"

#include <vector>
#include <map>
#include <string>

#include "FD.h"

#include "Feature.h"

#include "ROI.h"



CROI::CROI() {

	m_pFD = NULL;
	nState = 0;
	m_pFD = new CFD();
}

CROI::~CROI()
{
	if (NULL != m_pFD)
	{
		delete m_pFD;
		m_pFD = NULL;
	}
}


int CROI::DeepCopy(const CROI& rhs)
{
	nState = rhs.nState;
	x = rhs.x;
	y = rhs.y;
	w = rhs.w;
	h = rhs.h;
	cen_x = rhs.cen_x;
	cen_y = rhs.cen_y;
	cnt = rhs.cnt;

	m_pFD = NULL;
	m_pFD = new CFD();

	m_pFD->m_vFeature = rhs.m_pFD->m_vFeature;

	return 1;
}

CROI::CROI(const CROI& rhs)
{
	if (NULL == rhs.m_pFD) return;
	DeepCopy(rhs);
}


CROI& CROI::operator=(const CROI& rhs)
{
	//-------------------------------------------------------------------------
	// 1. 재참조 방지
	//-------------------------------------------------------------------------
	if (this == &rhs) return *this;

	if (NULL == rhs.m_pFD) return *this; 

	DeepCopy(rhs);

	return *this;
}