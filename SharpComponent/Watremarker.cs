using System;
using System.IO;
using System.Threading;

class Watermarker
{
    private LoggerCs logger;
    public Watermarker(LoggerCs logger)
    {
        this.logger = logger;
    }

    public void log(string msg)
    {
        logger.log(msg);
    }

    public void ProcessFileAsync(string inputPath, string outputDir)
    {
        string fileName = Path.GetFileName(inputPath);
        string outputPath = Path.Combine(outputDir, fileName);

        log($"Starting thread for {fileName}.");

        try
        {
            int result = ImageProcessorNative.ProcessImage(logger.log, inputPath, outputPath);
            if (result >= 0)
            {
                log($"Processed {fileName} -> {outputPath}.");
            }
            else
            {
                log($"Error processing {fileName} (C++ returned {result}).");
            }
        }
        catch (Exception ex)
        {
            log($"Exception in thread for {fileName}: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        LoggerCs logger = new LoggerCs("adapter.log", 10, 50);
        Watermarker watermarker = new Watermarker(logger);

        watermarker.log("App started.");

        string inputDir = @"C:\test_images\input";
        string outputDir = @"C:\test_images\output";

        if (!Directory.Exists(inputDir))
        {
            watermarker.log("Input directory does not exist.");
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
            watermarker.log("No supported image files found in input directory.");
            return;
        }

        watermarker.log($"Found {files.Length} image files. Starting threads...");

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

        watermarker.log("All threads finished. App finished.");
        logger.Dispose();
    }
}
