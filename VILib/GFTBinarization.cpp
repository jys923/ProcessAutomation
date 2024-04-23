#include "pch.h"

#include "GFTBinarization.h"


CGFTBinarization::CGFTBinarization()
{
	m_uiTh = 150;
}



CGFTBinarization::~CGFTBinarization()
{

}


int CGFTBinarization::Do(const cv::Mat& src, cv::Mat& res, const cv::Mat& maskImg, const cv::Mat& maskFImg, map<string, double>& rParam)
{
	if (0 == src.rows || 0 == src.cols) return -1;
	if (1 != src.channels())			return -2;

	int m_uiTh = 180;
	auto item = rParam.find("th");
	if (item != rParam.end())
	{
		m_uiTh = (unsigned int)item->second;
		cout << "find" << endl;
		cout << m_uiTh << endl;
	}
	else
	{
		return -3;
	}


	res = cv::Scalar(0);
	unsigned char* pRes			= res.ptr<uchar>(0);
	const unsigned char* pSrc	= src.ptr<uchar>(0);
	const unsigned char* pMask	= maskImg.ptr<uchar>(0);
	const unsigned char* pFMask = maskFImg.ptr<uchar>(0);
	int nTotSize = src.rows * src.cols;
	for (int i = 0; i < nTotSize; i++)
	{
		if (pFMask[i] != 255 && pMask[i] != 255)
		{
			pRes[i] = 0;
			continue;
		}

		
		if (pSrc[i] > m_uiTh)
			pRes[i] = 255;
	}

	m_Img = Mat::zeros(src.rows, src.cols, CV_8UC1);
	res.copyTo(m_Img);

	return 1;
}