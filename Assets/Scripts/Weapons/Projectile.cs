using UnityEngine;

// ENCAPSULATION: damage and pool are private; only Launch() sets them.
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifetime = 3f;     // returns to pool if it hits nothing

    private float damage;
    private float timer;
    private Rigidbody rb;
    private ObjectPool<Projectile> pool;               // where to go back to

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction, float damage, ObjectPool<Projectile> pool)
    {
        this.damage = damage;
        this.pool = pool;
        timer = lifetime;
        rb.linearVelocity = direction * speed;                         
    }

    private void Update()
    {
        timer -= Time.deltaTime;                                                
        if (timer <= 0f) ReturnToPool();
    }

    private void OnCollisionEnter(Collision hit)
    {
        // POLYMORPHISM through an interface: works on Player AND Enemy
        IDamageable target = hit.collider.GetComponentInParent<IDamageable>();   
        if (target != null) target.TakeDamage(damage);                          
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (!gameObject.activeSelf) return;          // safety: never return twice
        rb.linearVelocity = Vector3.zero;
        pool.Release(this);                              // 5. hand it back
    }
}