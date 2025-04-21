using UnityEngine;
using Mirror;
public class LazerProjectile : Weapon
{
    public Projectile projectilePrefab;
    public override void Attack()
    {
        Projectile proj = Instantiate(projectilePrefab, transform.position, transform.rotation);
        GameObject owner = transform.parent.gameObject;
        // Передача данных в снаряд
        proj.SetData(speed,  force,owner); // ← убедись, что `speed` НЕ 0
        proj.Initialize(transform.forward);
        NetworkServer.Spawn(proj.gameObject);
    }
}