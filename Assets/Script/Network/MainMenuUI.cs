using UnityEngine;
using UnityEngine.UI;
using Mirror;
using kcp2k;
using System;
public class MainMenuUI : MonoBehaviour
{
    public InputField portField;
    public InputField ipField;
    public void Start()
    {
        string localIP = GetLocalIPAddress();
        if (!string.IsNullOrEmpty(localIP))
        {
            Debug.Log("Локальный IPv4-адрес: " + localIP);
        }
        else
        {
            Debug.Log("Не удалось найти локальный IPv4-адрес.");
        }
        ipField.text = localIP;
    }
    public void StartHost()
    {
        NetworkManager.singleton.GetComponent<KcpTransport>().port = Convert.ToUInt16(portField.text);
        NetworkManager.singleton.networkAddress = ipField.text;
        NetworkManager.singleton.StartHost();
    }
    public void StartClient()
    {
        NetworkManager.singleton.GetComponent<KcpTransport>().port = Convert.ToUInt16(portField.text);
        NetworkManager.singleton.networkAddress = ipField.text;
        NetworkManager.singleton.StartClient();
    }
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
}