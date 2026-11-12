using UnityEngine;

// =====================================================================
// Pickup - base class for everything the player can collect.
//
// ABSTRACTION: abstract class. It handles what every pickup shares
//   (spinning, detecting the player, disappearing) and declares Apply()
//   as abstract, so each pickup MUST define its own effect.
// INHERITANCE: HealthPickup and AmmoPickup extend it.
// POLYMORPHISM: OnTriggerEnter calls Apply() without knowing which
//   kind of pickup it is.
// =====================================================================
public abstract class Pickup : MonoBehaviour
{
    [SerializeField] protected float amount = 25f;
    [SerializeField] private float spinSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);   // spin so it's easy to spot
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Only disappear if the effect was actually used (e.g. not at full health)
        if (Apply(other.gameObject)) Destroy(gameObject);
    }

    // POLYMORPHISM: each subclass decides what collecting it does
    protected abstract bool Apply(GameObject player);
}
