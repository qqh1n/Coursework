using System.Runtime.InteropServices;


[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void LogCallback(string message);
public static class ImageProcessorNative
{
    const string DLL_NAME = "ImageProcessor.dll";

    private static LogCallback _logCallback;

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int ProcessImage(
    LogCallback logCallback,
    [MarshalAs(UnmanagedType.LPWStr)] string inputPath,
    [MarshalAs(UnmanagedType.LPWStr)] string outputPath);

    public static void SetLogCallback(LogCallback logCallback)
    {
        _logCallback = logCallback;
    }
}