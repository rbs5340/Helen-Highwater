using UnityEngine;

public class MechProjectileBehaviour : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float flightSpeed = 10f;  // speed of movement
    public float lifetime = 5f;      // time before self-destruction

    private float direction = 1f;    // default: right
    private float fallSpeed = 0;

    public void Initialize(float dir)
    {
        direction = dir;
        Destroy(gameObject, lifetime); // destroy after set time
    }

    private void Update()
    {
        if (direction == 1f)
        {
            transform.position += new Vector3(1, fallSpeed * -1, 0) * direction * flightSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(1, fallSpeed, 0) * direction * flightSpeed * Time.deltaTime;
        }
        fallSpeed += 2 * Time.deltaTime;
        Debug.Log(fallSpeed);
    }
}
