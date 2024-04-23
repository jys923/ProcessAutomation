#pragma once


class CFD;

class CROI
{
public:
	CROI();
	~CROI();


public:
	CROI(const CROI& rhs);
	CROI& operator=(const CROI& rhs);

	int DeepCopy(const CROI& rhs);

public:
	int          nState;
	unsigned int x;
	unsigned int y;
	unsigned int w;
	unsigned int h;
	unsigned int cen_x;
	unsigned int cen_y;
	unsigned int cnt;

public:
	CFD * m_pFD;
};

