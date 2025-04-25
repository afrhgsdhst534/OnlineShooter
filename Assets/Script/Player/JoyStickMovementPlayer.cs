using UnityEngine;
using Mirror;
public class JoyStickMovementPlayer : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 5f;
    PlayerMovement playerMovement;
    [Header("References")]
    public Joystick joystick; 
    private Rigidbody rb;
    private Vector3 input;
    private Quaternion targetRotation;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        speed = playerMovement.speed;
        rotationSpeed = playerMovement.rotationSpeed;
        rb = GetComponent<Rigidbody>();
        if (isLocalPlayer && joystick == null)
        {
            joystick = FindFirstObjectByType<Joystick>();
        }
    }
    public override void OnStartLocalPlayer()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            enabled = false;  // Отключаем этот скрипт для хоста/сервера
        }
    }
    void Update()
    {
        if (!isLocalPlayer || joystick == null) return;
        input = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
        if (joystick.Horizontal<=0&& joystick.Vertical <= 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
            if (input.sqrMagnitude > 0.01f)
        {
            Vector3 move = input.normalized * playerMovement.speed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
            targetRotation = Quaternion.LookRotation(input);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * playerMovement.rotationSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        rb.linearVelocity = input * speed;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }
}