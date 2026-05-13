#define _CRT_SECURE_NO_WARNINGS
#include "pch.h"
#include "ImageProcessorHeader.h"
#include "LoggerCppHeader.h"

#pragma comment(lib, "Gdiplus.lib")

using namespace Gdiplus;

int GetEncoderClsid(const WCHAR* format, CLSID* pClsid) {
    UINT num = 0;
    UINT size = 0;
    GetImageEncodersSize(&num, &size);
    if (size == 0) return -1;

    ImageCodecInfo* pImageCodecInfo = (ImageCodecInfo*)malloc(size);
    if (!pImageCodecInfo) return -1;

    GetImageEncoders(num, size, pImageCodecInfo);
    for (UINT j = 0; j < num; ++j) {
        if (wcscmp(pImageCodecInfo[j].MimeType, format) == 0) {
            *pClsid = pImageCodecInfo[j].Clsid;
            free(pImageCodecInfo);
            return j;
        }
    }
    free(pImageCodecInfo);
    return -1;
}

extern "C" IMAGEPROCESSOR_API int ProcessImage(const wchar_t* inputPath, const wchar_t* outputPath) {
    LoggerCpp::log("ProcessImage: Drawing start...");

    int result = 0;
    {
        if (GetFileAttributesW(inputPath) == INVALID_FILE_ATTRIBUTES) {
            LoggerCpp::log("ProcessImage: File not found!");
            result = -11;
            goto cleanup;
        }

        Bitmap* bitmap = new Bitmap(inputPath);
        if (!bitmap || bitmap->GetLastStatus() != Ok) {
            LoggerCpp::log("ProcessImage: Failed to load bitmap!");
            delete bitmap;
            result = -2;
            goto cleanup;
        }

        Graphics* graphics = Graphics::FromImage(bitmap);
        if (!graphics || graphics->GetLastStatus() != Ok) {
            LoggerCpp::log("ProcessImage: Failed to create graphics!");
            delete graphics;
            delete bitmap;
            result = -3;
            goto cleanup;
        }

        Pen redPen(Color(255, 255, 0, 0), 5);
        graphics->DrawRectangle(&redPen, 10, 10, 200, 100);

        FontFamily fontFamily(L"Arial");
        if (!fontFamily.IsAvailable()) {
            FontFamily fallback(L"Microsoft Sans Serif");
            Font font(&fallback, 24, FontStyleBold, UnitPixel);
            SolidBrush textBrush(Color(255, 255, 255, 0));
            graphics->DrawString(L"Processed by C++", -1, &font, PointF(15.0f, 15.0f), &textBrush);
        }
        else {
            Font font(&fontFamily, 24, FontStyleBold, UnitPixel);
            SolidBrush textBrush(Color(255, 255, 255, 0));
            graphics->DrawString(L"Processed by C++", -1, &font, PointF(15.0f, 15.0f), &textBrush);
        }

        CLSID pngClsid;
        if (GetEncoderClsid(L"image/png", &pngClsid) == -1) {
            LoggerCpp::log("ProcessImage: PNG encoder not found!");
            delete graphics;
            delete bitmap;
            result = -4;
            goto cleanup;
        }

        Status status = bitmap->Save(outputPath, &pngClsid, NULL);
        delete graphics;
        delete bitmap;

        if (status != Ok) {
            LoggerCpp::log("ProcessImage: Save failed!");
            result = -5;
            goto cleanup;
        }
    }

cleanup:
    LoggerCpp::log(result == 0 ? "ProcessImage: Drawing success!" : "ProcessImage: Finished with error.");
    return result;
}
