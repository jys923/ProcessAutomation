#include "pch.h"

#include "Result.h"

#include "GrayImg.h"

#include "IBinarization.h"
#include "GFTBinarization.h"
#include "Morph_Closing.h"
#include "ILabeling.h"
#include "LabelingRes.h"
#include "Labeling.h"

#include "Feature.h"
#include "FD.h"
#include "Blob.h"
#include "BlobDetector.h"

#include "IRecipe.h"
#include "Recipe.h"

#include "UAInspectorBasedBlob_TH.h"



CUAInspectorBasedBlob_TH::CUAInspectorBasedBlob_TH()
{

}


CUAInspectorBasedBlob_TH::~CUAInspectorBasedBlob_TH()
{

}

int CUAInspectorBasedBlob_TH::Do(const CGrayImg& rSrc, const cv::Mat& maskImg, const cv::Mat& maskFImg, CResult & rRes, const IRecipe* rRecipe, int mode)
{
	if (1 != rSrc.m_Img.channels()) return -11;
	CRecipe* pR = (CRecipe*)rRecipe;

	int ret = 0;

	int img_h = rSrc.m_Img.rows;
	int img_w = rSrc.m_Img.cols;
	int ch    = rSrc.m_Img.channels();
	unsigned int th = pR->GetTh();
	unsigned int r = pR->GetR();
	unsigned int rmarg = pR->GetRMarg();
	//std::cout << "CUAInspectorBasedBlob_TH : " << ch << std::endl;

	Mat tmp = Mat::zeros(img_h, img_w, CV_8UC1);
	ret = ProcessBinarization(rSrc.m_Img, tmp, maskImg, maskFImg, th);
	if (ret < 0) return -11;

	Mat tmp2 = Mat::zeros(img_h, img_w, CV_8UC1);
	{
		CMorph_Closing NR;
		int err = NR.Create(2, 2);
		if (err < 0) return -23;
		ret = NR.Do(tmp, tmp2);
	}
	if (ret < 0) return -22;


	Mat res = Mat::zeros(img_h, img_w, CV_8UC1);
	CBlobDetector * pBD = ProcessBlobDetection(tmp2, res, maskImg, r, rmarg);
	if (nullptr == pBD) return -55;


	MakeResult(rSrc.m_Img, tmp, pBD, rRes);
	int nResImgSize = img_w * img_h * 4;
	if (nullptr == rRes.m_pData)
	{
		rRes.m_uiDataSize = nResImgSize;
		rRes.m_pData = new unsigned char[nResImgSize];
	}
	memcpy(rRes.m_pData, (void*)pBD->m_BlobImg.ptr<uchar>(0), nResImgSize);


	delete pBD;
	pBD = nullptr;

	return 1;
}


int CUAInspectorBasedBlob_TH::ProcessBinarization(const Mat & Src, Mat & Res, const cv::Mat& maskImg, const cv::Mat& maskFImg, int nTh)
{
	IBinarization* pBin = new CGFTBinarization();

	map<string, double> params;
	params.insert(make_pair("th", nTh));

	pBin->Do(Src, Res, maskImg, maskFImg, params);

	delete pBin;

	return 1;
}

CBlobDetector * CUAInspectorBasedBlob_TH::ProcessBlobDetection(const Mat& Src, Mat& Res, const cv::Mat& maskImg, uint r, uint rmarg)
{
	CBlobDetector* pBD = nullptr;

	pBD = new CBlobDetector();

	map<string, double> params;
	params.insert(make_pair("min_area_size", 50));
	params.insert(make_pair("max_area_size", 600));
	params.insert(make_pair("r", r));
	params.insert(make_pair("rmarg", rmarg));
	pBD->Do(Src, params, maskImg);

	return pBD;
}


int CUAInspectorBasedBlob_TH::MakeResult(const Mat& rOrgImg, const Mat& rBinImg, CBlobDetector* pBD, CResult& rRes)
{
	std::cout << pBD->m_vBlobs.size() << std::endl;

	for (int i = 0; i < pBD->m_vBlobs.size(); i++)
	{
		CBlob& rBlob = pBD->m_vBlobs.at(i);
		//if (false == rBlob.m_bState) continue;

		CROI roi;
		if (false == rBlob.m_bState)
			roi.nState = -1;
		else
			roi.nState = 1;
		roi.x = rBlob.m_rectArea.x;
		roi.y = rBlob.m_rectArea.y;
		roi.w = rBlob.m_rectArea.width;
		roi.h = rBlob.m_rectArea.height;
		roi.cen_x = rBlob.m_ptCenter.x;
		roi.cen_y = rBlob.m_ptCenter.y;
		roi.cnt = rBlob.m_uiSize;

		// cal mean gray_org
		CFeature nf;
		int mean_gray = CalMean(rOrgImg, rBinImg, roi.cen_x, roi.cen_y, 3);

		nf.m_dVal = mean_gray;
		nf.m_sName = "mean_gray";

		roi.m_pFD->m_vFeature.push_back(nf);

		rRes.m_vROI.push_back(roi);
	}


	return 1;
}


int CUAInspectorBasedBlob_TH::CalMean(const cv::Mat& OrgImg, const cv::Mat& BinImg, const cv::Rect& rect, int nCnt)
{

	double dSum = 0;
	double dCnt = 0;
	for (int i = rect.y; i < (rect.y + rect.height); i++)
	{
		const unsigned char* pData = BinImg.ptr<uchar>(i);
		const unsigned char* pOrg = OrgImg.ptr<uchar>(i);

		for (int j = rect.x; j < (rect.x + rect.width); j++)
		{

			if (255 == pData[j])
			{
				dSum += pOrg[j];
				dCnt++;
			}

		}
	}

	if (dSum < 1 || dCnt == 0) return -1;
	else
	{
		return (int)(dSum / dCnt);
	}
}

int CUAInspectorBasedBlob_TH::CalMean(const cv::Mat& OrgImg, const cv::Mat& BinImg, int x, int y, int winsize)
{

	double dSum = 0;
	double dCnt = 0;

	int marg = winsize / 2;
	int st_y = max(0, y - marg);
	int st_x = max(0, x - marg);
	int end_y = min(y + marg, OrgImg.rows - 1);
	int end_x = min(x + marg, OrgImg.cols - 1);


	for (int i = st_y; i <= end_y; i++)
	{
		const unsigned char* pData = BinImg.ptr<uchar>(i);
		const unsigned char* pOrg = OrgImg.ptr<uchar>(i);

		for (int j = st_x; j <= end_x; j++)
		{

			if (255 == pData[j])
			{
				dSum += pOrg[j];
				dCnt++;
			}

		}
	}

	//std::cout << "st_x : " << st_x << endl;
	//std::cout << "end_x : " << end_x << endl;
	//std::cout << "st_y : " << st_y << endl;
	//std::cout << "end_y : " << end_y << endl;
	//std::cout << "cnt : " << dCnt << endl;


	if (dSum < 1 || dCnt == 0) return -1;
	else
	{
		return (int)(dSum / dCnt);
	}
}
