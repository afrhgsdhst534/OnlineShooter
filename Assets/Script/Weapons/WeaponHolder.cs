using UnityEngine;
using Mirror;
using System.Threading.Tasks;

public class WeaponHolder : NetworkBehaviour
{
    public Weapon weapon;

    public override void OnStartServer()
    {
        base.OnStartServer();
        weapon = GetComponentInChildren<Weapon>();

        StartAttackLoop();
    }

    private async void StartAttackLoop()
    {
        while (true)
        {
            if (weapon == null) return;
            weapon.Attack();
            await Task.Delay(weapon.reloadTime);
        }
    }
}
