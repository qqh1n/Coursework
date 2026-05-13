using System;
using System.IO;
using System.Threading;

class Watermarker
{
    public void ProcessFileAsync(string inputPath, string outputDir)
    {
        string fileName = Path.GetFileName(inputPath);
        string outputPath = Path.Combine(outputDir, fileName);

        LoggerCs.Log($"Starting thread for {fileName}.");

        try
        {
            int result = ImageProcessorNative.ProcessImage(LoggerCs.Log, inputPath, outputPath);
            if (result >= 0)
            {
                LoggerCs.Log($"Processed {fileName} -> {outputPath}.");
            }
            else
            {
                LoggerCs.Log($"Error processing {fileName} (C++ returned {result}).");
            }
        }
        catch (Exception ex)
        {
            LoggerCs.Log($"Exception in thread for {fileName}: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        Watermarker watermarker = new Watermarker();

        ImageProcessorNative.SetLogCallback(LoggerCs.Log);

        LoggerCs.Log("App started.");

        string inputDir = @"C:\test_images\input";
        string outputDir = @"C:\test_images\output";

        if (!Directory.Exists(inputDir))
        {
            LoggerCs.Log("Input directory does not exist.");
            return;
        }

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string[] files = Directory.GetFiles(inputDir, "*.*").Where(f =>
        {
            string ext = Path.GetExtension(f).ToLowerInvariant();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png";
        }).ToArray();

        if (files.Length == 0)
        {
            LoggerCs.Log("No supported image files found in input directory.");
            return;
        }

        LoggerCs.Log($"Found {files.Length} image files. Starting threads...");

        Thread[] threads = new Thread[files.Length];
        for (int i = 0; i < files.Length; ++i)
        {
            string inputFile = files[i];
            string outputDirLocal = outputDir;

            threads[i] = new Thread(() => watermarker.ProcessFileAsync(inputFile, outputDirLocal));
            threads[i].Start();
        }

        foreach (Thread t in threads)
        {
            t.Join();
        }

        LoggerCs.Log("All threads finished. App finished.");
    }
}
