using UnityEngine;
using UnityEngine.AI;

// =====================================================================
// Enemy - base class for every robot.
//
// ABSTRACTION: abstract class - you can never create a plain "Enemy".
//   It defines what every robot has (health, sight, a current state) and
//   declares Attack() as abstract, so each robot MUST write its own.
// INHERITANCE: MeleeRobot and HeavyRobot extend this class and reuse all
//   of its movement, sight, damage and death logic.
// ENCAPSULATION: fields are protected/private; other scripts only get
//   read-only properties.
// INTERFACE: implements IDamageable so Projectiles can hurt it.
// STATE PATTERN (context): holds the current IEnemyState and hands its
//   Update to it. ChangeState() is the only way to switch states.
// OBSERVER: raises OnDied so WaveSpawner can count remaining enemies.
// =====================================================================
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackCooldown = 1.2f;
    [SerializeField] protected int scoreValue = 10;

    [Header("Senses")]
    [SerializeField] protected float sightRange = 20f;
    [SerializeField] protected float hearingRange = 8f;
    [SerializeField] private float eyeHeight = 1.5f;

    [Header("Links")]
    [SerializeField] private WaypointPath path;          // can be set in Inspector or by WaveSpawner
    [SerializeField] private Renderer bodyRenderer;      // flashes white when hit

    [Header("Loot")]
    [SerializeField] private Pickup[] dropPrefabs;
    [SerializeField, Range(0f, 1f)] private float dropChance = 0.3f;

    protected float health;
    protected NavMeshAgent agent;
    private IEnemyState state;
    private Color originalColor;
    private float flashTimer;

    // Read-only access for the states
    public NavMeshAgent Agent { get { return agent; } }
    public Transform Player { get; private set; }
    public float AttackRange { get { return attackRange; } }
    public float AttackCooldown { get { return attackCooldown; } }
    public string CurrentStateName { get { return state == null ? "None" : state.GetType().Name; } }
    public WaypointPath Path
    {
        get { return path; }
        set { path = value; }
    }

    // OBSERVER: WaveSpawner subscribes to know when this robot dies
    public event System.Action<Enemy> OnDied;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        health = maxHealth;
        if (bodyRenderer != null) originalColor = bodyRenderer.material.color;
    }

    protected virtual void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) Player = p.transform;

        if (state == null) ChangeState(new PatrolState());   // default starting state
    }

    private void Update()
    {
        if (state != null) state.Tick(this);   // STATE PATTERN: the state decides what happens
        UpdateFlash();
    }

    // STATE PATTERN: the single place where states are switched
    public void ChangeState(IEnemyState next)
    {
        if (state != null) state.Exit(this);
        state = next;
        state.Enter(this);
    }

    // Is the player within sight range AND not hidden behind a wall?
    public bool CanSeePlayer()
    {
        if (Player == null || DistanceToPlayer() > sightRange) return false;

        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 direction = (Player.position - eye).normalized;
        if (Physics.Raycast(eye, direction, out RaycastHit hit, sightRange))
            return hit.collider.CompareTag("Player");   // the first thing hit must be the player
        return false;
    }

    // Close enough to hear footsteps, even through walls
    public bool CanHearPlayer()
    {
        return DistanceToPlayer() <= hearingRange;
    }

    // Flat distance (ignores height differences)
    public float DistanceToPlayer()
    {
        if (Player == null) return Mathf.Infinity;
        Vector3 a = transform.position;
        Vector3 b = Player.position;
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    public void TakeDamage(float amount)
    {
        if (health <= 0f) return;

        health -= amount;
        Flash();

        if (health <= 0f)
        {
            Die();
        }
        else if (!(state is ChaseState) && !(state is AttackState))
        {
            ChangeState(new ChaseState());   // being shot makes the robot hunt you
        }
    }

    // POLYMORPHISM: each robot attacks differently
    public abstract void Attack();

    // Helper for subclasses: hurt the player through the IDamageable interface
    protected void DamagePlayer(float amount)
    {
        if (Player != null && Player.TryGetComponent(out IDamageable target))
            target.TakeDamage(amount);
    }

    private void Die()
    {
        if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);   // SINGLETON
        TryDropPickup();
        OnDied?.Invoke(this);                                                           // OBSERVER
        Destroy(gameObject);
    }

    private void TryDropPickup()
    {
        if (dropPrefabs == null || dropPrefabs.Length == 0) return;
        if (Random.value > dropChance) return;

        Pickup prefab = dropPrefabs[Random.Range(0, dropPrefabs.Length)];
        Instantiate(prefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
    }

    private void Flash()
    {
        if (bodyRenderer == null) return;
        bodyRenderer.material.color = Color.white;
        flashTimer = 0.1f;
    }

    private void UpdateFlash()
    {
        if (flashTimer <= 0f) return;
        flashTimer -= Time.deltaTime;
        if (flashTimer <= 0f && bodyRenderer != null) bodyRenderer.material.color = originalColor;
    }
}
