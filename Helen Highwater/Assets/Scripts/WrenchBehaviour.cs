using UnityEngine;
using System.Collections;
using System;

public class WrenchBehaviour : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float flightSpeed = 10f;  // speed of movement
    public float maxDistance = 5f;   // distance before returning
    public float pauseTime = 0.5f;   // time at peak before returning

    public Action OnDestroyCallback;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform player;
    private float direction = 1f; // default: right

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
        // move toward target position
        yield return MoveToTarget(targetPosition);

        // pause at peak
        yield return new WaitForSeconds(pauseTime);

        // return to player
        yield return MoveToTarget(player.position, true);

        Destroy(gameObject);
    }

    private IEnumerator MoveToTarget(Vector3 target, bool trackPlayer = false)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            if (trackPlayer)
            {
                target = player.position;
            }

            transform.position = Vector3.MoveTowards(transform.position, target, flightSpeed * Time.deltaTime);
            yield return null;
        }
    }
    private void OnDestroy()
    {
        OnDestroyCallback?.Invoke(); // Notify PlayerController
    }
}
