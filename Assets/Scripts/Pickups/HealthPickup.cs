using UnityEngine;

// INHERITANCE: extends Pickup. POLYMORPHISM: overrides Apply() to heal.
public class HealthPickup : Pickup
{
    protected override bool Apply(GameObject player)
    {
        // Heal() returns false at full health, so the pickup stays for later
        return player.TryGetComponent(out Health health) && health.Heal(amount);
    }
}
