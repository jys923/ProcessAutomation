#pragma once


using namespace cv;
using namespace std;

class CLabelingRes
{
public:
	CLabelingRes()
	{

	}

	~CLabelingRes()
	{

	}

public:
	vector<int>			m_vSize;
	vector<cv::Rect>	m_vArea;
	vector<cv::Point>	m_vCenter;
	vector<bool>		m_vValid;
};

