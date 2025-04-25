using UnityEngine;
using Mirror;
public class Projectile : NetworkBehaviour
{
    private float speed;
    private GameObject owner; 
    private Vector3 direction;
    private int damage;
    [SyncVar] private Vector3 syncedPosition;
    public void SetData(float speed, int damage,GameObject owner)
    {
        this.owner = owner;
        this.speed = speed;
        this.damage = damage;
    }
    public void Initialize(Vector3 dir)
    {
        Destroy(gameObject, 2);
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
        bool enemyHitsPlayer = other.gameObject.CompareTag("Player") && owner.CompareTag("Enemy");
        bool playerHitsEnemy = other.gameObject.CompareTag("Enemy") && owner.CompareTag("Player");
        if (enemyHitsPlayer || playerHitsEnemy)
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            NetworkServer.Destroy(gameObject);
        }
    }
}