#include "pch.h"

#include "BlobDetector.h"

//#include "EV_Util.h"

//#include "EV_AlgConfig.h"

//#include "EV_KeyframeMerger.h"

#include "UAI.h"



CUAI::CUAI()
{
	m_pVR = NULL;
	m_pClrLUT = NULL;
	m_pRes = NULL;

}

int CUAI::Create(CCIFrameInfo & rFrameInfo, const CEV_AlgConfig & Params, int nID)
{
	m_nType = nID;

	m_Param.m_nCameraSensorType = Params.m_nCameraSensorType;

	if (Params.m_bDSMode)
	{
		m_pVR = Params.m_pHalfVR;
		m_FrameInfo = rFrameInfo;

		m_FrameInfo.nH /= 2;
		m_FrameInfo.nW /= 2;

		m_Param.MIN_SIZE /= 4;
		m_Param.MAX_SIZE /= 4;

		m_Param.LIM_WIDTH /= 2;

		m_Param.CAND_RG_MIN_SIZE /= 4;

		//m_Param.RG_DIST = 3;


		int dilation_type = 2;
		int dilation_size = 1;
		m_ErosionElement = getStructuringElement(dilation_type, cv::Size(2 * dilation_size + 1, 2 * dilation_size + 1));
		m_DilationElement = getStructuringElement(dilation_type, cv::Size(2 * dilation_size + 1, 2 * dilation_size + 1));

		m_Param.m_bDSMode = true;
	}
	else
	{
		m_pVR = Params.m_pVR;
		m_FrameInfo = rFrameInfo;

		m_Param.m_bDSMode = false;

		int dilation_type = 2;
		int dilation_size = 1;

		m_ErosionElement = getStructuringElement(dilation_type, cv::Size(2 * dilation_size + 1, 2 * dilation_size + 1));
		m_DilationElement = getStructuringElement(dilation_type, cv::Size(2 * dilation_size + 1, 2 * dilation_size + 1));

	}

	m_pClrLUT = Params.m_pClrLUT;

	m_TmpImg = cv::Mat(cv::Size(m_FrameInfo.nH, m_FrameInfo.nW), CV_8UC1, cv::Scalar(0));
	m_BinaryImg = cv::Mat(cv::Size(m_FrameInfo.nH, m_FrameInfo.nW), CV_8UC1, cv::Scalar(0));
	m_LabelingImg = cv::Mat(cv::Size(m_FrameInfo.nH, m_FrameInfo.nW), CV_8UC1, cv::Scalar(0));

	m_pRes = new vector<CVL_Res>;
	if (NULL == m_pRes) return -1;
	m_pRes->reserve(100);


	return 1;
}


int CUAI::clear()
{
	if( m_pRes != NULL )
	{
		m_pRes->clear();
	}
	m_BinaryImg   = cv::Scalar(0);
	m_LabelingImg = cv::Scalar(0);
	m_TmpImg      = cv::Scalar(0);

	return 1;
}

CUAI::~CUAI(void)
{
	if( m_pRes != NULL )
	{
		delete m_pRes;
		m_pRes = NULL;
	}


}


int CUAI::checkReddishColor(cv::Mat & rgb, cv::Mat & hsv, cv::Mat & lab, int i, int j, int *** pLUT)
{

	int r = rgb.at<Vec3b>(i, j)[2];
	int g = rgb.at<Vec3b>(i, j)[1];
	int b = rgb.at<Vec3b>(i, j)[0];
	if (r < m_Param.RED_LOW) return 0;
	if (g > r || b > r)	return 0;
	if (r >= m_Param.RED_HI)
	{
		if ((r - g) <= m_Param.RG_DIFF_TH) return 0;
	}


	if( lab.at<Vec3b>(i,j)[0] < m_Param.LAB_LUMI_TH ) return 0;
	int diff = abs( lab.at<Vec3b>(i, j)[1] - lab.at<Vec3b>(i, j)[2] );
	if( diff >= m_Param.LAB_AB_DIFF_TH ) return 0;

	int hue = hsv.at<Vec3b>(i, j)[0];
	if( hue >= m_Param.HUE_RED_LOW && hue <= m_Param.HUE_RED_HIGH) return 0;
	if( hsv.at<Vec3b>(i, j)[1] < m_Param.SAT_RED_LOW ) return 0;

	int nr = r / m_Param.LUT_R_QS;
	int ng = g / m_Param.LUT_G_QS;
	int nb = b / m_Param.LUT_B_QS;
	nr *= m_Param.LUT_R_QS;
	ng *= m_Param.LUT_G_QS;
	nb *= m_Param.LUT_B_QS;
	if( pLUT[nr][ng][nb] > 0 )
	{
		return 1;
	}

	return 0;
}



