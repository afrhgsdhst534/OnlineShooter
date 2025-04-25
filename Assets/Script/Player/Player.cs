using Mirror;
using UnityEngine;
public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnMaxHealthChanged))] public int maxHealth ;
    [SyncVar(hook = nameof(OnAttackChanged))] public int attack ;
    [SyncVar(hook = nameof(OnReloadTimeChanged))] public int reloadTime ;
    private Health health;
    private WeaponHolder weaponHolder;
    private Joystick joystick;
    public override void OnStartLocalPlayer()
    {
        health = GetComponent<Health>();
        weaponHolder = GetComponent<WeaponHolder>();
        if (isLocalPlayer && joystick == null)
        {
            joystick = FindFirstObjectByType<Joystick>();
        }
        ApplyStats();
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (Time.timeScale <= 0)
            joystick.gameObject.SetActive(false);
        else
            joystick.gameObject.SetActive(true);
    }
    void OnMaxHealthChanged(int oldVal, int newVal)
    {
        if (isLocalPlayer && health != null)
            health.SetMaxHealth(newVal);
    }
    void OnAttackChanged(int oldVal, int newVal)
    {
        if (isLocalPlayer && weaponHolder != null)
            weaponHolder.SetAttack(newVal);
    }
    void OnReloadTimeChanged(int oldVal, int newVal)
    {
        if (isLocalPlayer && weaponHolder != null)
            weaponHolder.SetReloadTime(newVal);
    }
    public void ApplyStats()
    {
        if (health != null) health.SetMaxHealth(maxHealth);
        if (weaponHolder != null)
        {
            weaponHolder.SetAttack(attack);
            weaponHolder.SetReloadTime(reloadTime);
        }
    }
}