#include "pch.h"
#include "Functions.h"
#include <chrono>
#include <ratio>

double ForTest(int length)
{
	auto start_time = std::chrono::high_resolution_clock::now();
	int j = 0;
	for (int i = 0; i < length; i++)
	{
		j++;
	}
	auto end_time = std::chrono::high_resolution_clock::now();
	std::chrono::duration<double, std::milli> elapsed_time = end_time - start_time;

	double elapsed_ms = elapsed_time.count();

	return elapsed_ms;
}