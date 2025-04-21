using Mirror;
public class Health : NetworkBehaviour
{
    public int expReward = 10; // ← Сколько EXP даёт враг при смерти

    private HealthBar hb;
     public int maxHealth;
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int curHealth;
    public override void OnStartServer()
    {
        curHealth = maxHealth;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        hb = GetComponent<HealthBar>();
    }
    void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (hb != null)
        {
            hb.SetHealth(newHealth, maxHealth);
        }
    }
    [Server]
    public void TakeDamage(int damage)
    {
        if (!isServer) return;
        curHealth -= damage;
        if (curHealth <= 0)
        {
            Die();
        }
    }
    [Server]
    private void Die()
    {
        if (ExpManager.Instance != null)
        {
            ExpManager.Instance.AddXP(expReward);
        }
        NetworkServer.Destroy(gameObject);
    }
}