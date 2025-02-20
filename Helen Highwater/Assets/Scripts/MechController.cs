using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.UIElements;

public class MechController : MonoBehaviour
{
    public int playerId = 0; // The Rewired player ID (for a single player game, should always be 0)
    private Rewired.Player player; // The Rewired Player


    //Player states for all actions so far. i have a different rising and falling state in case we want to change the gravity to make the platforming feel better,
    enum state
    {
        idle,
        run,
        rise,
        fall,
        damaged,
        wrenchThrow,
        dash
    }

    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpStrength = 3f;
    public float decelerationFactor = 0.9f;

    [Header("Attack Settings")]
    public GameObject ProjectilePrefab;
    public Transform attackSpawnPoint;
    public float attackRate = 0.3f;
    public float attackAnimationTimer;

    private bool isGrounded;
    private float attackTimer;

    private float direction;
    private state playerState;

    private void Start()
    {
        // Get Rewired Player
        player = ReInput.players.GetPlayer(playerId);

        // Get Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        playerState = state.idle;

        if (!rb)
        {
            Debug.LogError("No Rigidbody2D found on Player!");
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleAttack();

        //Logs player game state for testing purposes
        Debug.Log(playerState.ToString());

    }

    private float lastDirection = 1f; // 1 for right, -1 for left

    private void HandleMovement()
    {
        direction = player.GetAxis("MoveHorizontal");

        if (Mathf.Abs(direction) > 0.1f) // small dead zone to prevent jitter

        {
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
            lastDirection = Mathf.Sign(direction); // Update last facing direction

            if (isGrounded)
            {
                playerState = state.run;
            }
        }
        else
        {
            // Apply deceleration
            rb.velocity = new Vector2(rb.velocity.x * decelerationFactor, rb.velocity.y);

            if (rb.velocity.x == 0f && isGrounded)
            {
                playerState = state.idle;
            }
        }
    }

    private void HandleJumping()
    {
        if (player.GetButtonDown("Jump") && isGrounded)

        {
            rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
            isGrounded = false;
            playerState = state.rise;
        }
        if (rb.velocity.y < 0f)
        {
            playerState = state.fall;
        }
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime; // Decrease attack cooldown

        if (player.GetButton("Attack") && attackTimer <= 0f) // Fire while attack button is held
        {
            ShootProjectile();
            attackTimer = attackRate; // Reset attack cooldown
        }

        // If attacking, keep the state as wrenchThrow
        if (player.GetButton("Attack"))
        {
            playerState = state.wrenchThrow;
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(ProjectilePrefab, attackSpawnPoint.position, Quaternion.identity);
        MechProjectileBehaviour projectileScript = projectile.GetComponent<MechProjectileBehaviour>();

        if (projectileScript)
        {
            projectileScript.Initialize(lastDirection);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (Mathf.Abs(rb.velocity.x) > 0)
            {
                playerState = state.run;
            }
            else
            {
                playerState = state.idle;
            }
        }
    }
}
