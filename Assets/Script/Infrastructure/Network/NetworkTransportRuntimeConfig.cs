using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

/// <summary>
/// 在 StartHost / StartServer / StartClient 之前配置 Unity Transport 的地址与端口。
/// 解决仅填 127.0.0.1 且 Server Listen 为空时，服务端只监听本机回环、局域网无法连入的问题。
/// 挂在与 <see cref="NetworkManager"/> 同一物体上（通常与 Unity Transport 同物体）。
/// </summary>
[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(NetworkManager))]
[DisallowMultipleComponent]
public class NetworkTransportRuntimeConfig : MonoBehaviour
{
    public enum NetworkRuntimeRole
    {
        /// <summary>不修改 Transport，沿用 Inspector 配置。</summary>
        UseInspectorDefaults = 0,
        /// <summary>本机为 Host（服务端 + 本地一客户端）。</summary>
        Host,
        /// <summary>纯服务端（无本地玩家客户端，需其他机器 StartClient）。</summary>
        Server,
        /// <summary>纯客户端。</summary>
        Client
    }

    [Header("角色（非 UseInspectorDefaults 时会在 Awake 写入 Transport）")]
    [SerializeField] private NetworkRuntimeRole role = NetworkRuntimeRole.UseInspectorDefaults;

    [Header("端口（与 Inspector 中 Unity Transport 一致，默认 7777）")]
    [SerializeField] private ushort port = 7777;

    [Header("Host / Server：客户端要连接的地址")]
    [Tooltip("本机作主机时，填写其他设备能访问到的 IP：局域网填写本机 IPv4；仅本机测试可填 127.0.0.1")]
    [SerializeField] private string serverAddressForClients = "127.0.0.1";

    [Header("Host / Server：监听绑定")]
    [Tooltip("接受局域网连接时使用 0.0.0.0；仅本机回环可用 127.0.0.1")]
    [SerializeField] private string serverListenBindAddress = "0.0.0.0";

    [Header("Client：要连接的服务端地址")]
    [SerializeField] private string clientConnectAddress = "127.0.0.1";

    [Header("可选：启动时自动调用 NetworkManager")]
    [SerializeField] private bool autoStart;

    private void Awake()
    {
        if (!TryGetComponent(out NetworkManager networkManager))
            return;
        if (!networkManager.TryGetComponent(out UnityTransport transport))
        {
            Debug.LogWarning("[NetworkTransportRuntimeConfig] 未找到 UnityTransport，跳过配置。");
            return;
        }

        ApplyCommandLineOverrides();

        if (role == NetworkRuntimeRole.UseInspectorDefaults)
        {
            if (autoStart)
                Debug.LogWarning("[NetworkTransportRuntimeConfig] autoStart 已勾选但 role 是 UseInspectorDefaults，不会自动启动。请改用 Host/Server/Client 或手动点击 NetworkManager 按钮。");
            return;
        }

        switch (role)
        {
            case NetworkRuntimeRole.Host:
            case NetworkRuntimeRole.Server:
                transport.SetConnectionData(serverAddressForClients, port, serverListenBindAddress);
                break;
            case NetworkRuntimeRole.Client:
                transport.SetConnectionData(clientConnectAddress, port, clientConnectAddress);
                break;
        }

        if (!autoStart)
            return;

        bool started = role switch
        {
            NetworkRuntimeRole.Host => networkManager.StartHost(),
            NetworkRuntimeRole.Server => networkManager.StartServer(),
            NetworkRuntimeRole.Client => networkManager.StartClient(),
            _ => false
        };

        if (!started)
            Debug.LogError($"[NetworkTransportRuntimeConfig] 自动启动失败：{role}。是否已运行其他 NetworkManager 或 Transport 已创建？");
    }

    private void ApplyCommandLineOverrides()
    {
        try
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                var a = args[i];
                if (a.StartsWith("-port=", StringComparison.OrdinalIgnoreCase) && ushort.TryParse(a.Substring(6), out var p))
                    port = p;
                else if (a.Equals("-port", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length && ushort.TryParse(args[i + 1], out var p2))
                    port = p2;

                if (a.StartsWith("-role=", StringComparison.OrdinalIgnoreCase))
                    TryParseRole(a.Substring(6));
                else if (a.Equals("-role", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                    TryParseRole(args[i + 1]);

                if (a.StartsWith("-server=", StringComparison.OrdinalIgnoreCase))
                    serverAddressForClients = a.Substring(8);
                else if (a.StartsWith("-client=", StringComparison.OrdinalIgnoreCase))
                    clientConnectAddress = a.Substring(8);
                else if (a.StartsWith("-listen=", StringComparison.OrdinalIgnoreCase))
                    serverListenBindAddress = a.Substring(8);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[NetworkTransportRuntimeConfig] 解析命令行失败：{e.Message}");
        }
    }

    private void TryParseRole(string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        if (value.Equals("host", StringComparison.OrdinalIgnoreCase))
            role = NetworkRuntimeRole.Host;
        else if (value.Equals("server", StringComparison.OrdinalIgnoreCase))
            role = NetworkRuntimeRole.Server;
        else if (value.Equals("client", StringComparison.OrdinalIgnoreCase))
            role = NetworkRuntimeRole.Client;
    }

    /// <summary>运行时启动 Host：绑定所有网卡（0.0.0.0），局域网客户端可通过本机 IP 连入。</summary>
    public bool StartAsHost()
    {
        if (!TryGetComponent(out NetworkManager networkManager)) return false;
        if (!networkManager.TryGetComponent(out UnityTransport transport))
        {
            Debug.LogWarning("[NetworkTransportRuntimeConfig] 未找到 UnityTransport");
            return false;
        }
        transport.SetConnectionData("0.0.0.0", port, "0.0.0.0");
        bool ok = networkManager.StartHost();
        if (!ok) Debug.LogError("[NetworkTransportRuntimeConfig] StartHost 失败");
        return ok;
    }

    /// <summary>运行时启动 Client：连接到指定局域网 IP 的主机。</summary>
    public bool StartAsClient(string serverIp)
    {
        if (!TryGetComponent(out NetworkManager networkManager)) return false;
        if (!networkManager.TryGetComponent(out UnityTransport transport))
        {
            Debug.LogWarning("[NetworkTransportRuntimeConfig] 未找到 UnityTransport");
            return false;
        }
        transport.SetConnectionData(serverIp, port);
        bool ok = networkManager.StartClient();
        if (!ok) Debug.LogError($"[NetworkTransportRuntimeConfig] StartClient 失败，目标={serverIp}:{port}");
        return ok;
    }
}
