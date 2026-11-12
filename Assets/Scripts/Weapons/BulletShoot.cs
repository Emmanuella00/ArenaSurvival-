using UnityEngine;

// STRATEGY (concrete): fast single shots, low damage.
// Plain C# class - created with "new", not attached to an object.
public class BulletShoot : IShootStrategy
{
    private readonly ObjectPool<Projectile> pool;
    private readonly float damage = 10f;

    public string Name { get { return "Bullet"; } }
    public float Cooldown { get { return 0.15f; } }   // fast
    public int AmmoCost { get { return 1; } }

    public BulletShoot(Projectile bulletPrefab)
    {
        pool = new ObjectPool<Projectile>(bulletPrefab, 20);
    }

    public void Shoot(Transform firePoint)
    {
        Projectile p = pool.Get();
        // spawn slightly forward so we don't shoot ourselves
        p.transform.position = firePoint.position + firePoint.forward * 0.5f;
        p.transform.rotation = firePoint.rotation;
        p.Launch(firePoint.forward, damage, pool);
    }
}
