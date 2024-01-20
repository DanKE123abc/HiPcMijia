using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
namespace HiPcMijia;

//copy from https://github.com/luojunyuan/dotNetPlayground/blob/master/BrightnessAdjust/AdjustScreenBuilder.cs
public class AdjustScreenByWmi
{
    // Store array of valid level values
    private readonly byte[] _brightnessLevels;

    // Define scope (namespace)
    readonly ManagementScope _scope = new ManagementScope("root\\WMI");

    // Define querys
    readonly SelectQuery _query = new SelectQuery("WmiMonitorBrightness");
    readonly SelectQuery _queryMethods = new SelectQuery("WmiMonitorBrightnessMethods");

    public bool IsSupported { get; set; }

    public AdjustScreenByWmi()
    {
        //get the level array for this system
        _brightnessLevels = GetBrightnessLevels();
        if (_brightnessLevels.Length == 0)
        {
            //"WmiMonitorBrightness" is not supported by the system
            IsSupported = false;
        }
        else
        {
            IsSupported = true;
        }
    }

    public void IncreaseBrightness()
    {
        if (IsSupported)
        {
            StartupBrightness(GetBrightness() + 10);
        }
    }

    public void DecreaseBrightness()
    {
        if (IsSupported)
        {
            StartupBrightness(GetBrightness() - 10);
        }
    }

    /// <summary>
    /// Returns the current brightness setting
    /// </summary>
    /// <returns></returns>
    public int GetBrightness()
    {
        using ManagementObjectSearcher searcher = new(_scope, _query);
        using ManagementObjectCollection objCollection = searcher.Get();

        byte curBrightness = 0;
        foreach (var o in objCollection)
        {
            var obj = (ManagementObject)o;
            curBrightness = (byte)obj.GetPropertyValue("CurrentBrightness");
            break;
        }

        return curBrightness;
    }

    /// <summary>
    /// Convert the brightness percentage to a byte and set the brightness using SetBrightness()
    /// </summary>
    /// <param name="iPercent"></param>
    private void StartupBrightness(int iPercent)
    {
        // XXX: ...
        if (iPercent < 0)
        {
            iPercent = 0;
        }
        else if (iPercent > 100)
        {
            iPercent = 100;
        }

        // iPercent is in the range of brightnessLevels
        if (iPercent <= _brightnessLevels[^1])
        {
            // Default level 100
            byte level = 100;
            foreach (byte item in _brightnessLevels)
            {
                // 找到 brightnessLevels 数组中与传入的 iPercent 接近的一项
                if (item >= iPercent)
                {
                    level = item;
                    break;
                }
            }

            SetBrightness(level);
        }
    }

    /// <summary>
    /// Set the brightness level to the targetBrightness
    /// </summary>
    /// <param name="targetBrightness"></param>
    public void SetBrightness(byte targetBrightness)
    {
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher(_scope, _queryMethods);
        using ManagementObjectCollection objectCollection = searcher.Get();
        foreach (var o in objectCollection)
        {
            var mObj = (ManagementObject)o;
            // Note the reversed order - won't work otherwise!
            mObj.InvokeMethod("WmiSetBrightness", new object[] { uint.MaxValue, targetBrightness });
            // Only work on the first object
            break;
        }
    }

    public byte[] GetBrightnessLevels()
    {
        // Output current brightness
        using ManagementObjectSearcher mos = new(_scope, _query);

        byte[] bLevels = Array.Empty<byte>();
        try
        {
            using ManagementObjectCollection moc = mos.Get();
            foreach (var managementBaseObject in moc)
            {
                var o = (ManagementObject)managementBaseObject;
                bLevels = (byte[])o.GetPropertyValue("Level");
                // Only work on the first object
                break;
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.Message);
        }

        return bLevels;
    }
}