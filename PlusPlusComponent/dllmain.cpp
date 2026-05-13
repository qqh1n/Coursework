#include "pch.h"
#include <gdiplus.h>

using namespace Gdiplus;

static ULONG_PTR g_gdiplusToken = 0;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        DisableThreadLibraryCalls(hModule);
        {
            GdiplusStartupInput gdiplusStartupInput;
            GdiplusStartup(&g_gdiplusToken, &gdiplusStartupInput, NULL);
        }
        break;
    case DLL_PROCESS_DETACH:
        if (g_gdiplusToken != 0)
            GdiplusShutdown(g_gdiplusToken);
        break;
    }
    return TRUE;
}