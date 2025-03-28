using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.UIElements;
using System;

public class PlayerController : MonoBehaviour
{
    public int playerId = 0;
    private Rewired.Player player;
    private int helenRunID;
    public Animator animator;

    enum state
    {
        idle,
        run,
        rise,
        fall,
        damaged,
        wrenchThrow,
        dash,
        parry
    }

    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Vector2 dashSpeed = new Vector2(10f, 2f);
    public float jumpStrength = 8f;
    public float decelerationFactor = 0.9f;
    public float dashDecelerationFactor = 0.75f;
    public int maxHealth = 3;
    public int health = 3;

    [Header("Attack Settings")]
    public GameObject WrenchPrefab;
    public Transform attackSpawnPoint;
    private float attackAnimationTimer = 0.4f;

    private bool isGrounded;
    private float attackTimer;
    private bool dashAvailable;

    private float direction;
    private state playerState;

    private float dashTimer = 0.5f;
    private float decelerate;

    private float damagedTimer;
    float knockbackAngle;

    private Vector2 spawnLocation;
    private GameObject activeWrench;

    private float lastDirection = 1f;

    [Header("Attack Pause Settings")]
    public float attackPauseDuration = 0.2f; // How long the player pauses in midair
    private bool isPausedMidAir = false; // Prevents movement during the pause

    private void Start()
    {
        helenRunID = AudioManager.Instance.AddAudio("helenRun2");
        player = ReInput.players.GetPlayer(playerId);
        rb = GetComponent<Rigidbody2D>();

        spawnLocation = rb.position;
        playerState = state.idle;

        if (!rb)
        {
            Debug.LogError("No Rigidbody2D found on Player!");
        }
    }

    private void Update()
    {
        if (!isPausedMidAir) // Prevent movement updates while paused
        {
            HandleMovement();
            if (playerState != state.damaged)
            {
                HandleJumping();     
                
                HandleAttack();
                HandleDash();
            }
        }

        if (rb.position.y < -7)
        {
            rb.position = spawnLocation;
            rb.velocity = Vector3.zero;
            health = maxHealth;
            HealthDisplay.Instance.healthChange(health);
        }

        foreach (state s in Enum.GetValues(typeof(state)))
        {
            //animator.SetBool(s.ToString(), s == playerState);
            if (s == playerState)
                animator.SetFloat(s.ToString(), 1.0f);
            else
                animator.SetFloat(s.ToString(), 0.0f);
            //Debug.Log(s.ToString());
        }

        if(animator.transform.rotation != new Quaternion(0f, (lastDirection - 1f) * 90f, 0f, 0f))
            animator.transform.rotation = new Quaternion(0f, (lastDirection - 1f) * 90f, 0f, 0f);
        //Debug.Log(playerState);
        
    }

    private void HandleMovement()
    {
        if (playerState == state.damaged)
        {
            damagedTimer -= Time.deltaTime;
            if (damagedTimer < 0)
            {
                playerState = state.idle;
            }
        }

        if (playerState != state.dash && playerState != state.damaged)
        {
            direction = player.GetAxis("MoveHorizontal");

            if (Mathf.Abs(direction) > 0.1f && Mathf.Abs(rb.velocity.x) < moveSpeed)
            {
                rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
                lastDirection = Mathf.Sign(direction);

                if (isGrounded && playerState != state.wrenchThrow)
                {
                    playerState = state.run;
                }
            }
            else
            {
                decelerate = (Mathf.Abs(rb.velocity.x) < moveSpeed) ? decelerationFactor : dashDecelerationFactor;
                rb.velocity = new Vector2(rb.velocity.x * decelerate, rb.velocity.y);

                if (Mathf.Abs(rb.velocity.x) <= 0.01f && isGrounded && playerState != state.wrenchThrow)
                {
                    playerState = state.idle;
                   // Debug.Log("TEST1");
                }
            }
        }

        if (playerState == state.run)
        {
            AudioManager.Instance.PlayAudio(helenRunID);
        }
        else
        {
            if (AudioManager.Instance.isPlaying(helenRunID))
            {
                AudioManager.Instance.StopAudio(helenRunID);
            }
        }
    }

    private void HandleJumping()
    {
        if (player.GetButtonDown("Jump") && isGrounded)
        {
            AudioManager.Instance.PlaySoundEffect("helenJump");
            rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
            isGrounded = false;
            playerState = state.rise;
        }
        if (rb.velocity.y < 0f && playerState != state.dash && !isGrounded)
        {
            playerState = state.fall;
            isGrounded = false;
        }
    }

