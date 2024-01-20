using System.Net.Sockets;

namespace HiPcMijia.BemfaApi;

public class BamfaConfig
{
    public string ip { get; set; }
    public int port { get; set; }
    public string key { get; set; }
}