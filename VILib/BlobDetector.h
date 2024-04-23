#pragma once

//#include "opencv2/opencv.hpp"
using namespace cv;
using namespace std;


class ILabeling;
class CBlob;

class CBlobDetectorParams
{
 
public:

	CBlobDetectorParams()
	{
		m_nImgH = 320;
		m_nImgW = 320;
		m_nMinAreaSize = 0;
		m_nMaxAreaSize = 0;
	}

	int m_nImgH;
	int m_nImgW;

	int m_nMinAreaSize;
	int m_nMaxAreaSize;
};

class  CBlobDetector 
{
public:
	CBlobDetector(  );
	~CBlobDetector(void);

public:
	int	Do(const cv::Mat& bw_img, map<string, double>& rParam, const cv::Mat& maskImg);

	
public:
	vector<CBlob> m_vBlobs;
	cv::Mat m_BlobImg;

private:
	ILabeling* m_pLabeler;


};