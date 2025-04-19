using Mirror;
using UnityEngine;
public class PlayerPositionSync : NetworkBehaviour
{
    [SyncVar] public Vector3 syncedPosition;
    void Update()
    {
        if (isLocalPlayer)
        {
            if ((transform.position - syncedPosition).sqrMagnitude > 0.001f)
            {
                CmdSendPosition(transform.position);
            }
        }
    }
    [Command(channel = Channels.Unreliable)]
    void CmdSendPosition(Vector3 pos)
    {
        syncedPosition = pos;
        transform.position = pos;
    }
}