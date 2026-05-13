#pragma once
#include "pch.h"

#ifdef IMAGEPROCESSOR_EXPORTS
#define IMAGEPROCESSOR_API __declspec(dllexport)
#else
#define IMAGEPROCESSOR_API __declspec(dllimport)
#endif

extern "C" IMAGEPROCESSOR_API int ProcessImage(void (*logCallback)(const char* msg), const wchar_t* inputPath, const wchar_t* outputPath);