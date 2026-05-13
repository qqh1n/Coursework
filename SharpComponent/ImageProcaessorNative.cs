using System.Runtime.InteropServices;

public static class ImageProcessorNative
{
    const string DLL_NAME = "ImageProcessor.dll";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LogCallback(string msg);

    private static LogCallback _logCallback;

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int ProcessImage(
    LogCallback logCallback,
    string inputPath,
    string outputPath);

    public static void SetLogCallback(LogCallback logCallback)
    {
        _logCallback = logCallback;
    }
}