    private void HandleAttack()
    {
        if (player.GetButtonDown("Attack") && WrenchPrefab && attackSpawnPoint && playerState != state.dash)
        {
            if (activeWrench == null)
            {
                attackTimer = attackAnimationTimer;

                activeWrench = Instantiate(WrenchPrefab, attackSpawnPoint.position, Quaternion.identity);
                WrenchBehaviour wrenchScript = activeWrench.GetComponent<WrenchBehaviour>();
                AudioManager.Instance.PlaySoundEffect("wrenchThrow");

                if (wrenchScript)
                {
                    wrenchScript.Initialize(lastDirection, transform);
                    wrenchScript.OnDestroyCallback = OnWrenchDestroyed;
                }

                // Start the midair pause effect
                if (!isGrounded)
                {
                    StartCoroutine(PauseMidAir());
                }
            }
        }
       // Debug.Log(attackTimer.ToString());
        if (attackTimer > 0)
        {
            playerState = state.wrenchThrow;
            attackTimer -= Time.deltaTime;
            
        }
        if (attackTimer <= 0 && isGrounded && playerState == state.wrenchThrow)
        {
            playerState = state.idle;
        }
        else if (attackTimer <= 0 && !isGrounded && playerState == state.wrenchThrow)
        {
            playerState = state.fall;
        }

    }

    private void HandleDash()
    {
        if (playerState == state.dash)
        {
            if (dashTimer > 0)
            {
                dashTimer -= Time.deltaTime;
                rb.velocity = new Vector2(dashSpeed.x * Mathf.Sign(rb.velocity.x), rb.velocity.y);
            }
            else
            {
                playerState = state.fall;
            }
        }
        else if (player.GetButtonDown("Dash") && dashAvailable)
        {
            attackTimer = 0;
            dashAvailable = false;
            playerState = state.dash;
            dashTimer = 0.5f;
            isGrounded = false;
            rb.velocity = new Vector2(dashSpeed.x * lastDirection, dashSpeed.y);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Hazard"))
        {
            damagedTimer = 1f;
            playerState = state.damaged;
            AudioManager.Instance.PlaySoundEffect("helenHit");
            health -= 1;
            HealthDisplay.Instance.healthChange(health);
            Debug.Log(health);

            knockbackAngle = (collision.gameObject.transform.position.x > rb.position.x) ? -1f : 1f;
            rb.velocity = new Vector2(knockbackAngle, 3f);

            if (health <= 0)
            {
                Debug.Log("YOU DIED");
                rb.position = spawnLocation;
                rb.velocity = Vector3.zero;
                // Resets Helen's HP, since it was reaching negative values
                health = maxHealth;
                HealthDisplay.Instance.healthChange(health);
            }
        }
        else if (collision.gameObject.CompareTag("Ground") && playerState != state.wrenchThrow && rb.velocity.y == 0)
        {
            isGrounded = true;
            dashAvailable = true;
            playerState = (Mathf.Abs(rb.velocity.x) > 0) ? state.run : state.idle;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11 && playerState == state.dash && dashTimer < 0.45)
        {
            playerState = state.rise;
            rb.velocity = new Vector2(rb.velocity.x, 5f);
            dashAvailable = true;
            AudioManager.Instance.PlaySoundEffect("helenParry");
        }
        // Logic for heart pickup
        if(col.gameObject.layer == 14)
        {
            GainHealth(1);
        }
    }

    private void OnWrenchDestroyed()
    {
        activeWrench = null;
    }

    private IEnumerator PauseMidAir()
    {
        isPausedMidAir = true;
        Vector2 storedVelocity = rb.velocity; // save current velocity
        rb.velocity = Vector2.zero; // stop movement
        rb.gravityScale = 0; // disable gravity

        float timer = attackPauseDuration;
        while (timer > 0)
        {
            if (player.GetButtonDown("Dash") && dashAvailable)
            {
                // let player dash out of the airstop
                rb.gravityScale = 1; // restore gravity
                isPausedMidAir = false;
                HandleDash(); // immediately dash
                yield break; // exit coroutine
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        // restore normal movement
        rb.gravityScale = 1;
        rb.velocity = storedVelocity;
        isPausedMidAir = false;
        attackTimer = 0;
        
        if(rb.velocity.y > 0)
        {
            playerState = state.rise;
     
        }
        else
        {
            playerState = state.fall;
            
        }
        Debug.Log(playerState);
    }

    private void GainHealth(int healthGained)
    {
        if (health + healthGained <= maxHealth)
        {
            health += healthGained;
            HealthDisplay.Instance.healthChange(health);
            Debug.Log(health);
        }
    }
}
