using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.UIElements;
using UnityEngine.UI;
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

    [Header("Hover Settings")]
    public float maxHoverFuel = 2f;
    public float hoverFuelDrain = 1f;
    public float hoverAcceleration = 0.1f;
    public float maxHoverUpwardSpeed = 2f;

    private float currentHoverFuel;

    private bool jumpReleased = false;

    private float prevYVelocity = 0;

    // Integers to store looping SFX indexes
    private int mechRunID;
    private int mechHoverID;

    private float lastDirection = 1f;

    private Vector2 spawnLocation;

    public float mechTimer;
    private float maxMechTime = 90f;

    private UnityEngine.UI.Slider escapeSlider;
    //public GameObject hoverSlider;

    //Hover Slider Related
    public GameObject hoverSlider;
    public GameObject parent;

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

        currentHoverFuel = maxHoverFuel;

        escapeSlider = EscapeTimer.Instance.getSlider();

        //Hover Slider Related
        this.transform.position = parent.transform.position;
        parent.transform.position = Vector3.zero;
        CameraFollowPlayer.Instance.SetPlayer(this.gameObject);
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
            mechTimer = maxMechTime;
            AudioManager.Instance.PlaySoundEffect("mechDeath");
        }

        UpdateSlider(escapeSlider, mechTimer, maxMechTime);

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
        {
            animator.transform.rotation = new Quaternion(0f, (lastDirection - 1f) * 90f, 0f, 0f);
        }
        //Logs player game state for testing purposes
        //Debug.Log(prevYVelocity);

        //Hover Slider Related
        Vector3 pos = transform.position;
        pos.y += 1f;
        hoverSlider.transform.position = pos;
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
            prevYVelocity = rb.velocity.y;
            // Plays mech jump sound effect
            AudioManager.Instance.PlaySoundEffect("mechJump");
            rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
            isGrounded = false;
            playerState = state.rise;

            jumpReleased = false;
        }

        if (!player.GetButton("Jump"))
        {
            jumpReleased = true;
        }

        if (rb.velocity.y < -0.1f && playerState != state.hover)
        {
            prevYVelocity = rb.velocity.y;
            //Debug.Log("FALL");
            playerState = state.fall;
            isGrounded = false;
            jumpReleased = true; // allow hover at jump apex
		}

        if (rb.velocity.y == 0 && playerState != state.idle && playerState != state.run && prevYVelocity < 0)
        {
            //Debug.Log("LANDED NOT COLLISION");
            playerState = (Mathf.Abs(rb.velocity.x) > 0.25) ? state.run : state.idle;
            isGrounded = true;
            hoverSlider.gameObject.SetActive(false);
            currentHoverFuel = maxHoverFuel;
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

    private void HandleHover()
    {
        bool jumpHeld = player.GetButton("Jump");

        // eneter hovering state when not grounded + jump is held
        if (!isGrounded && jumpHeld && currentHoverFuel > 0f && jumpReleased)
        {
            prevYVelocity = rb.velocity.y;
            if (playerState != state.hover)
            {
                hoverSlider.gameObject.SetActive(true);
                playerState = state.hover;
            }

            // apply upward acceleration
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + hoverAcceleration);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -Mathf.Infinity, maxHoverUpwardSpeed));

            // drain fuel
            currentHoverFuel -= hoverFuelDrain * Time.deltaTime;
            UpdateSlider(hoverSlider.GetComponent<UnityEngine.UI.Slider>(), currentHoverFuel, maxHoverFuel);
            hoverSlider.gameObject.transform.GetChild(1).gameObject.SetActive(currentHoverFuel>0f);
            // Play hover sound
            AudioManager.Instance.PlayAudio(mechHoverID);
        }
        else
        {
            // exit hover
            if (playerState == state.hover)
            {
                prevYVelocity = rb.velocity.y;
                playerState = (rb.velocity.y < 0f) ? state.fall : state.rise;
            }

            // stop hover audio
            if (AudioManager.Instance.isPlaying(mechHoverID))
            {
                AudioManager.Instance.StopAudio(mechHoverID);
            }
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && rb.velocity.y == 0)

        {
            //Debug.Log("LANDED");
            CameraFollowPlayer.Instance.Shake();
            AudioManager.Instance.PlaySoundEffect("mechLand");

            isGrounded = true;
            hoverSlider.gameObject.SetActive(false);
            currentHoverFuel = maxHoverFuel;

            playerState = (Mathf.Abs(rb.velocity.x) > 0.25) ? state.run : state.idle;
        }
    }

    public void UpdateSlider(UnityEngine.UI.Slider slider, float value, float maxValue)
    {
        slider.value = value / maxValue;
    }

}
