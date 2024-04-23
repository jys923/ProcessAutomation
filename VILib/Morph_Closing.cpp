#include "pch.h"
#include "Morph_Closing.h"


CMorph_Closing::CMorph_Closing()
{
	m_bCreated = false;

	m_uiType = 2;
	m_uiSize = 1;

	m_ErosionElement = getStructuringElement(m_uiType, cv::Size(2 * m_uiSize + 1, 2 * m_uiSize + 1));
	m_DilationElement = getStructuringElement(m_uiType, cv::Size(2 * m_uiSize + 1, 2 * m_uiSize + 1));
}

CMorph_Closing::~CMorph_Closing()
{

}


int CMorph_Closing::Create(unsigned int type, unsigned int size)
{
	m_bCreated = true;

	m_uiType = type;
	m_uiSize = size;

	m_ErosionElement  = getStructuringElement(m_uiType, cv::Size(2 * m_uiSize + 1, 2 * m_uiSize + 1));
	m_DilationElement = getStructuringElement(m_uiType, cv::Size(2 * m_uiSize + 1, 2 * m_uiSize + 1));

	return 1;
}

int CMorph_Closing::Do(const cv::Mat& src, cv::Mat& res)
{
	if (false == m_bCreated) return -10;

	cv::dilate(src, res, m_DilationElement);
	cv::erode(res, res, m_ErosionElement);

	return 1;
}