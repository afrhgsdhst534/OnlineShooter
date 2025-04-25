using UnityEngine;
using UnityEngine.UI;
using Mirror;
using kcp2k;
public class MainMenuUI : MonoBehaviour
{
    public InputField portField;
    public InputField ipField;
    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        string localIP = GetLocalIPAddress();
        if (!string.IsNullOrEmpty(localIP))
        {
            Debug.Log("��������� IPv4-�����: " + localIP);
            ipField.text = localIP;
        }
        else
        {
            Debug.Log("�� ������� ����� ��������� IPv4-�����.");
        }
#else
        ipField.text = "";
#endif
    }
    public void StartHost()
    {
        ushort port = TryParsePort();
        NetworkManager.singleton.GetComponent<KcpTransport>().port = port;
        NetworkManager.singleton.networkAddress = ipField.text;
        NetworkManager.singleton.StartHost();
    }
    public void StartClient()
    {
        ushort port = TryParsePort();
        NetworkManager.singleton.GetComponent<KcpTransport>().port = port;
        NetworkManager.singleton.networkAddress = ipField.text;
        NetworkManager.singleton.StartClient();
    }
    private ushort TryParsePort()
    {
        ushort port = 7777;
        ushort.TryParse(portField.text, out port);
        return port;
    }
#if UNITY_EDITOR || UNITY_STANDALONE
    string GetLocalIPAddress()
    {
        string localIP = string.Empty;
        foreach (var networkInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
            {
                var ipProperties = networkInterface.GetIPProperties();
                foreach (var address in ipProperties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = address.Address.ToString();
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(localIP))
                {
                    break;
                }
            }
        }
        return localIP;
    }
#endif
}