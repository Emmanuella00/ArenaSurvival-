using UnityEngine;

// =====================================================================
// Health - the player's hit points.
//
// ENCAPSULATION: currentHealth is private. The only ways to change it
//   are TakeDamage() and Heal(), which enforce the rules (never below 0,
//   never above max, no changes after death).
// ABSTRACTION / INTERFACE: implements IDamageable, so a Projectile or a
//   robot can hurt the player without knowing what a "Player" is.
// OBSERVER PATTERN (publisher): raises OnHealthChanged and OnDied.
//   UIManager and GameManager subscribe; Health never calls them directly.
// =====================================================================
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead;

    // Read-only access for other scripts (no setter = cannot be changed from outside)
    public float CurrentHealth { get { return currentHealth; } }
    public float MaxHealth { get { return maxHealth; } }

    // OBSERVER: events other scripts can subscribe to with +=
    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDied;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0f);   // never below 0
        OnHealthChanged?.Invoke(currentHealth);                    // notify subscribers

        if (currentHealth <= 0f)
        {
            isDead = true;
            OnDied?.Invoke();
        }
    }

    // Returns false when already at full health, so a HealthPickup is not wasted
    public bool Heal(float amount)
    {
        if (isDead || amount <= 0f || currentHealth >= maxHealth) return false;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);   // never above max
        OnHealthChanged?.Invoke(currentHealth);
        return true;
    }
}
