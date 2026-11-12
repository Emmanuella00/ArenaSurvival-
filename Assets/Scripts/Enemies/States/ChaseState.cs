using UnityEngine;

// =====================================================================
// ChaseState - STATE PATTERN (concrete state).
//
// Runs at the player.
// Transitions:
//   close enough to hit            -> AttackState
//   lost sight for 5 seconds       -> PatrolState (resumes at nearest waypoint)
// =====================================================================
public class ChaseState : IEnemyState
{
    private const float GiveUpTime = 5f;
    private float lostSightTimer;

    public void Enter(Enemy enemy)
    {
        enemy.Agent.isStopped = false;
        lostSightTimer = 0f;
    }

    public void Tick(Enemy enemy)
    {
        if (enemy.Player == null) return;

        enemy.Agent.SetDestination(enemy.Player.position);

        if (enemy.DistanceToPlayer() <= enemy.AttackRange)
        {
            enemy.ChangeState(new AttackState());
            return;
        }

        if (enemy.CanSeePlayer())
        {
            lostSightTimer = 0f;
        }
        else
        {
            lostSightTimer += Time.deltaTime;
            if (lostSightTimer > GiveUpTime) enemy.ChangeState(new PatrolState());
        }
    }

    public void Exit(Enemy enemy) { }
}
