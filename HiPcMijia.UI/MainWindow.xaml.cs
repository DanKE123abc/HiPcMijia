using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using Exception = System.Exception;
using Microsoft.Win32;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace HiPcMijia.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private System.Windows.Forms.NotifyIcon notifyIcon = null;

    public MainWindow()
    {

        #region 检测

        string MName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
        string PName = System.IO.Path.GetFileNameWithoutExtension(MName);
        System.Diagnostics.Process[] myProcess = System.Diagnostics.Process.GetProcessesByName(PName);

        if (myProcess.Length > 1)
        {
            MessageBox.Show("程序已在运行，请勿重复运行", "HiPcMijia.UI");
            Debug.Warning("~~~~~尝试运行多个UI进程，已阻止~~~~~");
            Environment.Exit(0);
            return;
        }

        #endregion

        try
        {
            InitializeComponent();
            this.MouseLeave += MainWindow_MouseLeave;
        }
        catch (Exception ex)
        {
            Debug.Error($"MainWindow Startup Error: {ex}");
        }
        
    }

    private void MainWindow_MouseLeave(object sender, MouseEventArgs e)
    {
        this.Hide();
        this.Topmost = true;
    }

    private void MainWindow_OnDeactivated(object? sender, EventArgs e)
    {
        this.Hide();
        this.Topmost = true;
    }
    
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        init_Window();
        restartBtn_Click(null,null);
        OnDrawTextBlock();
        this.notifyIcon = new System.Windows.Forms.NotifyIcon();
        this.notifyIcon.Text = "HiPcMijia";
        this.notifyIcon.Icon = (System.Drawing.Icon)Properties.Resources.ResourceManager.GetObject("icon");
        this.notifyIcon.Visible = true;
        this.notifyIcon.Click += notifyIcon_Click;

    }

    private void init_Window()
    {
        Dispatcher.InvokeAsync(new Action(() => { this.Visibility = Visibility.Hidden; }));
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;
        double taskbarHeight = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height;
        // 设置窗口的高度为屏幕的高度减去任务栏高度
        this.Height = (screenHeight - taskbarHeight) / 2.5;
        this.Width = screenWidth / 5;
        // 设置窗口的位置
        DisableWindowDragging(this);
        this.Topmost = true; // 设置窗口始终显示在顶部
        this.Top = (screenHeight - this.Height - taskbarHeight);
        this.Left = screenWidth - this.Width - screenWidth / 20;
    }
    
    private void notifyIcon_Click(object? sender, EventArgs eventArgs)
    {
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;
        double taskbarHeight = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height;
        this.Top = (screenHeight - this.Height - taskbarHeight);
        this.Left = screenWidth - this.Width - screenWidth / 20;
        this.Visibility = Visibility.Visible;
        OnDrawTextBlock();
    }

    private void sending_Notifications(string message, int timeout = 1000)
    {
        this.notifyIcon.BalloonTipText = message;
        this.notifyIcon.ShowBalloonTip(timeout);
    }

    private void DisableWindowDragging(Window window)
    {
        IntPtr hwnd = new WindowInteropHelper(window).Handle;
        HwndSource.FromHwnd(hwnd)?.AddHook(WindowProc);
    }

    private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;

        if (msg == WM_SYSCOMMAND && (int)wParam == SC_MOVE)
        {
            handled = true;
        }

        return IntPtr.Zero;
    }

    private void startBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            HiPcMijiaProcess.Start();
        }
        catch (Exception exception)
        {
            sending_Notifications($"启动HiPcMijia进程发生错误：{exception}");
        }
        OnDrawTextBlock();
    }

    private void stopBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            HiPcMijiaProcess.Stop();
        }
        catch (Exception exception)
        {
            sending_Notifications($"停止HiPcMijia进程发生错误：{exception}");
        }
        OnDrawTextBlock();
    }

    private void restartBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            HiPcMijiaProcess.Restart();
        }
        catch (Exception exception)
        {
            sending_Notifications($"重启HiPcMijia进程发生错误：{exception}");
        }
        OnDrawTextBlock();
    }

    private void OnDrawTextBlock()
    {
        if (HiPcMijiaProcess.IsRunning)
        {
            this.textBlock.Foreground = new SolidColorBrush(Colors.Green);
            this.textBlock.Text = "已连接";
        }
        else
        {
            this.textBlock.Foreground = new SolidColorBrush(Colors.Brown);
            this.textBlock.Text = "未连接";
        }
    }

    private void setStartUpBtn_Click(object sender, RoutedEventArgs e)
    {
        RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        if (registryKey != null)
        {
            registryKey.SetValue("HiPcMijia", Application.ExecutablePath);
            MessageBox.Show(@"已设置开机自启设置", @"HiPcMijia", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void removeStartUpBtn_Click(object sender, RoutedEventArgs e)
    {
        RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        if (registryKey != null)
        {
            registryKey.DeleteValue("HiPcMijia", false);
            MessageBox.Show(@"已移除开机自启设置", @"HiPcMijia", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }


    private void quitBtn_Click(object sender, RoutedEventArgs e)
    {
        HiPcMijiaProcess.Stop();
        Environment.Exit(0);
    }
}
