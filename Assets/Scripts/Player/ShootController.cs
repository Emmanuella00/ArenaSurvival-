using UnityEngine;

// STRATEGY (context): holds the current gun as an interface and swaps it at runtime.
// POLYMORPHISM: Fire() behaves differently depending on the loaded strategy.
// ENCAPSULATION: strategies, ammo and timing are private; Ammo is read-only.
public class ShootController : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField] private Projectile rocketPrefab;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int startingReserve = 90;

    private IShootStrategy[] strategies;
    private IShootStrategy current;
    private AmmoSystem ammo;
    private float nextFireTime;

    // UI and AmmoPickup read this; they can't replace it (no setter)
    public AmmoSystem Ammo { get { return ammo; } }

    private void Awake()
    {
        ammo = new AmmoSystem(magazineSize, startingReserve);   // Awake, so the UI can subscribe in Start
    }

    private void Start()
    {
        if (bulletPrefab == null || rocketPrefab == null || firePoint == null)
            Debug.LogError("ShootController: drag FirePoint, Bullet and Rocket prefabs into the Inspector slots!");

        strategies = new IShootStrategy[]
        {
            new BulletShoot(bulletPrefab),
            new RocketShoot(rocketPrefab),
        };
        SetStrategy(0);
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime) Fire();   // hold to fire
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetStrategy(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetStrategy(1);
        if (Input.GetKeyDown(KeyCode.R)) ammo.Reload();
    }

    public void SetStrategy(int index)
    {
        if (index < 0 || index >= strategies.Length) return;
        current = strategies[index];
        Debug.Log("Weapon: " + current.Name);
    }

    private void Fire()
    {
        if (!ammo.TryUseAmmo(current.AmmoCost))
        {
            Debug.Log("Out of ammo! Press R to reload");
            nextFireTime = Time.time + 0.3f;
            return;
        }
        current.Shoot(firePoint);
        nextFireTime = Time.time + current.Cooldown;   // each gun has its own fire rate
    }
}
