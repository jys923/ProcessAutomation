#pragma once

using namespace cv;
using namespace std;


class CBlob
{

public:
	bool			m_bState;
	unsigned int	m_uiSize;
	cv::Rect		m_rectArea;
	cv::Point		m_ptCenter;

	vector<cv::Point> m_vPts;
	
};

