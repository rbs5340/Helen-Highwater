using UnityEngine;

public class MechProjectileBehaviour : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float flightSpeed = 10f;  // speed of movement
    public float lifetime = 5f;      // time before self-destruction

    public LayerMask groundLayer;    // LayerMask for detecting ground

    private float direction = 1f;    // default: right
    private float fallSpeed = 0;

    public void Initialize(float dir)
    {
        direction = dir;
        Destroy(gameObject, lifetime); // destroy after set time
    }

    private void Update()
    {
        // Move forward and apply fall speed
        if (direction == 1f)
        {
            transform.position += new Vector3(1, fallSpeed * -1, 0) * direction * flightSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(1, fallSpeed, 0) * direction * flightSpeed * Time.deltaTime;
        }

        // Apply gravity-like effect
        fallSpeed += 2 * Time.deltaTime;

        // Check for collision with the ground
        if (Physics2D.OverlapCircle(transform.position, 0.1f, groundLayer))
        {
            Destroy(gameObject); // Destroy projectile upon ground collision
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If it hits the ground, destroy the projectile
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
