using UnityEngine;

// =====================================================================
// AttackState - STATE PATTERN (concrete state).
//
// Stops, faces the player and calls enemy.Attack() on a cooldown.
// POLYMORPHISM: Attack() is abstract in Enemy, so MeleeRobot punches
//   and HeavyRobot slams - this state does not need to know which.
// Transition:
//   player moves out of range      -> ChaseState
//   (the 1.2x margin stops the robot flickering between Attack and Chase
//    when the player stands right on the edge of the range)
// =====================================================================
public class AttackState : IEnemyState
{
    private float nextAttackTime;

    public void Enter(Enemy enemy)
    {
        enemy.Agent.isStopped = true;
        nextAttackTime = Time.time + 0.3f;   // short wind-up before the first hit
    }

    public void Tick(Enemy enemy)
    {
        if (enemy.Player == null) return;

        if (enemy.DistanceToPlayer() > enemy.AttackRange * 1.2f)
        {
            enemy.ChangeState(new ChaseState());
            return;
        }

        // Turn to face the player (ignore height so the robot does not tilt)
        Vector3 look = enemy.Player.position - enemy.transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.01f) enemy.transform.rotation = Quaternion.LookRotation(look);

        if (Time.time >= nextAttackTime)
        {
            enemy.Attack();                                   // POLYMORPHISM
            nextAttackTime = Time.time + enemy.AttackCooldown;
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.Agent.isStopped = false;
    }
}
