using UnityEngine;

// INHERITANCE: extends Pickup. POLYMORPHISM: overrides Apply() to refill ammo.
public class AmmoPickup : Pickup
{
    protected override bool Apply(GameObject player)
    {
        if (!player.TryGetComponent(out ShootController shooter)) return false;
        shooter.Ammo.AddAmmo(Mathf.RoundToInt(amount));   // AmmoSystem raises OnAmmoChanged -> HUD updates
        return true;
    }
}
