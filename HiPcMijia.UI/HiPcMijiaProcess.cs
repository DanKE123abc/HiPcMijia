using System.Diagnostics;

namespace HiPcMijia.UI;

public class HiPcMijiaProcess
{
    private static Process hiPcMijiaProcess = new();

    public static void Start()
    {
        hiPcMijiaProcess.StartInfo.FileName = ".//HiPcMijia.exe";
        hiPcMijiaProcess.StartInfo.Arguments = "";
        hiPcMijiaProcess.Start();
    }

    public static void Stop()
    {
        Process[] processes = Process.GetProcessesByName("HiPcMijia");
        if (processes.Count() > 0)//判断如果存在
        {
            processes[0].Kill();//关闭程序
        }
        Thread.Sleep(1000);
    }

    public static void Restart()
    {
        if (IsRunning)
        {
            Stop();
        }
        Start();
    }
    
    public static bool IsRunning
    {
        get
        {
            Process[] processes = Process.GetProcessesByName("HiPcMijia");
            if (processes.Length > 0)
            {
                return true;
            }
            else
            {
                return false;

            }
        }
    }
    
    
}