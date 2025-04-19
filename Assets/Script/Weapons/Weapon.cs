using UnityEngine;
using Mirror;

public abstract class Weapon : MonoBehaviour
{
    public int reloadTime;
    public int force;
    public float speed;
    public abstract void Attack();
}
