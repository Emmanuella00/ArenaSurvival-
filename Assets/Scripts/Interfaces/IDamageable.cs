// ABSTRACTION: a contract for anything that can be hurt.
// Projectiles only know about this interface, not about Player or Enemy.
public interface IDamageable
{
    void TakeDamage(float amount);
}
