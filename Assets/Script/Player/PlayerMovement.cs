using UnityEngine;
using Mirror;
public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float tiltAngle = 20f;
    public float rotationSpeed = 5f;
    private Vector3 moveInput;
    private Rigidbody rb;
    private Quaternion targetRotation;
    private bool isMoving;
    [SerializeField] private Material material;
    [SerializeField] private AudioSource source;
    [SyncVar] public Vector3 syncedPosition;
    private Vector3 lastPosition;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
        rb.isKinematic = !isLocalPlayer;
        rb.freezeRotation = true;
        if (isLocalPlayer && material == null)
        {
            var renderer = GetComponentInChildren<MeshRenderer>();
            material = renderer.materials[1];
        }
    }
    void Update()
    {
        if (!isLocalPlayer) return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(h, 0f, v).normalized;
        float tiltZ = -h * tiltAngle;
        float tiltY = 0f;
        if (v != 0)
        {
            if (h > 0) tiltY = tiltAngle;
            if (h < 0) tiltY = -tiltAngle;
        }
        targetRotation = Quaternion.Euler(0f, tiltY, tiltZ);
        bool nowMoving = moveInput.sqrMagnitude > 0.01f;
        if (nowMoving != isMoving)
        {
            isMoving = nowMoving;
            CmdSetMovingState(isMoving);
        }
        if ((transform.position - lastPosition).sqrMagnitude > 0.01f)
        {
            CmdSendPosition(transform.position);
            lastPosition = transform.position;
        }
    }
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        rb.linearVelocity = moveInput * speed;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }
    [Command(channel = Channels.Unreliable)]
    void CmdSendPosition(Vector3 pos)
    {
        syncedPosition = pos;
        transform.position = pos;
    }
    [Command]
    void CmdSetMovingState(bool isMove)
    {
        RpcUpdateEffects(isMove);
    }
    [ClientRpc]
    void RpcUpdateEffects(bool moving)
    {
        if (!isLocalPlayer) return;

        if (moving)
        {
            material.EnableKeyword("_EMISSION");
            if (!source.isPlaying)
            {
                source.Play();
            }
        }
        else
        {
            material.DisableKeyword("_EMISSION");
            if (source.isPlaying) source.Stop();
        }
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isServer)
            PlayerManager.instance.AddPlayer(gameObject);
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isServer)
            PlayerManager.instance.RemovePlayer(gameObject);
    }
}