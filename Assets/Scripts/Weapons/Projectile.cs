using System.Collections.Generic;
using UnityEngine;

// ENCAPSULATION: damage, radius and pool are private; only Launch() sets them.
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifetime = 3f;

    private float damage;
    private float blastRadius;                 // 0 = single-target bullet
    private float timer;
    private Rigidbody rb;
    private ObjectPool<Projectile> pool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction, float damage, ObjectPool<Projectile> pool, float blastRadius = 0f)
    {
        this.damage = damage;
        this.pool = pool;
        this.blastRadius = blastRadius;
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
        if (blastRadius > 0f)
        {
            Explode();
        }
        else
        {
            // POLYMORPHISM through an interface: works on Player AND Enemy
            IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
            if (target != null) target.TakeDamage(damage);
        }
        ReturnToPool();
    }

    // Damage every IDamageable inside the blast radius, once each
    private void Explode()
    {
        HashSet<IDamageable> alreadyHit = new HashSet<IDamageable>();
        foreach (Collider c in Physics.OverlapSphere(transform.position, blastRadius))
        {
            if (c.CompareTag("Player")) continue;        // don't hurt yourself
            IDamageable target = c.GetComponentInParent<IDamageable>();
            if (target != null && alreadyHit.Add(target))
                target.TakeDamage(damage);
        }
    }

    private void ReturnToPool()
    {
        if (!gameObject.activeSelf) return;
        rb.linearVelocity = Vector3.zero;
        pool.Release(this);
    }
}
