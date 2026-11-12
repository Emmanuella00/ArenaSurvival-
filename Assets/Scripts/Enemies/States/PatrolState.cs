using UnityEngine;

// =====================================================================
// PatrolState - STATE PATTERN (concrete state).
//
// Walks the WaypointPath (ALGORITHM 1). Starts from the NEAREST waypoint
// (O(n) search), then moves to the next one in a loop (O(1) each step).
// Transitions:
//   sees or hears the player       -> ChaseState
//   patrols too long without contact -> ChaseState (hunts the player,
//   so waves can never get stuck)
// =====================================================================
public class PatrolState : IEnemyState
{
    private const float ArriveDistance = 0.6f;
    private const float MaxPatrolTime = 15f;

    private int waypointIndex;
    private float patrolTimer;

    public void Enter(Enemy enemy)
    {
        enemy.Agent.isStopped = false;
        patrolTimer = 0f;

        if (enemy.Path != null && enemy.Path.Count > 0)
        {
            // ALGORITHM 1, part 2: resume from the closest point, not always point 0
            waypointIndex = enemy.Path.GetNearestIndex(enemy.transform.position);
            enemy.Agent.SetDestination(enemy.Path.GetPoint(waypointIndex));
        }
    }

    public void Tick(Enemy enemy)
    {
        patrolTimer += Time.deltaTime;

        // Decision: switch to chasing?
        if (enemy.CanSeePlayer() || enemy.CanHearPlayer() || patrolTimer > MaxPatrolTime)
        {
            enemy.ChangeState(new ChaseState());
            return;
        }

        if (enemy.Path == null || enemy.Path.Count == 0) return;

        // Reached the current waypoint? Go to the next one.
        if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance < ArriveDistance)
        {
            // ALGORITHM 1, part 1: next index with wrap-around, O(1)
            waypointIndex = enemy.Path.GetNextIndex(waypointIndex);
            enemy.Agent.SetDestination(enemy.Path.GetPoint(waypointIndex));
        }
    }

    public void Exit(Enemy enemy) { }
}
