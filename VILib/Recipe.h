#pragma once	
#include "IRecipe.h"



class CRecipe : public IRecipe
{

public:
	unsigned int uiImgH;
	unsigned int uiImgW;
	unsigned int uiImgCH;

	unsigned int uiR;
	unsigned int uiRMarg;
	unsigned int uiTh;
	
	std::string  sModel;

public:
	CRecipe();

	int Read(std::string model);


	std::string GetModel()
	{
		return sModel;
	}

	unsigned int GetImgH()
	{
		return uiImgH;
	}

	unsigned int GetImgW()
	{
		return uiImgW;
	}

	unsigned int GetImgCh()
	{
		return uiImgCH;
	}

	unsigned int GetR()
	{
		return uiR;
	}

	unsigned int GetRMarg()
	{
		return uiRMarg;
	}	
	
	unsigned int GetTh()
	{
		return uiTh;
	}
};

