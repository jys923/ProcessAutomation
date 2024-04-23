#include "pch.h"


#include <fstream>
#include <iostream>
#include <msclr\marshal_cppstd.h>

#include "opencv2/opencv.hpp"

#include "Markup.h"

#include "Recipe.h"

#include "IUAIController.h"

#include "UAIController.h"

#include "GrayImg.h"

#include "Result.h"

#include "VIRes.h"

#include "FD.h"

#include "Feature.h"

#include "VILib.h"

using namespace cv;
using namespace System;
using namespace System::Runtime::InteropServices;


IUAIController* m_pMgr = nullptr;
std::vector<CRecipe> m_vRecipeList;
int g_RecipeIndex = -1;
int gb_Load = false;


// CLR 클래스 라이브러리'를 선택하고 프로젝트를 생성

namespace VILib
{


	VILibWrapper::VILibWrapper()
	{
		m_pMgr = nullptr;
	}


	VILibWrapper::~VILibWrapper()
	{

		if (nullptr != m_pMgr)
		{
			delete m_pMgr;
			m_pMgr = nullptr;
		}
	
	}

	int VILibWrapper::Unload(int mode)
	{

		if (nullptr != m_pMgr)
		{
			delete m_pMgr;
			m_pMgr = nullptr;
		}


		gb_Load = false;
		g_RecipeIndex = -1;
		return 1;
	}


	int WriteRecipes()
	{
		CMarkup xml;

		xml.AddElem(MCD_T("MODEL"));

		xml.AddChildElem(MCD_T("Item"));
		xml.IntoElem();
		xml.AddAttrib(MCD_T("type"), MCD_T("U"));
		xml.AddChildElem(MCD_T("Name"), MCD_T("SC-GP5"));
		xml.AddChildElem(MCD_T("ImgH"), MCD_T("780"));
		xml.AddChildElem(MCD_T("ImgW"), MCD_T("780"));
		xml.AddChildElem(MCD_T("ImgCH"), MCD_T("4"));
		xml.AddChildElem(MCD_T("R"), MCD_T("140"));
		xml.AddChildElem(MCD_T("RMarg"), MCD_T("8"));
		xml.AddChildElem(MCD_T("Th"), MCD_T("190"));
		xml.OutOfElem();

		xml.AddChildElem(MCD_T("Item"));
		xml.IntoElem();
		xml.AddAttrib(MCD_T("type"), MCD_T("U"));
		xml.AddChildElem(MCD_T("Name"), MCD_T("SC-GP75"));
		xml.AddChildElem(MCD_T("ImgH"), MCD_T("780"));
		xml.AddChildElem(MCD_T("ImgW"), MCD_T("780"));
		xml.AddChildElem(MCD_T("ImgCH"), MCD_T("4"));
		xml.AddChildElem(MCD_T("R"), MCD_T("140"));
		xml.AddChildElem(MCD_T("RMarg"), MCD_T("8"));
		xml.AddChildElem(MCD_T("Th"), MCD_T("190"));
		xml.OutOfElem();

		xml.Save(MCD_T("SYS\\UCAPRecipe.xml"));

		return 1;
	}

	int ReadRecipes()
	{
		CMarkup xml;
		bool bret = xml.Load(MCD_T("SYS\\UCAPRecipe.xml"));
		if (false == bret)
		{
			gb_Load = false;
			return -1;
		}

		while (xml.FindChildElem(MCD_T("Item")))
		{
			CRecipe newR;

			xml.IntoElem();

			xml.FindChildElem(MCD_T("Name"));
			{
				std::wstring wsName = xml.GetChildData();
				newR.sModel.assign(wsName.begin(), wsName.end());
			}

			xml.FindChildElem(MCD_T("ImgH"));
			{
				std::wstring wsT = xml.GetChildData();
				std::string sH;
				sH.assign(wsT.begin(), wsT.end());
				newR.uiImgH = atoi(sH.c_str());
			}

			xml.FindChildElem(MCD_T("ImgW"));
			{
				std::wstring wsT = xml.GetChildData();
				std::string sH;
				sH.assign(wsT.begin(), wsT.end());
				newR.uiImgW = atoi(sH.c_str());
			}

			xml.FindChildElem(MCD_T("ImgCH"));
			{
				std::wstring wsT = xml.GetChildData();
				std::string sH;
				sH.assign(wsT.begin(), wsT.end());
				newR.uiImgCH = atoi(sH.c_str());
			}
			xml.FindChildElem(MCD_T("R"));
			{
				std::wstring wsT = xml.GetChildData();
				std::string sH;
				sH.assign(wsT.begin(), wsT.end());
				newR.uiR = atoi(sH.c_str());
			}
			xml.FindChildElem(MCD_T("RMarg"));
			{
				std::wstring wsT = xml.GetChildData();
				std::string sH;
				sH.assign(wsT.begin(), wsT.end());
				newR.uiRMarg = atoi(sH.c_str());
			}
			xml.FindChildElem(MCD_T("Th"));
			{
				std::wstring wsT = xml.GetChildData();
				std::string sH;
				sH.assign(wsT.begin(), wsT.end());
				newR.uiTh = atoi(sH.c_str());
			}

			xml.OutOfElem();

			m_vRecipeList.push_back(newR);
		}

		return 1;
	}


