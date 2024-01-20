using System;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using NAudio.CoreAudioApi;

namespace HiPcMijia;

public class WindowsCommand
{
    public static void Shutdown()
    {
        Debug.Log($"[Windows]Power off");
        Process.Start("shutdown", "/s /t 0");
        Process currentProcess = Process.GetCurrentProcess();
        Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
        foreach (Process process in processes)
        {
            process.Kill();
        }
    }

    public static void SetVolume(int volumeLevel)
    {
        try
        {
            var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            float volume = volumeLevel == 1? 0 : Math.Max(0, Math.Min(1, volumeLevel / 100.0f));
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            Debug.Log($"[Windows]SetVolume{volumeLevel}");
        }
        catch (Exception ex)
        {
            // TODO:处理异常
            // 处理它干嘛?调个音量还有处理异常吗？
            Debug.Error($"[Windows]SetVolume failed:{ex}");
        }
    }


    public static int GetVolume()
    {
        try
        {
            var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            int volumeLevel = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            return volumeLevel;
        }
        catch (Exception ex)
        {
            Debug.Error($"[Windows]GetVolume failed:{ex}");
            return 100;
        }
    }

    public static void SetScreenBrightness(int brightness)
    {
        try
        {
            AdjustScreenByWmi adjustScreen = new AdjustScreenByWmi();
            if (adjustScreen.IsSupported)
            {
                byte[] brightnessLevels = adjustScreen.GetBrightnessLevels();
                int level = brightnessLevels.FirstOrDefault(b => b >= brightness);
                if (level!= 0)
                {
                    adjustScreen.SetBrightness((byte)level);
                }
            }
            Debug.Log($"[Windows]SetBrightness{brightness}");
        }
        catch (Exception ex)
        {
            Debug.Error($"[Windows]SetBrightness failed:{ex}");
        }
        
    }

    public static int GetScreenBrightness()
    {
        try
        {
            var brightness = new AdjustScreenByWmi().GetBrightness();
            return brightness;
        }
        catch (Exception e)
        {
            return 100;
        }
        
    }
    
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);
    private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
    private const uint WM_SYSCOMMAND = 0x0112;
    private const uint SC_MONITORPOWER = 0xf170;
    public static void TurnOnScreen()
    {
        SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, -1);
    }
    public static void TurnOffScreen()
    {
        SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
    }


    public static bool IsAdministrator()
    {
        bool result;
        try
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            result = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            result = false;
        }

        return result;
    }

    // 以管理员身份重新启动应用程序
    public static void RestartAsAdmin(string[] args = null)
    {
        var startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = true;
        startInfo.WorkingDirectory = Environment.CurrentDirectory;
        startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
        if (args != null)
        {
            startInfo.Arguments = string.Join(" ", args);
        }

        startInfo.Verb = "runas"; // 请求管理员权限
        try
        {
            Process.Start(startInfo);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            Debug.Warning("管理员权限启动失败，请尝试以管理员身份重新启动应用程序。");
        }

        Environment.Exit(0);
    }

}