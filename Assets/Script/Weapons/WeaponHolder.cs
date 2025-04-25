using UnityEngine;
using Mirror;
using System.Threading.Tasks;
public class WeaponHolder : NetworkBehaviour
{
    public int reloadTime = 1000;
    public Weapon weapon;
    public override void OnStartServer()
    {
        base.OnStartServer();
        weapon = GetComponentInChildren<Weapon>();
        StartAttackLoop();
    }
    private async void StartAttackLoop()
    {
        while (weapon!=null)
        {
            if (Time.timeScale <= 0) return;
            weapon.Attack();
            await Task.Delay(Mathf.Max(100, reloadTime));
        }
    }
    public void SetAttack(int value)
    {
        weapon.force = value;
    }
    public void SetReloadTime(int value)
    {
        reloadTime = value;
    }
}