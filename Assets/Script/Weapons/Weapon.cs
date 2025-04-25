using UnityEngine;
public abstract class Weapon : MonoBehaviour
{
    public int force = 5;
    public float speed = 10f;
    public abstract void Attack();
}