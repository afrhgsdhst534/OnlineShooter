using UnityEngine;
using Mirror;
public class Health : NetworkBehaviour
{
    public int currentHealth;
    private int maxHealth;
    public int exp;
    public void SetMaxHealth(int value)
    {
        maxHealth = value;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
    public void TakeDamage(int amount)
    {
        if (!isServer) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            ExpManager.Instance.AddXP(exp);
            NetworkServer.Destroy(gameObject);
        }
    }
}