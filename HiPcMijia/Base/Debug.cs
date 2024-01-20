using System;
using System.IO;

namespace HiPcMijia;

public class Debug
{
    public static string LogFilePath;
    public static string WarningFilePath;
    public static string ErrorFilePath;

    public static bool IsLogEnabled;
    public static bool IsWarningEnabled;
    public static bool IsErrorEnabled;

    public static void Log(object message)
    {
        if (IsLogEnabled)
        {
            Console.WriteLine($"L[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
            WriteToFile($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}", LogFilePath);
        }
    }

    public static void Warning(object message)
    {
        if (IsWarningEnabled)
        {
            Console.WriteLine($"W[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
            WriteToFile($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}", WarningFilePath);
        }
    }

    public static void Error(object message)
    {
        if (IsErrorEnabled)
        {
            Console.WriteLine($"E[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
            WriteToFile($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}", ErrorFilePath);
        }
    }

    private static void WriteToFile(string message, string path)
    {
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(message);
        }
    }
}