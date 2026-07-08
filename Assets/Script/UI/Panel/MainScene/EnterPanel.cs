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

    // 鏍囪鏄惁宸叉垚鍔熶负 NetworkManager 娉ㄥ唽鍥炶皟锛堥伩鍏嶉噸澶嶆敞鍐岋級
    private bool networkCallbacksRegistered = false;
    // 鑷畾涔夋秷鎭悕
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
        Debug.Log($"[EnterPanel] OnEnable: NM.Singleton={NetworkManager.Singleton != null}, IsListening={NetworkManager.Singleton?.IsListening}");
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
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
        Debug.Log($"[EnterPanel] OnClickCreate: NM.Singleton={NetworkManager.Singleton != null}");
        var config = NetworkManager.Singleton?.GetComponent<NetworkTransportRuntimeConfig>();
        if (config != null)
        {
            Debug.Log("[EnterPanel] 调用 StartAsHost");
            config.StartAsHost();
        }
        else
        {
            Debug.Log("[EnterPanel] 无 config, 直接 StartHost");
            NetworkManager.Singleton?.StartHost();
        }

        if (hostIpText != null)
            hostIpText.text = "IP: " + GetLocalLanIp();
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[EnterPanel] Client connected: {clientId}, 即将加载 GameScene");
        SceneManager.LoadScene("GameScene");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[EnterPanel] Client disconnected: {clientId}");
    }

    // 鑾峰彇鏈満灞€鍩熺綉 IPv4锛堜紭鍏堣繑鍥� 192.168.x.x / 10.x.x.x / 172.x.x.x锛�
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
