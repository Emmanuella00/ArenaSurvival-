# ArenaSurvival

A first-person wave survival shooter in Unity: hold a walled arena against waves of robots with two very different weapons and one shared supply of ammunition.

## Features

- **Two weapons, one magazine:** bullets cost 1 round and hit a single target for 10; rockets cost 5 rounds and hit everything within 3 metres for 40. Switch with `1` and `2`.
- **Robots that hunt you:** they patrol a waypoint route until they see you within 20 metres or hear you within 8, then chase and attack. Shoot one and it comes after you.
- **Two robot types:** melee robots are fast and punch for 10; heavy robots are slow, four times tougher, and slam everything within 3 metres for 25.
- **It gets harder:** wave 1 sends 3 robots, wave 2 sends 5, and heavy robots go from a quarter of the wave to half.
- **Fair spawning:** every robot appears at the point farthest from you, so nothing ever materialises at your back.
- **Loot:** 6 in 10 defeated robots drop a health pack (+25) or an ammo box (+30 rounds).

## Get Started

1. Clone the repository:

```
git clone https://github.com/Emmanuella00/ArenaSurvival-.git
```

2. Open the folder in Unity 6.
3. Open `Assets/Scenes/Arena.unity` and press Play.

> If input throws errors on the first run, set **Edit → Project Settings → Player → Active Input Handling** to **Both**.

## How to Play

Survive every wave. You start with 100 health and lose when it reaches zero; clear the last wave and you win.

| Action | Keys |
|---|---|
| Move | `W` `A` `S` `D` |
| Sprint | hold `Shift` |
| Jump | `Space` |
| Aim | mouse |
| Fire | hold left mouse button |
| Bullet weapon | `1` |
| Rocket weapon | `2` |
| Reload | `R` |
| Release the cursor | `Esc` |

**Tips**

- Save rockets for groups. They cost five rounds, so firing one at a single robot wastes four.
- Reload between waves, not when the magazine runs dry with robots closing in.
- Robots hear you within 8 metres even through walls, so a wall hides you from sight but not from sound.
- Pickups only disappear when they are useful, so a health pack left on the ground at full health will still be there later.

## Project structure
 
```
Assets/Scripts/
├── Player/       PlayerMovement, PlayerLook, Health, ShootController
├── Weapons/      IShootStrategy, BulletShoot, RocketShoot, Projectile, AmmoSystem
├── Enemies/      Enemy (abstract), MeleeRobot, HeavyRobot, WaypointPath
│   └── States/   IEnemyState, PatrolState, ChaseState, AttackState
├── Pickups/      Pickup (abstract), HealthPickup, AmmoPickup
├── Managers/     GameManager, UIManager, WaveSpawner
├── Interfaces/   IDamageable
└── Utilities/    ObjectPool
```
## Credits

- Wall texture: [Concrete034](https://ambientcg.com) by ambientCG (CC0)
- Floor texture: [PavingStones151](https://ambientcg.com) by ambientCG (CC0)
