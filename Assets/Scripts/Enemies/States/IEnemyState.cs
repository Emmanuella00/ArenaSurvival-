// =====================================================================
// IEnemyState - STATE PATTERN contract.
//
// Every AI mode (Patrol, Chase, Attack) is its own class with these
// three methods. Enemy (the "context") keeps one current state and
// hands its Update to it. Each state decides when to switch to another.
// ABSTRACTION: Enemy only knows this interface, never the concrete states'
// internal logic.
// =====================================================================
public interface IEnemyState
{
    void Enter(Enemy enemy);   // runs once when the state starts
    void Tick(Enemy enemy);    // runs every frame while active
    void Exit(Enemy enemy);    // runs once when leaving the state
}