int CUAI::checkReddishColor(unsigned char * prgb, unsigned char * phsv, unsigned char * plab, int i, int *** pLUT)
{
	int r = prgb[i + 2];
	int g = prgb[i + 1];
	int b = prgb[i];
	if (r < m_Param.RED_LOW) return 0;
	if (g > r || b > r)	return 0;
	if (r >= m_Param.RED_HI)
	{
		if ((r - g) <= m_Param.RG_DIFF_TH) return 0;
	}

	if (plab[i] < m_Param.LAB_LUMI_TH) return 0;
	int diff = abs(plab[i+1] - plab[i+2]);
	if (diff >= m_Param.LAB_AB_DIFF_TH) return 0;

	int hue = phsv[i];
	if (hue >= m_Param.HUE_RED_LOW && hue <= m_Param.HUE_RED_HIGH) return 0;
	if (phsv[i+1] < m_Param.SAT_RED_LOW) return 0;

	int nr = r / m_Param.LUT_R_QS;
	int ng = g / m_Param.LUT_G_QS;
	int nb = b / m_Param.LUT_B_QS;
	nr *= m_Param.LUT_R_QS;
	ng *= m_Param.LUT_G_QS;
	nb *= m_Param.LUT_B_QS;
	if (pLUT[nr][ng][nb] > 0)
	{
		return 1;
	}

	return 0;
}



int CUAI::doBinaryzation(cv::Mat & rgb, cv::Mat & hsv, cv::Mat & lab, cv::Mat & res, int dist, int nTh, unsigned char * pVR, int *** pLUT)
{

	//----------------------------------------------------------------------------
	// make 1st reddish map
	//----------------------------------------------------------------------------
	m_TmpImg = cv::Scalar(0);
	if (rgb.isContinuous() && hsv.isContinuous() && lab.isContinuous() && m_TmpImg.isContinuous() )
	{
		int nc = hsv.rows*hsv.cols*hsv.channels();

		uchar * phsv = hsv.ptr<uchar>(0);
		uchar * prgb = rgb.ptr<uchar>(0);
		uchar * plab = lab.ptr<uchar>(0);

		uchar * out = m_TmpImg.ptr<uchar>(0);
		for (int jj = 0, kk = 0; jj < nc; jj += 3, kk++)
		{
			//--------------------------------------------------------------------------
			// check positon
			//--------------------------------------------------------------------------
			if ( 0 == pVR[kk])	continue;

			int type = checkReddishColor( prgb, phsv, plab, jj, pLUT);
			if (0 == type) continue;

			out[kk] = 1;
		}
	}
	else
	{

		for (int ii = dist; ii < rgb.rows - dist; ii++)
		{
			int npos = ii * rgb.cols;
			for (int jj = dist; jj < rgb.cols - dist; jj++)
			{
				if ( 0 == pVR[npos + jj])	continue;

				int type = checkReddishColor(rgb, hsv, lab, ii, jj, pLUT);
				if (0 == type) continue;

				m_TmpImg.at<uchar>(ii, jj) = 1;
			}
		}
	}



	//----------------------------------------------------------------------------
	// make 2nd reddish map
	//----------------------------------------------------------------------------
	{
		for (int ii = dist; ii < rgb.rows - dist; ii++)
		{
			for (int jj = dist; jj < rgb.cols - dist; jj++)
			{
				if (0 == m_TmpImg.at<uchar>(ii, jj)) continue;

				int sum = -1;
				sum = m_TmpImg.at<uchar>(ii + 1, jj) +
					m_TmpImg.at<uchar>(ii - 1, jj) +
					m_TmpImg.at<uchar>(ii, jj + 1) +
					m_TmpImg.at<uchar>(ii, jj - 1) +
					m_TmpImg.at<uchar>(ii + 1, jj + 1) +
					m_TmpImg.at<uchar>(ii + 1, jj - 1) +
					m_TmpImg.at<uchar>(ii - 1, jj + 1) +
					m_TmpImg.at<uchar>(ii - 1, jj - 1);
				if (sum >= nTh) res.at<unsigned char>(ii, jj) = 255;
			}
		}
	}

	return 1;
}


