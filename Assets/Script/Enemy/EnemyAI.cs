using UnityEngine;
using Mirror;
[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : NetworkBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private NetworkIdentity targetIdentity;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        if (isServer)
        {
            GameObject player = PlayerManager.instance?.RandomPlayer();
            if (player != null)
            {
                targetIdentity = player.GetComponent<NetworkIdentity>();
            }
        }
    }
    void Update()
    {
        if (!isServer || targetIdentity == null) return;
        PlayerMovement player = targetIdentity.GetComponent<PlayerMovement>();
        if (player == null) return;
        Vector3 targetPos = player.syncedPosition;
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }
        moveDirection = dir.normalized;
    }
    void FixedUpdate()
    {
        if (!isServer || targetIdentity == null) return;
        rb.MovePosition(transform.position + moveDirection * speed * Time.fixedDeltaTime);
    }
}