	int VILibWrapper::Load(int mode)
	{
		if (gb_Load) return 0;

		int ret = 0;
		if (1 == mode)
		{
			ret = WriteRecipes();
			if (ret < 0) return -1;
		}

		ret = ReadRecipes();
		if (ret < 0) return -1;

		for (int i = 0; i < m_vRecipeList.size(); i++)
		{
			CRecipe& r = m_vRecipeList[i];
			std::cout << r.sModel << std::endl;
			std::cout << r.uiImgH << std::endl;
			std::cout << r.uiR << std::endl;
			std::cout << r.uiTh << std::endl;
		}

		if (nullptr != m_pMgr)
		{
			delete m_pMgr;
			m_pMgr = nullptr;
		}
		
		m_pMgr = new CUAIController();
		m_pMgr->Create();

		gb_Load = true;

		return 1;
	}

	int FindRecipeIndex(std::string sModelName)
	{
		bool bFind = false;
		int idx = -1;
		for (int i = 0; i < m_vRecipeList.size(); i++)
		{
			CRecipe& r = m_vRecipeList[i];
			const char* rsModel = r.sModel.c_str();
			int cmp_res = strcmp(sModelName.c_str(), rsModel);
			if (0 == cmp_res)
			{
				bFind = true;
				idx = i;
				//std::cout << rsModel << std::endl;			
				break;
			}
		}

		return idx;
	}


	int VILibWrapper::DoInspection(System::String^ sModel, array<byte>^ src, int img_w, int img_h, int img_ch, 
		                           array<byte>^ dest, VIRes^% res, int opt)
	{

		if (false == gb_Load) return -11;

		std::string sModelName = msclr::interop::marshal_as<std::string>(sModel);
		std::cout << "sModelName : " << sModelName << std::endl;
#if 1
		int idx = FindRecipeIndex(sModelName);
		if (idx < 0) return -12;

		g_RecipeIndex = idx;
		std::cout << "recipe idx : " << g_RecipeIndex << std::endl;

		const CRecipe& rRecipe = m_vRecipeList[g_RecipeIndex];
		if (img_w != rRecipe.uiImgW) return -24;
		if (img_h != rRecipe.uiImgH) return -25;
		if (img_ch != rRecipe.uiImgCH) return -26;	
#else
		g_RecipeIndex = 0;
		CRecipe& rRecipe = m_vRecipeList[g_RecipeIndex];
		rRecipe.uiImgW = img_w;
		rRecipe.uiImgH = img_h;
		rRecipe.uiImgCH = 4;
		rRecipe.uiR = 140;
		rRecipe.uiRMarg = 8;
		rRecipe.uiTh = 180;
		
#endif

	    int ret = DoInspection(sModel, src, dest, res, opt);
		return ret;
	}


	int VILibWrapper::DoInspection(System::String^ sModel, array<byte>^ src, array<byte>^ dest, VIRes^% res, int opt)
	{

		//if (false == gb_Load)    return -22;
		if (g_RecipeIndex < 0)   return -33;
		if (nullptr == m_pMgr)   return -99;
	
		pin_ptr<uint8_t> source_pinned = &src[0];
		uint8_t* source_pin_c = source_pinned;

		CRecipe & rRecipe = m_vRecipeList[g_RecipeIndex];
		std::cout << rRecipe.uiImgW << std::endl;
		std::cout << rRecipe.uiImgH << std::endl;	
		std::cout << rRecipe.uiImgCH << std::endl;

		CGrayImg srcImg;
		srcImg.Create(source_pin_c, rRecipe.uiImgW, rRecipe.uiImgH, rRecipe.uiImgCH);

		std::string sModelName = msclr::interop::marshal_as<std::string>(sModel);
		std::string sTFullpath = "SYS\\" + sModelName + "_mask_t.bmp";
		cv::Mat maskTImg = cv::imread(sTFullpath, IMREAD_GRAYSCALE);
		if (maskTImg.empty())
		{
			return -1;
		}
		
		std::string sFFullpath = "SYS\\" + sModelName + "_mask_f.bmp";
		cv::Mat maskFImg = cv::imread(sFFullpath, IMREAD_GRAYSCALE);
		if (maskFImg.empty())
		{
			return -1;
		}

		CResult InspRes;
		m_pMgr->Do(srcImg, maskTImg, maskFImg, &rRecipe, InspRes);

		//---------------------------------------------------------------------
		// make result
		//---------------------------------------------------------------------

		//---------------------------------------------------------------------
		// draw a region of inspection
		//---------------------------------------------------------------------
		{
			Mat clr_src;
			cvtColor(srcImg.m_Img, clr_src, COLOR_GRAY2BGRA);
			int r = clr_src.rows / 2;
			int c = clr_src.cols / 2;

			int InnerR = rRecipe.GetR() - (rRecipe.GetRMarg() / 2);
			int OuterR = rRecipe.GetR() + (rRecipe.GetRMarg() / 2);
			circle(clr_src, cv::Point(c, r), InnerR, Scalar(0, 255, 0), 1);
			circle(clr_src, cv::Point(c, r), OuterR, Scalar(0, 255, 0), 1);

			memcpy(source_pin_c, clr_src.ptr<uchar>(0), rRecipe.uiImgW * rRecipe.uiImgH * rRecipe.uiImgCH);
		}


		//---------------------------------------------------------------------
		// copy results of inspection
		//---------------------------------------------------------------------
		uchar* pRes = InspRes.m_pData;
		pin_ptr<uint8_t> dest_pinned = &dest[0];
		uint8_t* dest_pin_c = dest_pinned;
		memcpy(dest_pin_c, pRes, rRecipe.uiImgW * rRecipe.uiImgH * rRecipe.uiImgCH);

		res->nCnt = InspRes.m_vROI.size();
		res->list = gcnew array<ResItem>(res->nCnt);
		const int nLimit = res->nCnt;
		for (int i = 0; i < nLimit; i++)
		{
			const CROI& rROI = InspRes.m_vROI.at(i);

			res->list[i].nState = rROI.nState;
			res->list[i].nx = rROI.x;
			res->list[i].ny = rROI.y;
			res->list[i].nw = rROI.h;
			res->list[i].nh = rROI.w;
			res->list[i].cen_x = rROI.cen_x;
			res->list[i].cen_y = rROI.cen_y;

			const CFD * pFD = rROI.m_pFD;
			const CFeature& rF = pFD->m_vFeature.at(0);
			res->list[i].nval = rF.m_dVal;
		}


		return 1;
	}

