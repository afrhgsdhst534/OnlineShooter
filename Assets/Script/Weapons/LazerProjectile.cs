using UnityEngine;
using Mirror;
public class LazerProjectile : Weapon
{
    public Projectile projectilePrefab;
    public override void Attack()
    {
        Projectile proj = Instantiate(projectilePrefab, transform.position, transform.rotation);
        GameObject owner = transform.parent.gameObject;
        proj.SetData(speed,  force,owner);
        proj.Initialize(transform.forward);
        NetworkServer.Spawn(proj.gameObject);
    }
}