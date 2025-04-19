using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class RoomManager : NetworkBehaviour
{
    public PlayerManager manager;
    public Button[] buttons;
    [Client]
    public override void OnStartClient()
    {
        if (isServer) return;
        buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        StartNet();
    }
    [Server]
    public override void OnStartServer()
    {
        StartNet();
    }
    [Server]
    private void StartNet()
    {
        closeButton.GetComponent<Image>().color = new Color(0, 255, 0);
        closeButton.GetComponentInChildren<Text>().text = "Room Opened";
        manager.onAdd += KickOut;
    }
    public Button closeButton;
    [SyncVar]
    private bool isClosed;
    [Server]
    public void ChangeRoomState()
    {
        isClosed = !isClosed;
        if (isClosed)
        {
            closeButton.GetComponent<Image>().color = new Color(255,0,0);
            closeButton.GetComponentInChildren<Text>().text = "Room Closed";

        }
        else
        {
            closeButton.GetComponent<Image>().color = new Color(0, 255, 0);
            closeButton.GetComponentInChildren<Text>().text = "Room Opened";
        }
    }
    [Server]
    private void KickOut(GameObject obj)
    {
        if (!isClosed) return;
        NetworkConnectionToClient conn = obj.GetComponent<NetworkIdentity>().connectionToClient;
        if (conn != null)
        {
            conn.Disconnect();
        }
    }
}