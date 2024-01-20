using System.Collections;
using System.Net.Sockets;
using System.Text;
using HiPcMijia.Coroutine;

namespace HiPcMijia.BemfaApi;

public class BemfaConnect
{
    public Socket tcpClientSocket;
    public string ip;
    public int port;
    public string key;
    public string topic;
    
    public BemfaConnect(BamfaConfig config, string topic, Action<Dictionary<string, string>> callBackAction = null)
    {
        this.ip = config.ip;
        this.port = config.port;
        this.key = config.key;
        this.topic = topic;
        Connect();
        Heartbeat();
        EventListener(callBackAction);
    }

    public void EventListener(Action<Dictionary<string, string>> callBackAction)
    {
        Task.Run(() => {
            while (true)
            {
                byte[] recvData = new byte[1024];
                int dataSize = tcpClientSocket.Receive(recvData);
                if (dataSize > 0)
                {
                    string decodedData = Encoding.UTF8.GetString(recvData, 0, dataSize).Trim();

                    Dictionary<string, string> parsedData = decodedData.Split('&')
                        .Select(part => part.Split('='))
                        .ToDictionary(split => split[0], split => split[1]);

                    callBackAction(parsedData);
                    Debug.Log($"{topic} 传来信号");
                }
                else
                {
                    Debug.Warning($"{topic} 连接发生错误");
                    Connect();
                }
            }
        });
    }
    
    public void Connect()
    {
        Debug.Log($"{topic} 连接");

        tcpClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        string serverIP = ip;
        int serverPort = port;
        try
        {
            // 连接服务器
            tcpClientSocket.Connect(serverIP, serverPort);
            //发送订阅指令
            string substr = $"cmd=1&uid={key}&topic={topic}\r\n";
            byte[] data = Encoding.UTF8.GetBytes(substr);
            tcpClientSocket.Send(data);
        }
        catch
        {
            Debug.Error($"{topic} 连接失败");
            Thread.Sleep(2000);
            Connect();
        }
    }
    
    public void Heartbeat()
    {
        try
        {
            // 发送心跳
            string keeplive = "ping\r\n";
            byte[] data = Encoding.UTF8.GetBytes(keeplive);
            tcpClientSocket.Send(data);
        }
        catch
        {
            Thread.Sleep(2000);
            Connect();
        }
    }
    
    public void SendMsg(string msg)
    {
        try
        {
            string substr = $"cmd=2&uid={key}&topic={topic}&msg={msg}\r\n";
            byte[] data = Encoding.UTF8.GetBytes(substr);
            tcpClientSocket.Send(data);
        }
        catch
        {
            Thread.Sleep(2000);
            Connect();
        }
    }
    
}