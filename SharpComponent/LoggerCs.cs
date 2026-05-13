using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

public static class LoggerCs
{
    private static readonly object lockObj = new();
    private static StreamWriter _writer;

    public static void Open(string filePath)
    {
        var fileStream = new FileStream(
            filePath,
            FileMode.Append,
            FileAccess.Write,
            FileShare.ReadWrite
            );
        _writer = new StreamWriter(fileStream) { AutoFlush = true };
    }

    public static void Log(string msg)
    {
        lock (lockObj)
        {
            string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff} {msg}";
            _writer?.WriteLine(line);
        }
    }
    public static void Close()
    {
        lock (lockObj)
        {
            _writer?.Dispose();
        }
    }
}
