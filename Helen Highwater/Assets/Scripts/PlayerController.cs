using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.UIElements;
using System;

public class PlayerController : MonoBehaviour
{
    public int playerId = 0; // The Rewired player ID (for a single player game, should always be 0)
    private Rewired.Player player; // The Rewired Player

    // Integers to store looping SFX indexes
    private int helenRunID;


    //Player states for all actions so far. i have a different rising and falling state in case we want to change the gravity to make the platforming feel better,
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
    public float dashVerticleSpeed = 2f;
    public float jumpStrength = 8f;
    public float decelerationFactor = 0.9f;
    public float dashDecelerationFactor = 0.75f;

    [Header("Attack Settings")]
    public GameObject WrenchPrefab;
    public Transform attackSpawnPoint;
    public float attackAnimationTimer;

    private bool isGrounded;
    private float attackTimer;
    private bool dashAvailable;
     
    private float direction;
    private state playerState;

    public float dashTimer = 0.5f;
    private float decelerate;

    private void Start()
    {
        // Loads in Helen's run audio
        helenRunID = AudioManager.Instance.AddAudio("helenRun2");

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
        HandleDash();

        //Logs player game state for testing purposes
        //Debug.Log(playerState.ToString());
        
    }

    private float lastDirection = 1f; // 1 for right, -1 for left

    private void HandleMovement()
    {
        if (playerState != state.dash)
        {
            direction = player.GetAxis("MoveHorizontal");

            if (Mathf.Abs(direction) > 0.1f && Mathf.Abs(rb.velocity.x) < moveSpeed) // small dead zone to prevent jitter

            {
                //Debug.Log(Mathf.Abs(rb.velocity.x));
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
                //Decelerates more during the ending of the parry state when your velocity is higher than your regular movement speed
                if (Mathf.Abs(rb.velocity.x) < moveSpeed) { 
                    decelerate = decelerationFactor;
                }
                else
                {
                    decelerate = dashDecelerationFactor;
                }
                rb.velocity = new Vector2(rb.velocity.x * decelerate, rb.velocity.y);

                if (rb.velocity.x == 0f && isGrounded)
                {
                    playerState = state.idle;
                }
            }
        }

        if(playerState == state.run)
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
        if(rb.velocity.y < 0f && playerState != state.dash)
        {
            playerState = state.fall;
            isGrounded = false;
        }
    }

    private void HandleAttack()
    {
        if (player.GetButtonDown("Attack") && WrenchPrefab && attackSpawnPoint && playerState != state.dash)
        {
            attackTimer = attackAnimationTimer;

            GameObject wrench = Instantiate(WrenchPrefab, attackSpawnPoint.position, Quaternion.identity);
            WrenchBehaviour wrenchScript = wrench.GetComponent<WrenchBehaviour>();
            AudioManager.Instance.PlaySoundEffect("wrenchThrow");

            if (wrenchScript)
            {
                wrenchScript.Initialize(lastDirection, transform);
            }
        }

        //Handles wrench throwing state, should trump every other game state animation wise
        if (attackTimer > 0)
        {
            playerState = state.wrenchThrow;
            attackTimer -= Time.deltaTime;  
        }
    }

    private void HandleDash()
    {
        //Debug.Log(dashTimer);
        //Will keep the player's momentum while in dash state and increment the timer
        if(playerState == state.dash)
        {

            if (dashTimer > 0)
            {
                dashTimer -= Time.deltaTime;
                if (rb.velocity.x >= 0f)
                {
                    rb.velocity = new Vector2(dashSpeed.x, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(dashSpeed.x * -1, rb.velocity.y);
                }

            }
            else
            {
                playerState = state.fall;
            }
        }

        //Will begin the dash if not already in dash state and the button is pressed
        else if (player.GetButtonDown("Dash") && dashAvailable)
        {
            dashAvailable = false;
            playerState = state.dash;
            dashTimer = 0.5f;
            isGrounded = false;
            if (rb.velocity.x >= 0f)
            {
                rb.velocity = dashSpeed;
            }
            else
            {
                rb.velocity = dashSpeed * new Vector2(-1f, 1f);
            }

        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.ToString());
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            dashAvailable = true;
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


    //Checks for collision with the wrench trigger for the parry jump
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11 && playerState == state.dash)
        {
            playerState = state.rise;
            rb.velocity = new Vector2(rb.velocity.x, 5f);
            dashAvailable = true;
        }

    }
}
