#pragma once

//#include "IARDetector.h"




#include "opencv2/opencv.hpp"
using namespace cv;
using namespace std;

//class CEV_KeyframeMerger;
class CBlobInfo;
class  CBlobDetector;
class CEV_AR_C1_DetectorParam
{

public:
	CEV_AR_C1_DetectorParam()
	{
		RED_LOW = 90;

		RED_HI = 220;

		RG_DIFF_TH = 80;

		LUT_R_QS = 10;
		LUT_G_QS = 5;
		LUT_B_QS = 5;
	
		MIN_SIZE  = 10*10;
		LIM_WIDTH = 90;
		MAX_SIZE  = LIM_WIDTH*LIM_WIDTH;

		CAND_RG_MIN_SIZE = 30*30;

		LIM_CAND = 20;

		RG_DIST = 5;

		DR_MIN_SIZE = 10;
		RG_TH = DR_MIN_SIZE/2;

		RG_OV_TH = 0.65f;
		m_bDSMode = false;


		HUE_RED_LOW = 16;
		HUE_RED_HIGH = 156; //164
		SAT_RED_LOW = 100;

		LAB_LUMI_TH = 40; // red 기준 75 
		LAB_AB_DIFF_TH = 20;
		LAB_AB_DIFF_TH_ON_DR = 10;

		m_nCameraSensorType = 0;

	}

	~CEV_AR_C1_DetectorParam()
	{

	}

public:
	int RED_LOW;

	int RED_HI;

	int RG_DIFF_TH;

	int LUT_R_QS;
	int LUT_G_QS;
	int LUT_B_QS;

	int MIN_SIZE;
	int LIM_WIDTH;

	int MAX_SIZE;
	int CAND_RG_MIN_SIZE;

	int LIM_CAND;

	int RG_DIST;

	int DR_MIN_SIZE;

	int RG_TH;

	float RG_OV_TH;

	bool m_bDSMode;

	int HUE_RED_LOW;
	int HUE_RED_HIGH; //164
	int SAT_RED_LOW;

	int LAB_LUMI_TH; // red 기준 75 
	int LAB_AB_DIFF_TH;
	int LAB_AB_DIFF_TH_ON_DR;

	int	m_nCameraSensorType;


};


class CUAI
{
public:
	CUAI();
	virtual ~CUAI(void); 


	//inheritance
public:
	void setDebug(bool bSet)
	{
	}

	bool getDebug()
	{
		return false;
	}

	vector<CVL_Res> * getResult()
	{
		return m_pRes;
	}


	int getResultCnt()
	{
		return m_pRes->size();
	}

	int reset()
	{
		return clear();
	}


	int Create(CCIFrameInfo & rFrameInfo, const CEV_AlgConfig & Params, int nID);
	int Do(cv::Mat & src,  cv::Mat &, cv::Mat &, int frame_num);


private:
	int clear();

	int Preprocessing( cv::Mat & src );

	int checkReddishColor(cv::Mat & rgb, cv::Mat & hsv, cv::Mat & lab, int i, int j, int *** pLUT);
	int checkReddishColor(unsigned char * prgb, unsigned char * phsv, unsigned char * plab, int i, int *** pLUT);

	int doBinaryzation( cv::Mat & src, cv::Mat & hsv_img, cv::Mat & lab, cv::Mat & LabelingImg, int, int, unsigned char *, int ***);

	int findAbnormalRegion( CBlobInfo & blob, cv::Mat & hsv, cv::Mat & rgb, ::Mat & m_BinaryImg, unsigned char * pVR, int dist, CVL_Res & vl, cv::Mat & final_res_img, bool bDebugMode);

	int findCandidateBlobs( Mat & rgb, Mat & hsv, CBlobDetector & Blobs, int hue_th, vector<CBlobInfo> & );

	int Do( cv::Mat & rgb,  cv::Mat & hsv, cv::Mat & lab, int frame_num,  int hue_th, int nth, cv::Mat & final_res_img );

private : 
	double HistCompare(cv::Mat &src_base1, cv::Mat &src_base2);
private:
	cv::Mat m_DilationElement;
	cv::Mat m_ErosionElement;

private:

	cv::Mat m_TmpImg;
	cv::Mat m_BinaryImg;
	cv::Mat m_LabelingImg;

private:
	vector<CVL_Res> * m_pRes;
	CEV_AR_C1_DetectorParam m_Param;


private:
	unsigned char * m_pVR;
	int *** m_pClrLUT;
	//CCIFrameInfo m_FrameInfo;

private:
	//CEV_KeyframeMerger * m_pKM;
};
