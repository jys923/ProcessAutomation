#pragma once

class IRecipe
{

public:
	IRecipe()
	{

	}

	virtual int Read(std::string model)
	{
		return -1;
	}
	virtual std::string GetModel()
	{
		std::string srt = "";
		return srt;
	}
	virtual unsigned int GetImgH()
	{
		return 0;
	}

	virtual unsigned int GetImgW()
	{
		return 0;
	}
	virtual unsigned int GetImgCh()
	{
		return 0;
	}
	virtual unsigned int GetR()
	{
		return 0;
	}
	virtual unsigned int GetRMarg()
	{
		return 0;
	}

	virtual unsigned int GetTh()
	{
		return 0;
	}

};

