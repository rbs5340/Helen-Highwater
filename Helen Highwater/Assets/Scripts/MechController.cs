using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.UIElements;
using System;

public class MechController : MonoBehaviour
{
    public int playerId = 0; // The Rewired player ID (for a single player game, should always be 0)
    private Rewired.Player player; // The Rewired Player
    public Animator animator;


    //Player states for all actions so far. i have a different rising and falling state in case we want to change the gravity to make the platforming feel better,
    enum state
    {
        idle,
        run,
        rise,
        fall,
        damaged,
        wrenchThrow,
        hover
    }

    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpStrength = 100f;
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

    //Testing hover
    private float hoverTimer;
    private bool hoverAvailable;

    // Integers to store looping SFX indexes
    private int mechRunID;
    private int mechHoverID;

    private float lastDirection = 1f;

    private Vector2 spawnLocation;

    public float mechTimer;

    private void Start()
    {
        // Get Rewired Player
        player = ReInput.players.GetPlayer(playerId);

        // Get Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        spawnLocation = rb.position;

        playerState = state.idle;

        if (!rb)
        {
            Debug.LogError("No Rigidbody2D found on Player!");
        }

        mechRunID = AudioManager.Instance.AddAudio("mechRun");
        mechHoverID = AudioManager.Instance.AddAudio("mechHover");

        mechTimer = 90f;
    }

    private void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleAttack();
        HandleHover(); //HOVER TEST

        mechTimer -= Time.deltaTime;

        //Handles respawning
        if (rb.position.y < -7 || mechTimer < 0f)
        {
            rb.position = spawnLocation;
            rb.velocity = Vector3.zero;
            mechTimer = 90f;
            AudioManager.Instance.PlaySoundEffect("mechDeath");
        }

        //Sends Animation states to animator
        foreach (state s in Enum.GetValues(typeof(state)))
        {
            //animator.SetBool(s.ToString(), s == playerState);
            if (s == playerState)
                animator.SetFloat(s.ToString(), 1.0f);
            else
                animator.SetFloat(s.ToString(), 0.0f);
            //Debug.Log(s.ToString());
        }

        if (animator.transform.rotation != new Quaternion(0f, (lastDirection - 1f) * 90f, 0f, 0f))
            animator.transform.rotation = new Quaternion(0f, (lastDirection - 1f) * 90f, 0f, 0f);
        //Logs player game state for testing purposes
        //Debug.Log(mechTimer);
        
    }

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

            if (rb.velocity.x <= 0.25f && isGrounded)
            {
                playerState = state.idle;
            }
        }

        if (playerState == state.run)
        {
            AudioManager.Instance.PlayAudio(mechRunID);
        }
        else
        {
            if (AudioManager.Instance.isPlaying(mechRunID))
            {
                AudioManager.Instance.StopAudio(mechRunID);
            }
        }
    }

    private void HandleJumping()
    {
        if (player.GetButtonDown("Jump") && isGrounded)

        {
            // Plays mech jump sound effect
            AudioManager.Instance.PlaySoundEffect("mechJump");
            rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
            isGrounded = false;
            playerState = state.rise;
        }
        if (rb.velocity.y < -0.1f && playerState != state.hover)
        {
            Debug.Log("FALL");
            playerState = state.fall;
            isGrounded = false;
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
        if (player.GetButton("Attack") && playerState != state.hover)
        {
            playerState = state.wrenchThrow;
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(ProjectilePrefab, attackSpawnPoint.position, Quaternion.identity);
        MechProjectileBehaviour projectileScript = projectile.GetComponent<MechProjectileBehaviour>();

        // Play the projectile sound effect
        AudioManager.Instance.PlaySoundEffect("mechProjectile");

        if (projectileScript)
        {
            projectileScript.Initialize(lastDirection);
        }
    }

    //Testing Hover
    private void HandleHover()
    {
        //Debug.Log(dashTimer);
        //Will keep the player's momentum while in dash state and increment the timer
        if (playerState == state.hover)
        {

            if (hoverTimer > 0)
            {
                hoverTimer -= Time.deltaTime;
                if (rb.velocity.y <= 0f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0.3f); //I made this 0.3 to counteract gravity or something idk but it works
                }

            }
            else
            {
                playerState = state.fall;
            }
        }

        //Will begin the dash if not already in dash state and the button is pressed
        else if (player.GetButtonDown("Dash") && hoverAvailable && isGrounded == false)
        {
            hoverAvailable = false;
            playerState = state.hover;
            hoverTimer = 1f;
            isGrounded = false;
        }

        if (playerState == state.hover)
        {
            AudioManager.Instance.PlayAudio(mechHoverID);
        }
        else
        {
            if (AudioManager.Instance.isPlaying(mechHoverID))
            {
                AudioManager.Instance.StopAudio(mechHoverID);
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // This code doesn't always occur when the mech touches the ground, I think it might be
        // because the y-velocity needs to be 0. Could consider having a low threshold instead
        // of requiring it to be exactly 0 - Will
        if (collision.gameObject.CompareTag("Ground"))
            
        {
            Debug.Log("LANDED");
            CameraFollowPlayer.Instance.Shake();
            AudioManager.Instance.PlaySoundEffect("mechLand");
            isGrounded = true;
            hoverAvailable = true; //Test hover

            playerState = (Mathf.Abs(rb.velocity.x) > 0) ? state.run : state.idle;
        }
    }

}
