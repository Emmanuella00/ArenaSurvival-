using UnityEngine;

// STRATEGY (concrete): slow rockets with splash damage.
public class RocketShoot : IShootStrategy
{
    private readonly ObjectPool<Projectile> pool;
    private readonly float damage = 40f;
    private readonly float blastRadius = 3f;

    public string Name { get { return "Rocket"; } }
    public float Cooldown { get { return 1f; } }     // slow
    public int AmmoCost { get { return 5; } }        // expensive

    public RocketShoot(Projectile rocketPrefab)
    {
        pool = new ObjectPool<Projectile>(rocketPrefab, 5);
    }

    public void Shoot(Transform firePoint)
    {
        Projectile p = pool.Get();
        p.transform.position = firePoint.position + firePoint.forward * 0.5f;
        p.transform.rotation = firePoint.rotation;
        p.Launch(firePoint.forward, damage, pool, blastRadius);
    }
}
