using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkTest : MonoBehaviour
{
    [Header("UI 组件绑定")]
    [SerializeField] private TMP_InputField _messiageInput;
    [SerializeField] private Button _sendButton;

    [Header("配置连接信息")]
    [SerializeField] private string _serverAddress = "127.0.0.1";
    [SerializeField] private int _serverPort = 8080;

    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private async void Start()
    {
        try
        {
            await ConnectAsync();
        }
        catch (Exception e)
        {
            CloseConnection();
            Debug.LogError($"连接服务器失败：{e.Message}");
        }
    }

    private void OnDestroy()
    {
        CloseConnection();
    }

    /// <summary>
    /// 异步连接服务端
    /// </summary>
    private async Task ConnectAsync()
    {
        Socket connectingSocket = _socket;
        try
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(_serverAddress), _serverPort);
            await connectingSocket.ConnectAsync(ipPoint);
            Debug.Log($"已连接服务端：{connectingSocket.RemoteEndPoint}");
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 关闭连接并释放套接字
    /// </summary>
    private void CloseConnection()
    {
        Socket socket = _socket;
        _socket = null;

        socket.Shutdown(SocketShutdown.Both);
        socket.Dispose();
    }

    private void ConfirmSendMessiage()
    {
        string sendMessiage = _messiageInput.text.Trim();

        if (string.IsNullOrEmpty(sendMessiage))
        {
            Debug.Log("请输入消息...");
            return;
        }

        Debug.Log($"发送消息：{sendMessiage}");
    }
}