int CUAI::findAbnormalRegion(CBlobInfo & rbi, cv::Mat & hsv, cv::Mat & rgb, cv::Mat & BinImg, unsigned char * pVR, int dist, CVL_Res & vl,  cv::Mat & final_res_img, bool bDebugMode)
{
	
	//-------------------------------------------------------------------------
	// find abnormal region
	//-------------------------------------------------------------------------
	CAbnormalRegionParam ARParam;
	ARParam.nRange = dist;
	ARParam.nRG_TH = m_Param.RG_TH;
	ARParam.fRG_OV_TH = m_Param.RG_OV_TH;
	ARParam.HUE_RED_HIGH = m_Param.HUE_RED_HIGH;
	ARParam.HUE_RED_LOW = m_Param.HUE_RED_LOW;
	CAbnormalRegionResult ARRes;
	int ret = find_abnormal_region(rbi, hsv, rgb, BinImg, pVR, ARParam, ARRes, final_res_img, bDebugMode);
	if( ret <= 0 ) return -1;
	if(ARRes.nptCnt <= 1 ) return -1;


	//-------------------------------------------------------------------------
	// Regional Descriptor : extract shape information
	//-------------------------------------------------------------------------
	float fRatio = (float)(ARRes.nptCnt) / (float)(rbi.nPtCnt) ;
	float fRTH = 2.5;
	if( rbi.nPtCnt < m_Param.CAND_RG_MIN_SIZE )
	{
		fRTH = get_LIV( (float)(m_Param.MIN_SIZE), 7.0f, (float)(m_Param.CAND_RG_MIN_SIZE), 2.5f, (float)rbi.nPtCnt );
	}
	if( fRatio >= fRTH )	return -1;


	float fw = (float)ARRes.rRR.size.width;
	float fh = (float)ARRes.rRR.size.height;
	float fratio = 0;
	if( fw > fh )   fratio = fw / fh;
	else			fratio = fh / fw;
	if( fratio >= 3.0 )
	{
		return -1;
	}


	//-------------------------------------------------------------------------
	// save result
	//-------------------------------------------------------------------------
	int ext_sx = rbi.sx - dist;
	if( ext_sx < 0) ext_sx = 0;
	int ext_sy  = rbi.sy - dist;
	if( ext_sy < 0 ) ext_sy = 0;
	vl.x = ext_sx + ARRes.region.x;
	vl.y = ext_sy + ARRes.region.y;
	vl.w = ARRes.region.width;
	vl.h = ARRes.region.height;
	vl.cen_x = (vl.x + vl.w/2);
	vl.cen_y = (vl.y + vl.h/2);
	vl.area  = ARRes.nptCnt;

	return 1;
}


int CUAI::findCandidateBlobs( Mat & rgb, Mat & hsv, CEV_BlobDetector & blob_list, int hue_th, vector<CBlobInfo> & res )
{
	int valid_cnt = 0;
	int nBlobs = blob_list.m_vBlobs.size();
	for( int bi = 0; bi < nBlobs; bi++ )
	{
		blob_list.m_vValid.at(bi) = false;

		//---------------------------------------------------------------------------
		// check shape features
		//---------------------------------------------------------------------------
		int nBlobPtCnt = blob_list.m_vBlobs[bi].size();
		if( nBlobPtCnt < 1 ) continue;

		cv::RotatedRect & rRR = blob_list.m_vRotatedBoundingRect.at(bi);
		if( rRR.size.height <= 1 || rRR.size.width <= 1 ) continue;
		if( rRR.size.height > m_Param.LIM_WIDTH || rRR.size.width > m_Param.LIM_WIDTH )	continue;
		int th_size = (int)( (float)rRR.size.area() * 0.30f );
		if ( nBlobPtCnt < th_size )	continue;


		//---------------------------------------------------------------------------
		//  extract color prop.
		//---------------------------------------------------------------------------
		cv::Rect & region =  blob_list.m_vBoundingRect.at(bi);
		int sx = region.x;	
		int sy = region.y;		
		int ex = sx + (region.width+1);		
		int ey = sy + (region.height+1);
		if( ex >= rgb.cols ) ex = rgb.cols;
		if( ey >= rgb.rows ) ey = rgb.rows;

		int min_h = 300;
		vector<cv::Point> & ptList = blob_list.m_vBlobs[bi];
		for( unsigned int ii = 0; ii < ptList.size(); ii++ )
		{
			cv::Point & pt = ptList.at( ii );
			int ch = hsv.at<Vec3b>(pt.y, pt.x)[0] ;
			if( ch > m_Param.HUE_RED_HIGH )
			{
				ch = 180 - ch;
				if( ch >= m_Param.HUE_RED_LOW ) ch = m_Param.HUE_RED_LOW -1;
			}
			if( min_h > ch )
				min_h = ch;
		}
		if( min_h > hue_th ) continue;


		//---------------------------------------------------------------------------
		// create blob info 
		//---------------------------------------------------------------------------
		CBlobInfo blob_info;
		blob_info.id = bi;
		blob_info.sx = sx;
		blob_info.sy = sy;
		blob_info.ex = ex;
		blob_info.ey = ey;
		blob_info.h = min_h;
		blob_info.nPtCnt = nBlobPtCnt;
		res.push_back( blob_info );

		blob_list.m_vValid.at(bi) = true;
		valid_cnt++;
	} 

	return valid_cnt;
}



