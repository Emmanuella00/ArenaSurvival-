using UnityEngine;

// =====================================================================
// HeavyRobot - slow, tough, area slam.
//
// INHERITANCE: extends Enemy, only changes stats and the attack.
// POLYMORPHISM: same Attack() call as MeleeRobot, different behaviour.
// =====================================================================
public class HeavyRobot : Enemy
{
    [SerializeField] private float slamDamage = 25f;
    [SerializeField] private float slamRadius = 3f;

    // Reset() runs when the component is first added in the Editor:
    // gives the Heavy its own default stats instead of the Enemy defaults.
    private void Reset()
    {
        maxHealth = 150f;
        moveSpeed = 2f;
        attackRange = 2.5f;
        attackCooldown = 2.5f;
        scoreValue = 30;
    }

    public override void Attack()
    {
        // Area attack: hits the player anywhere inside the slam radius
        if (DistanceToPlayer() <= slamRadius) DamagePlayer(slamDamage);
        Debug.Log(name + " SLAMS the ground!");
    }
}
