using System;
using System.Collections;
using System.Diagnostics;
using HiPcMijia.BemfaApi;
using System.Diagnostics;
using DanKeJson;
using HiPcMijia.Coroutine;
using Newtonsoft.Json;

namespace HiPcMijia;

public class Program
{
    public static Config config;
    public static BemfaConnect pcPower;
    public static BemfaConnect pcVolume;
    public static BemfaConnect pcScreenBrightness;
    public static BemfaConnect pcBluetoothEvent;
    public static void Main()
    {
        InitConfig();
        string strProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        if(config.DeveloperSetting.BlockMultipleInstances)
        {
            if (System.Diagnostics.Process.GetProcessesByName(strProcessName).Length > 1)
            {
                Debug.Warning("~~~~~尝试运行多个实例，已阻止~~~~~");
                return;
            }
        }

        Debug.Log("~~~~~~~~~~~~~~Running~~~~~~~~~~~~~~");
        Debug.Warning("~~~~~~~~~~~~~~Running~~~~~~~~~~~~~~");
        Debug.Error("~~~~~~~~~~~~~~Running~~~~~~~~~~~~~~");

        if (config.FunctionSetting.PcPower)
            pcPower = new BemfaConnect(config.BemfaConfig, config.FunctionSetting.PcPowerName, PcPowerControl);
        if (config.FunctionSetting.PcVolume)
            pcVolume = new BemfaConnect(config.BemfaConfig, config.FunctionSetting.PcVolumeName, SetVolumeEvent);
        if (config.FunctionSetting.PcScreenBrightness)
            pcScreenBrightness = new BemfaConnect(config.BemfaConfig, config.FunctionSetting.PcScreenBrightnessName, SetScreenBrightnessEvent);
        if (config.FunctionSetting.PcBluetooth)
            pcBluetoothEvent = new BemfaConnect(config.BemfaConfig, config.FunctionSetting.PcBluetoothName, BluetoothEvent);

        while (true)
        {
            if (pcPower != null)
                pcPower.SendMsg("on");
            if (pcVolume != null)
                pcVolume.SendMsg($"on#{WindowsCommand.GetVolume()}");
            if (pcScreenBrightness != null)
                pcScreenBrightness.SendMsg($"on#{WindowsCommand.GetScreenBrightness()}");
            Thread.Sleep(config.FunctionSetting.ReportingInterval);
        }

        void InitConfig()
        {
            try
            {
                var jsonString = File.ReadAllText(".\\Config.jsonc");
                config = JSON.ToData<Config>(jsonString, true);
                if (config.DeveloperSetting.AutoCleanuplogFile)
                {
                    if (File.Exists(config.DeveloperSetting.LogFilePath))
                        File.Delete(config.DeveloperSetting.LogFilePath);
                    if (File.Exists(config.DeveloperSetting.WarningFilePath))
                        File.Delete(config.DeveloperSetting.WarningFilePath);
                    if (File.Exists(config.DeveloperSetting.ErrorFilePath))
                        File.Delete(config.DeveloperSetting.ErrorFilePath);
                }
                Debug.IsLogEnabled = config.DeveloperSetting.IsLogEnabled;
                Debug.IsWarningEnabled = config.DeveloperSetting.IsWarningEnabled;
                Debug.IsErrorEnabled = config.DeveloperSetting.IsErrorEnabled;
                Debug.LogFilePath = config.DeveloperSetting.LogFilePath;
                Debug.WarningFilePath = config.DeveloperSetting.WarningFilePath;
                Debug.ErrorFilePath = config.DeveloperSetting.ErrorFilePath;
            }
            catch(Exception ex)
            {
                Debug.Warning("读取配置文件出错");
                return;
            }
        }
        
        void PcPowerControl(Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey("msg"))
            {
                var msgValue = dictionary["msg"];
                if (msgValue == "off")
                {
                    WindowsCommand.Shutdown();
                }
            }
        }

        void SetVolumeEvent(Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey("msg"))
            {
                var msgValue = dictionary["msg"];
                if (msgValue == "off")
                {
                    WindowsCommand.SetVolume(1);
                }
                else
                {
                    int index = msgValue.LastIndexOf("#");
                    if (index != -1)
                    {
                        WindowsCommand.SetVolume(int.Parse(msgValue.Substring(index + 1)));
                    }
                }
            }
        }

        void SetScreenBrightnessEvent(Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey("msg"))
            {
                var msgValue = dictionary["msg"];
                if (msgValue == "off")
                {
                    WindowsCommand.TurnOffScreen();
                }
                else
                {
                    WindowsCommand.TurnOnScreen();
                    int index = msgValue.LastIndexOf("#");
                    if (index != -1)
                    {
                        WindowsCommand.SetScreenBrightness(int.Parse(msgValue.Substring(index + 1)));
                    }
                }

            }
        }

        void BluetoothEvent(Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey("msg"))
            {
                var msgValue = dictionary["msg"];
                if (msgValue == "off")
                {
                    WindowsCommand.StopBluetooth();
                }
                else if (msgValue == "on")
                {
                    WindowsCommand.StartBluetooth();
                }
            }
        }
    }
    
}
