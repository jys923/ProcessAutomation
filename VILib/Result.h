#pragma once


#include "ROI.h"

class CResult
{
public:
	CResult()
	{
		m_pData = NULL;
		m_uiDataSize = 0;
	}

	~CResult()
	{
		if (nullptr != m_pData)
		{
			delete[]m_pData;
			m_pData = nullptr;
		}
	}

public:
	std::vector<CROI> m_vROI;
	unsigned int m_uiDataSize;
	unsigned char* m_pData;
	
};

