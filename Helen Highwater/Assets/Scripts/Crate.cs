using UnityEngine;

public class Crate : MonoBehaviour
{
    private enum CrateType
    {
        Wooden,
        Metal
    }

    [SerializeField] private CrateType crateType;

    private const int ProjectileLayer = 11;
    private const string MechTag = "Player - Mech";
    private const string MechProjectileTag = "Anchor";

    // Handle collision events
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(MechTag))
        {
            DestroyCrate();
        }
    }

    // Handle trigger events
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != ProjectileLayer) return;

        if (DestructibleByProjectile(collision))
        {
            DestroyCrate();
        }
    }

    // Determines if this crate can be destroyed by a projectile
    private bool DestructibleByProjectile(Collider2D collision)
    {
        return crateType == CrateType.Wooden || // Wooden crates always destructible
               (crateType == CrateType.Metal && collision.gameObject.CompareTag(MechProjectileTag)); // Metal crates only destructible by mech projectiles
    }

    // Method to destroy this crate when necessary
    private void DestroyCrate()
    {
        // Play the crate destroying sound effect
        AudioManager.Instance.PlaySoundEffect("crateDestroy");
        Destroy(gameObject);
    }
}
