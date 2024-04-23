#pragma once

//#include "opencv2/opencv.hpp"
// 
using namespace cv;
//using namespace std;

class CGrayImg
{
public:
	CGrayImg();
	~CGrayImg();

	int Create(unsigned char* pImg, int w, int h, int ch);
public:
	CGrayImg(const CGrayImg& rhs);
	CGrayImg& operator= (const CGrayImg& rhs);
	void DeepCopy(const CGrayImg& rhs);

public:
	bool m_bCreated;
	cv::Mat m_Img;

};

