#include "pch.h"
#include "GrayImg.h"



CGrayImg::CGrayImg()
{
	m_bCreated = false;
}


CGrayImg::~CGrayImg()
{
	 
}


void CGrayImg::DeepCopy(const CGrayImg& rhs)
{
	m_Img = rhs.m_Img.clone();
}

CGrayImg::CGrayImg(const CGrayImg& rhs)
{
	DeepCopy(rhs);
}


CGrayImg& CGrayImg::operator= (const CGrayImg& rhs)
{
	if (this == &rhs) return *this;

	DeepCopy(rhs);

	return *this;
}

int CGrayImg::Create(unsigned char* source_pin_c, int img_w, int img_h, int ch)
{
	if (m_bCreated) return -1;

	if (1 == ch || 3 == ch || 4 == ch)
	{
		
	}
	else
	{
		return -2;
	}

	if (1 == ch)
	{
		m_Img = Mat::zeros(img_h, img_w, CV_8UC1);
		unsigned char* pSrc = m_Img.ptr<uchar>(0);

		int nTotSize = (img_h * img_w);
		memcpy(pSrc, source_pin_c, nTotSize);

	}
	else if (3 == ch)
	{
		//std::cout << ch << std::endl;

		Mat tmp = Mat::zeros(img_h, img_w, CV_8UC3);
		unsigned char* pTmp = tmp.ptr<uchar>(0);
		int nTotSize = (img_h * img_w * ch);
		memcpy(pTmp, source_pin_c, nTotSize);
		cvtColor(tmp, m_Img, COLOR_BGR2GRAY);
	}
	else if(4 == ch)
	{
		//std::cout << ch << std::endl;

		Mat tmp = Mat::zeros(img_h, img_w, CV_8UC4);
		unsigned char* pTmp = tmp.ptr<uchar>(0);
		int nTotSize = (img_h * img_w * ch);
		memcpy(pTmp, source_pin_c, nTotSize);
		cvtColor(tmp, m_Img, COLOR_BGRA2GRAY);
	}
	else
	{
		return -4;
	}

	
	return 1;
}