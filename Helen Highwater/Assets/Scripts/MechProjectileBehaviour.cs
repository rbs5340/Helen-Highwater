using UnityEngine;

public class MechProjectileBehaviour : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float flightSpeed = 10f;  // speed of movement
    public float lifetime = 5f;      // time before self-destruction

    private float direction = 1f;    // default: right

    public void Initialize(float dir)
    {
        direction = dir;
        Destroy(gameObject, lifetime); // destroy after set time
    }

    private void Update()
    {
        transform.position += Vector3.right * direction * flightSpeed * Time.deltaTime;
    }
}