int CUAI::Do( cv::Mat & rgb,  cv::Mat & hsv, cv::Mat & lab, int frame_num, int hue_th, int nTh, cv::Mat & final_res_img )
{

	//----------------------------------------------------------------------------
	//  initialize
	//----------------------------------------------------------------------------
	m_BinaryImg   = cv::Scalar(0);
	m_LabelingImg = cv::Scalar(0);

	if (m_Param.m_bDSMode)
	{
		//int ret = doBinaryzation(rgb, hsv, lab, m_BinaryImg, 1, 3, m_pVR, m_pClrLUT);
		//int ret = doBinaryzation(rgb, hsv, lab, m_BinaryImg, 1, 2, m_pVR, m_pClrLUT);
		int ret = doBinaryzation(rgb, hsv, lab, m_BinaryImg, 1, 1, m_pVR, m_pClrLUT);
		if (ret <= 0) return 0;

		//----------------------------------------------------------------------------
		// remove small regions
		//----------------------------------------------------------------------------
		cv::dilate(m_BinaryImg, m_LabelingImg, m_DilationElement);
		cv::erode(m_LabelingImg, m_LabelingImg, m_ErosionElement);
	}
	else
	{
		//----------------------------------------------------------------------------
		//  doBinaryzation
		//----------------------------------------------------------------------------
		int ret = doBinaryzation(rgb, hsv, lab, m_BinaryImg, 1,3, m_pVR, m_pClrLUT);
		if (ret <= 0) return 0;

		//----------------------------------------------------------------------------
		// remove small regions
		//----------------------------------------------------------------------------
		cv::dilate(m_BinaryImg, m_LabelingImg, m_DilationElement);
		cv::erode(m_LabelingImg, m_LabelingImg, m_ErosionElement);
	}




	//---------------------------------------------------------------------------
	// find blobs
	//---------------------------------------------------------------------------
	CBlobDetectorParams blob_param;
	blob_param.m_nImgH = m_FrameInfo.nH;
	blob_param.m_nImgW = m_FrameInfo.nW;
	blob_param.m_nMinAreaSize = m_Param.MIN_SIZE;
	blob_param.m_nMaxAreaSize = m_Param.MAX_SIZE;
	CBlobDetector cand_blob( blob_param );
	cand_blob.Do( m_LabelingImg );
	int nBlobs = cand_blob.m_vBlobs.size();
	if( nBlobs == 0 )  return 0;


	//----------------------------------------------------------------------------
	//
	// find candidate blobs
	//
	//----------------------------------------------------------------------------
	vector<CBlobInfo> bi_list;
	bi_list.reserve( nBlobs );
	int valid_cnt =  findCandidateBlobs( rgb, hsv, cand_blob, hue_th, bi_list );
	if( valid_cnt == 0 || valid_cnt >= m_Param.LIM_CAND )  return 0;


	
	}  // main for routine


	

}


int CUAI::Do( cv::Mat & rgb, cv::Mat & hsv, cv::Mat & lab, int frame_num)
{

	/*
		1. using a LUT Table of rgb ( by manual )
		2. over 10*10 size
	*/


	//----------------------------------------------------------------------------
	//  check param
	//----------------------------------------------------------------------------
	// if( frame_num < 0 ) return 0;


	//----------------------------------------------------------------------------
	//  check camera sensor type
	//----------------------------------------------------------------------------
	//if (Params.m_nCameraSensorType >= 1) return 0;


	//----------------------------------------------------------------------------
	//  main routine
	//----------------------------------------------------------------------------

#if 0
	cv::Mat final_res_img;
	m_pRes->clear();
	int ret = Do( rgb, hsv, lab, frame_num, 15, 0, final_res_img);
	if( 1 == ret ) return 1;
	else 	       return 0;
#endif
	return 1;
}
