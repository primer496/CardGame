using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterPanel : BasePanel
{
    private Transform UIEnterButton;
    private Transform UICreateButton;
    private Transform UICancelButton;

    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TextMeshProUGUI hostIpText;

    // 标记是否已成功为 NetworkManager 注册回调（避免重复注册）
    private bool networkCallbacksRegistered = false;
    // 自定义消息名
    private const string AssignPlayerIdMsg = "AssignPlayerId";

    private void Awake()
    {
        UIEnterButton = transform.Find("JoinButton");
        UICreateButton = transform.Find("CreateButton");
        UICancelButton = transform.Find("CancelButton");

        UIEnterButton.GetComponent<Button>().onClick.AddListener(OnClickEnterButton);
        UICreateButton.GetComponent<Button>().onClick.AddListener(OnClickCreateButton);
        UICancelButton.GetComponent<Button>().onClick.AddListener(OnClickCancelButton);
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClickEnterButton()
    {
        string ip = ipInputField != null ? ipInputField.text.Trim() : string.Empty;
        if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
        var config = NetworkManager.Singleton?.GetComponent<NetworkTransportRuntimeConfig>();
        if (config != null)
            config.StartAsClient(ip);
        else
            NetworkManager.Singleton?.StartClient();
    }

    private void OnClickCancelButton()
    {
        UIManager.Instance.ClosePanel(UIConst.EnterPanel);
    }

    private void OnClickCreateButton()
    {
        var config = NetworkManager.Singleton?.GetComponent<NetworkTransportRuntimeConfig>();
        if (config != null)
            config.StartAsHost();
        else
            NetworkManager.Singleton?.StartHost();

        if (hostIpText != null)
            hostIpText.text = "IP: " + GetLocalLanIp();
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected with ID: {clientId}");
        SceneManager.LoadScene("GameScene");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected with ID: {clientId}");
    }

    // 获取本机局域网 IPv4（优先返回 192.168.x.x / 10.x.x.x / 172.x.x.x）
    private static string GetLocalLanIp()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up) continue;
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
            foreach (UnicastIPAddressInformation addr in ni.GetIPProperties().UnicastAddresses)
            {
                if (addr.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                string ip = addr.Address.ToString();
                if (ip.StartsWith("192.168.") || ip.StartsWith("10.") || ip.StartsWith("172."))
                    return ip;
            }
        }
        return "127.0.0.1";
    }
}
