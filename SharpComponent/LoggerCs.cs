using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

public class LoggerCs: IDisposable
{
    private string filePath;
    private int batchSize;
    private int flushTimeout;
    private static readonly object lockObj = new();
    private readonly CancellationTokenSource cts = new();
    private Queue<string> currentBatch;
    private ConcurrentQueue<Queue<string>> batchesQueue;
    private volatile bool isRunning = true;
    private Thread writerThread;

    public LoggerCs(string filePath, int batchSize, int flushTimeout)
    {
        this.filePath = filePath;
        this.batchSize = batchSize;
        this.flushTimeout = flushTimeout;
        this.currentBatch = new Queue<string>();
        this.batchesQueue = new ConcurrentQueue<Queue<string>>();
        writerThread = new Thread(WriterLoop) { IsBackground = true, Name = "LogWriter" };
        writerThread.Start();
    }

    public void log(string msg)
    {
        lock (lockObj)
        {
            string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff} {msg}";
            currentBatch.Enqueue(line);
            if (currentBatch.Count == batchSize)
            {
                swapCurrentQueue();
            }
        }
    }

    private void swapCurrentQueue()
    {
        batchesQueue.Enqueue(currentBatch);
        currentBatch = new Queue<string>();
    }

    private void WriterLoop()
    {
        FileStream fileStream = new FileStream(
            filePath,
            FileMode.Append,
            FileAccess.Write,
            FileShare.ReadWrite
            );
        StreamWriter writer = new StreamWriter(fileStream) { AutoFlush = false };

        while (!cts.Token.IsCancellationRequested)
        {
            FlushBatches(writer);
            Thread.Sleep(flushTimeout);
        }
        FlushBatches(writer);
        writer.Flush();
    }

    private void FlushBatches(StreamWriter writer)
    {
        while (batchesQueue.TryDequeue(out var batch))
        {
            while (batch.TryDequeue(out var line))
            {
                writer.WriteLine(line);
            }
        }
        writer.Flush();
    }

    public void Shutdown()
    {
        lock (lockObj)
        {
            if (currentBatch.Count > 0)
            {
                swapCurrentQueue();
            }
        }

        cts.Cancel();
        writerThread.Join(5000);
        cts.Dispose();
    }

    public void Dispose() => Shutdown();
}