	int VILibWrapper::MakeMaskImg(System::String^ sModel, System::String^ full_path, int nw, int nh, int cen_x, int cen_y, int radius)
	{
		std::string sModelName = msclr::interop::marshal_as<std::string>(sModel);
		std::cout << "sModelName : " << sModelName << std::endl;

		std::string sFullpath = msclr::interop::marshal_as<std::string>(full_path);
		std::cout << "sFullpath : " << sFullpath << std::endl;
		

		{
			std::string sNFullpath = sFullpath + sModelName + "_mask_t.bmp";
			std::cout << "sFullpath : " << sNFullpath << std::endl;
			cv::Mat maskImg = Mat::zeros(nh, nw, CV_8UC3);

			int rmarg = 8;
			circle(maskImg, cv::Point(cen_x, cen_y), radius, Scalar(255, 255, 255), rmarg);
			bool bret = cv::imwrite(sNFullpath, maskImg);
			if (false == bret)
			{
				std::cout << "error : " << std::endl;
				return -12;
			}
		}

		{
			std::string sFFullpath = sFullpath + sModelName + "_mask_f.bmp";
			std::cout << "sFullpath : " << sFFullpath << std::endl;

			cv::Mat maskImg = Mat::zeros(nh, nw, CV_8UC3);

			int rmarg = 8;
			int inner_r = radius - (rmarg / 2 + 1);
			circle(maskImg, cv::Point(cen_x, cen_y), inner_r-1, Scalar(255, 255, 255), FILLED);
			circle(maskImg, cv::Point(cen_x, cen_y), inner_r-10, Scalar(0, 0, 0), FILLED);

			bool bret = cv::imwrite(sFFullpath, maskImg);
			if (false == bret)
			{
				std::cout << "error : " << std::endl;
				return -12;
			}
			/*
			for (int rr = 0; rr < maskImg.rows; rr++)
			{
				for (int cc = 0; cc < maskImg.cols; cc++)
				{
					unsigned char val = maskImg.at<Vec3b>(rr, cc)[0];
					if (val == 255 && rr == 390)
					{
						std::cout << rr << " , " << cc << std::endl;
					}
				}
			}
			*/
		}

		return 1;
	}


	//std::string sFullpath2 = "SYS\\"+ sModelName +"_mask.bmp";
			//std::cout << "sFullpath2 : " << sFullpath2 << std::endl;

			// r = 140
			// 140 - (marg/2) = 136

			// r = 131 marg = 8
			//   245

			// normal area
			/*
		rmarg == 8

	390 , 246
	390 , 247
	390 , 248
	390 , 249

	390 , 250

	390 , 251
	390 , 252
	390 , 253
	390 , 254




	390 , 526
	390 , 527
	390 , 528
	390 , 529
	390 , 530
	390 , 531
	390 , 532
	390 , 533
	390 , 534

	*/

	// false area
			/*
	390 , 255
	390 , 256
	390 , 257
	390 , 258
	390 , 259
	390 , 260
	390 , 261
	390 , 262
	390 , 263


	390 , 517
	390 , 518
	390 , 519
	390 , 520
	390 , 521
	390 , 522
	390 , 523
	390 , 524
	390 , 525

		*/



}