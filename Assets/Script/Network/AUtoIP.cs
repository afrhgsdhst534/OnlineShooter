using UnityEngine;
public class AUtoIP : MonoBehaviour
{
    void Start()
    {
        string localIP = GetLocalIPAddress();
        if (!string.IsNullOrEmpty(localIP))
        {
            Debug.Log("��������� IPv4-�����: " + localIP);
        }
        else
        {
            Debug.Log("�� ������� ����� ��������� IPv4-�����.");
        }
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