using System.Runtime.InteropServices;

public static class ImageProcessorNative
{
    const string DLL_NAME = "ImageProcessor.dll";

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int ProcessImage(
    string inputPath,
    string outputPath);
}