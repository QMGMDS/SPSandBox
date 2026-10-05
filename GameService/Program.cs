using System.Net;
using System.Net.Sockets;

/// <summary>
/// 服务端监听地址配置，通过选择不同地址切换联机模式
/// </summary>
internal static class ServerAddresses
{
    /// <summary>
    /// 模式一：只监听回环地址，本机客户端使用此地址连接
    /// </summary>
    public const string LOOPBACK_ADDRESS = "127.0.0.1";

    /// <summary>
    /// 模式二：只监听指定局域网地址，本机和其他客户端都使用此地址连接
    /// 使用前填写本机网卡的实际 IPv4 地址，例如 192.168.1.100
    /// </summary>
    public const string LAN_ADDRESS = "192.168.82.205";

    /// <summary>
    /// 模式三：监听所有 IPv4 地址，包括回环地址和各网卡地址
    /// 客户端连接时使用服务端的实际地址，不使用 0.0.0.0
    /// </summary>
    public const string ANY_ADDRESS = "0.0.0.0";
}

internal class Program
{
    private const string LISTEN_ADDRESS = ServerAddresses.ANY_ADDRESS;
    private const int LISTEN_PORT = 8080;

    private static async Task Main(string[] args)
    {
        using CancellationTokenSource cancellationSource = new();
        List<Socket> clientSockets = new();
        ConsoleCancelEventHandler cancelHandler = (_, e) =>
        {
            /* e.Cancel = true 作用
                CancelKeyPress 事件在用户按下 Ctrl+C 时触发，默认行为是终止进程
                通过设置 e.Cancel = true 可以阻止进程立即终止，允许程序在退出前执行清理操作
            */
            e.Cancel = true;
            cancellationSource.Cancel();
        };
        Console.CancelKeyPress += cancelHandler;

        try
        {
            using Socket listenerSocket = CreateListenerSocket(LISTEN_ADDRESS, LISTEN_PORT, 4);
            Console.WriteLine($"服务端已监听 {listenerSocket.LocalEndPoint}，按 Ctrl+C 退出...");
            await AcceptClientsAsync(listenerSocket, clientSockets, cancellationSource.Token);
        }
        catch (OperationCanceledException) when (cancellationSource.IsCancellationRequested)
        {
            Console.WriteLine("服务端已收到用户的退出请求（Ctrl+C），正在关闭连接...");
        }
        catch (Exception e)
        {
            Console.WriteLine($"服务端运行失败：{e.Message}");
        }
        finally
        {
            Console.CancelKeyPress -= cancelHandler;
            foreach (Socket clientSocket in clientSockets)
            {
                clientSocket.Dispose();
            }
            Console.WriteLine("服务端已关闭所有客户端连接，按任意键退出...");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// 创建、绑定并启动监听套接字
    /// </summary>
    /// <param name="ipAddress">ip地址</param>
    /// <param name="port">端口号</param>
    /// <param name="backlog">等待接受的连接队列长度</param>
    private static Socket CreateListenerSocket(string ipAddress, int port, int backlog)
    {
        // 本服务要求等待队列长度为正数，避免使用无效的启动配置
        if (backlog <= 0)
        {
            // 抛出 "参数异常"，构造函数的传入参数分别代表：哪个参数有问题，以及问题是什么
            throw new ArgumentOutOfRangeException(nameof(backlog), "等待队列长度必须大于 0");
        }

        IPEndPoint ipPoint = new(IPAddress.Parse(ipAddress), port);
        Socket listenerSocket = new(ipPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listenerSocket.Bind(ipPoint);
            listenerSocket.Listen(backlog);
            return listenerSocket;
        }
        catch
        {
            // 初始化失败时，调用方拿不到套接字，必须在这里释放
            listenerSocket.Dispose();
            throw;
        }
    }

    /// <summary>
    /// 循环异步接受客户端连接
    /// </summary>
    private static async Task AcceptClientsAsync(Socket listenerSocket, List<Socket> clientSockets, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Socket clientSocket = await listenerSocket.AcceptAsync(cancellationToken);
            clientSockets.Add(clientSocket);
            Console.WriteLine($"客户端已连入：{clientSocket.RemoteEndPoint}");
        }
    }
}
