using System;
using System.IO;
using System.Threading;

public static class LoggerCs
{
    private static readonly object lockObj = new();

    public static void Log(string msg)
    {
        lock (lockObj)
        {
            string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff} [CS] {msg}";
            File.AppendAllText("adapter_cs.log", line + Environment.NewLine);
        }
    }
}
