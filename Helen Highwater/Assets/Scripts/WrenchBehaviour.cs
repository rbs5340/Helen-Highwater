using UnityEngine;
using System.Collections;
using System;

public class WrenchBehaviour : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float flightSpeed = 10f;  // speed of movement
    public float maxDistance = 5f;   // distance before returning
    public float pauseTime = 0.5f;   // time at peak before returning

    public LayerMask groundLayer;    // LayerMask for detecting ground

    public Action OnDestroyCallback;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform player;
    private float direction = 1f; // default: right
    private bool hitGround = false;

    public void Initialize(float dir, Transform playerTransform)
    {
        direction = dir;
        player = playerTransform;
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.right * maxDistance * direction;

        StartCoroutine(BoomerangRoutine());
    }

    private IEnumerator BoomerangRoutine()
    {
        // Move toward target position, stop early if hitting ground
        yield return MoveToTarget(targetPosition);

        // Pause at peak or after hitting ground
        yield return new WaitForSeconds(pauseTime);

        // Return to player
        if (player == null) // Check if the player has been destroyed
        {
            Destroy(gameObject);
            yield break;
        }

        yield return MoveToTarget(player.position, true);

        Destroy(gameObject);
    }

    private IEnumerator MoveToTarget(Vector3 target, bool trackPlayer = false)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            if (player == null) // If player is destroyed, destroy the projectile
            {
                Destroy(gameObject);
                yield break;
            }

            if (trackPlayer)
            {
                target = player.position;
            }

            // Move forward
            transform.position = Vector3.MoveTowards(transform.position, target, flightSpeed * Time.deltaTime);

            // Check if we hit the ground
            if (!trackPlayer && Physics2D.OverlapCircle(transform.position, 0.1f, groundLayer))
            {
                hitGround = true;
                break; // Stop moving forward, enter pause state
            }

            yield return null;
        }
}

private void OnDestroy()
    {
        OnDestroyCallback?.Invoke(); // Notify PlayerController
    }
}
