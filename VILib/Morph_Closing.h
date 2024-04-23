#pragma once


using namespace cv;

class CMorph_Closing
{
public:
	CMorph_Closing();
	~CMorph_Closing();

public:
	int Create(unsigned int type, unsigned int size);
	int Do(const cv::Mat& src, cv::Mat& res);


private:
	bool m_bCreated;
	unsigned int m_uiType;
	unsigned int m_uiSize;

	cv::Mat m_DilationElement;
	cv::Mat m_ErosionElement;
};

