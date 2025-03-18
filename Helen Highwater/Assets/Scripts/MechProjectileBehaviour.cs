using UnityEngine;

public class MechProjectileBehaviour : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float flightSpeed = 10f;  // speed of movement
    public float lifetime = 5f;      // time before self-destruction

    public float gravity = 0.001f; //gravity WIP
    private float direction = 1f;    // default: right
    private float fallSpeed = 0;

    public void Initialize(float dir)
    {
        direction = dir;
        Destroy(gameObject, lifetime); // destroy after set time
    }

    private void Update()
    {
        transform.position += new Vector3(1, Time.deltaTime * gravity, 0)* direction * flightSpeed * Time.deltaTime;
    }
}
