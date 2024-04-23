
#include "pch.h"

#include "LabelingRes.h"

#include "Blob.h"

#include "Labeling.h"

#include "BlobDetector.h"




CBlobDetector::CBlobDetector()
{
	m_pLabeler = NULL;
}

CBlobDetector::~CBlobDetector(void)
{
	if (NULL != m_pLabeler)
	{
		delete m_pLabeler;
		m_pLabeler = NULL;
	}
}

int CBlobDetector::Do( const cv::Mat & src, map<string, double>& rParam, const cv::Mat& maskImg)
{
	if (0 == src.rows || 0 == src.cols) return -1;
	
	auto param1 = rParam.find("min_area_size");
	if (param1 == rParam.end())
	{
		return -2;
	}
	int nMinAreaSize = (int)param1->second;

	auto param2 = rParam.find("max_area_size");
	if (param2 == rParam.end())
	{
		return -3;
	}
	int nMaxAreaSize = (int)param2->second;

	auto param3 = rParam.find("r");
	if (param3 == rParam.end())
	{
		return -4;
	}
	int R = (int)param3->second;

	auto param4 = rParam.find("rmarg");
	if (param4 == rParam.end())
	{
		return -4;
	}
	int RMarg = (int)param4->second;


	if (NULL != m_pLabeler)
	{
		delete m_pLabeler;
		m_pLabeler = NULL;
	}
	m_pLabeler = new CLabeling();
	map<string, string> param;
	param.insert(make_pair("connectivity", "8"));
	cv::Mat res;
	m_pLabeler->Do(src, res, param);

	//-------------------------------------------------------------------------
	// make result
	//-------------------------------------------------------------------------
	// image format : BGRA
	//-------------------------------------------------------------------------
	m_vBlobs.clear();
	Mat dst;
	cvtColor(src, dst, COLOR_GRAY2BGRA);

	const CLabelingRes& resList = m_pLabeler->GetResult();
	for (unsigned int i = 0; i < resList.m_vSize.size(); i++)
	{
		int nSize = resList.m_vSize.at(i);
		if (nSize < nMinAreaSize || nSize > nMaxAreaSize)
			continue;

		CBlob blob;
		blob.m_bState		= true;
		blob.m_uiSize		= resList.m_vSize.at(i);
		blob.m_rectArea		= resList.m_vArea.at(i);
		blob.m_ptCenter		= resList.m_vCenter.at(i);
		m_vBlobs.push_back(blob);
	}


	//-------------------------------------------------------------------------
	// check overlapped Blobs
	//-------------------------------------------------------------------------
	for (int ii = 0; ii < m_vBlobs.size(); ii++)
	{
		if (false == m_vBlobs.at(ii).m_bState) continue;

		for (int jj = 0; jj < m_vBlobs.size(); jj++)
		{
			if (ii == jj) continue;
			if (false == m_vBlobs.at(jj).m_bState) continue;

			int diff_x = abs(m_vBlobs.at(ii).m_ptCenter.x - m_vBlobs.at(jj).m_ptCenter.x);
			int diff_y = abs(m_vBlobs.at(ii).m_ptCenter.y - m_vBlobs.at(jj).m_ptCenter.y);
			if ((diff_x < 20) && (diff_y < 20))
			{
				m_vBlobs.at(ii).m_bState = false;
				m_vBlobs.at(jj).m_bState = false;

				std::cout << ii << " , " << jj << ", " << diff_x << " , " << diff_y << std::endl;			
			}
		}
	}

	//-------------------------------------------------------------------------
	// check invalid regions
	//-------------------------------------------------------------------------
	for (unsigned int i = 0; i < m_vBlobs.size(); i++)
	{
		CBlob& blob = m_vBlobs.at(i);

		if (false == blob.m_bState) continue;
	
		uchar val = maskImg.at<uchar>(blob.m_ptCenter.y, blob.m_ptCenter.x);
		if (val != 255)
			blob.m_bState = false;
	}


	int r_cen = src.rows / 2;
	int c_cen = src.cols / 2;

	//-------------------------------------------------------------------------
	// draw ROI regions
	//-------------------------------------------------------------------------
	for (unsigned int i = 0; i < m_vBlobs.size(); i++)
	{
		const CBlob & blob = m_vBlobs.at(i);

		if (false == blob.m_bState) continue;

		cv::rectangle(dst, blob.m_rectArea, Scalar(0, 255, 255, 255));
		cv::circle(dst, blob.m_ptCenter, 2, Scalar(0, 0, 255, 255), 1 );
		cv::line(dst, cv::Point(c_cen, r_cen), blob.m_ptCenter, Scalar(0, 0, 255, 255), 2);
	}


	//-------------------------------------------------------------------------
	// draw inspeciton area
	//-------------------------------------------------------------------------
	int InnerR = R - (RMarg/2);
	int OuterR = R + (RMarg/2);
	cv::circle(dst, cv::Point(c_cen, r_cen), InnerR, Scalar(0, 255, 0, 255), 1);
	cv::circle(dst, cv::Point(c_cen, r_cen), OuterR, Scalar(0, 255, 0, 255), 1);


	//-------------------------------------------------------------------------
	// copy result image
	//-------------------------------------------------------------------------
	m_BlobImg = Mat::zeros(src.rows, src.cols, CV_8UC4);
	dst.copyTo(m_BlobImg);

	return 1;
}
