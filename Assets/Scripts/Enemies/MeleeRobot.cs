using UnityEngine;

// =====================================================================
// MeleeRobot - fast, close-range puncher.
//
// INHERITANCE: extends Enemy and reuses all its movement, sight, damage
//   and death logic. It only adds what is different: how it attacks.
// POLYMORPHISM: overrides the abstract Attack().
// =====================================================================
public class MeleeRobot : Enemy
{
    [SerializeField] private float punchDamage = 10f;

    public override void Attack()
    {
        DamagePlayer(punchDamage);
        Debug.Log(name + " punches for " + punchDamage);
    }
}
