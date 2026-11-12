using UnityEngine;

// STRATEGY PATTERN + ABSTRACTION: every gun is a swappable algorithm
// with its own rules. ShootController only knows this contract.
public interface IShootStrategy
{
    string Name { get; }
    float Cooldown { get; }      // seconds between shots
    int AmmoCost { get; }        // rounds used per shot
    void Shoot(Transform firePoint);
}
