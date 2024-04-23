#include "pch.h"

#include "LabelingRes.h"

#include "Labeling.h"



CLabeling::CLabeling()
{

}


CLabeling::~CLabeling()
{

}


int CLabeling::labeling(const Mat & bin, Mat & dst)
{

	Mat labels, stats, centroids;
	int cnt = connectedComponentsWithStats(bin, labels, stats, centroids);

	for (int i = 1; i < cnt; i++) 
	{
		int* pstats  = stats.ptr<int>(i);
		double* pcen = centroids.ptr<double>(i);

		m_Res.m_vArea.push_back(Rect(pstats[0], pstats[1], pstats[2], pstats[3]));
		m_Res.m_vSize.push_back(pstats[4]);

		cv::Point pt;
		pt.y = (int)pcen[1];
		pt.x = (int)pcen[0];
		m_Res.m_vCenter.push_back(pt);

		m_Res.m_vValid.push_back(true);
	}

	return 1;
}


int CLabeling::Do(const cv::Mat& src, cv::Mat & res, map<string, string>& rParam)
{
	
	return labeling(src, res);
}