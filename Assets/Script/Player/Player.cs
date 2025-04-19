using UnityEngine;
using Mirror;
[RequireComponent(typeof(PlayerPositionSync))]
public abstract class Player : NetworkBehaviour
{
    [SerializeField]
    private PlayerManager playerManager;
    public override void OnStartServer()
    {
        playerManager = PlayerManager.instance;
        playerManager.AddPlayer(gameObject);
    }
    private void OnDestroy()
    {
        if (isServer && PlayerManager.instance != null)
        {
            PlayerManager.instance.RemovePlayer(gameObject);
        }
    }
}