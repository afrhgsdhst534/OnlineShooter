using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    private float speed;
    private Vector3 direction;
    private int damage;

    [SyncVar] private Vector3 syncedPosition;

    public void SetData(float speed, int damage)
    {
        this.speed = speed;
        this.damage = damage;

    }

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
        syncedPosition = transform.position;
    }


    void Update()
    {
        if (isServer)
        {
            transform.position += direction * speed * Time.deltaTime;
            syncedPosition = transform.position;

        }
        else
        {
            transform.position = syncedPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damage);

            NetworkServer.Destroy(gameObject);
        }
    }
}
