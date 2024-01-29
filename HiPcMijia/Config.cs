using HiPcMijia.BemfaApi;
namespace HiPcMijia;

public class Config
{
    public BamfaConfig BemfaConfig { get; set; }
    public FunctionSetting FunctionSetting { get; set;}
    public DeveloperSetting DeveloperSetting { get; set; }
}

public class FunctionSetting
{
    public bool PcPower { get; set; }
    public string PcPowerName { get; set; }
    
    public bool PcVolume { get; set; }
    public string PcVolumeName { get; set; }
    
    public bool PcScreenBrightness { get; set; }
    public string PcScreenBrightnessName { get; set; }

    public bool PcBluetooth { get; set; }
    public string PcBluetoothName { get; set; }

    public int ReportingInterval { get; set; }
}

public class DeveloperSetting
{
    public string LogFilePath { get; set; }
    public string WarningFilePath { get; set; }
    public string ErrorFilePath { get; set; }
    
    public bool IsLogEnabled { get; set; }
    public bool IsWarningEnabled { get; set; }
    public bool IsErrorEnabled { get; set; }
    
    public bool AutoCleanuplogFile { get; set; }
    
    public bool AutoStartProcess { get; set;}
    public bool BlockMultipleInstances { get; set;}
}
