using UnityEngine;
using Mirror;
[RequireComponent(typeof(Rigidbody))]
public class Enemy : NetworkBehaviour
{
    [SyncVar] private NetworkIdentity targetIdentity;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] protected GameObject playerGO;
    private Rigidbody rb;
    private Vector3 movement;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        if (isServer)
        {
            playerGO = PlayerManager.instance.RandomPlayer();
            if (playerGO != null)
            {
                targetIdentity = playerGO.GetComponent<NetworkIdentity>();
            }
        }
    }
    void Update()
    {
        if (targetIdentity == null) return;
        PlayerPositionSync sync = targetIdentity.GetComponent<PlayerPositionSync>();
        if (sync == null) return;
        Vector3 targetPosition = sync.syncedPosition;
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion finalRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);
        }
        movement = direction.normalized;
    }
    void FixedUpdate()
    {
        if (targetIdentity == null) return;
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }
}