using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speedValue; // How much distance the enemy will cover each frame
    [SerializeField] private int timerValue; // Number of frames that the enemy will pause for
    [SerializeField] private BoxCollider2D myCollider; // This enemy's collision box

    private float speed; // Movement speed of the enemy
    private int timer; // Number of frames the enemy will pause for
    private bool edgeOfPlat; // Whether or not the enemy has reached the end of the platform
    private bool isAlive; // Is this enemy alive or not
    private bool firstMove; // Used to invoke code when this enemy first moves
    private bool isMoving; // Is the crab still moving?

    private Camera mainCamera;
    private bool onScreen; // Is the crab on screen?
    private bool enteredScreen; // Did the crab just enter the screen?
    private Vector3 screenPoint;

    // Integers to store looping SFX indexes
    private int crabWalkID;

    // For respawning the crab on death
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float xPos;
    [SerializeField] private float yPos;
    private Vector2 startPos;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Centers the "enemy"
        //transform.position = Vector3.zero;

        // Puts the enemy literally anywhere else
        //transform.position = new Vector3(0, 1f, 0);

        // Sets base speed
        if(speedValue == 0)
        {
            speedValue = 2.0f;
        }
        
        // Sets the "timer"
        if(timerValue == 0)
        {
            timerValue = 30;
        }

        // Initializes all variables
        speed = speedValue;
        timer = timerValue;
        edgeOfPlat = false;
        isAlive = true;
        firstMove = false;
        isMoving = true;
        onScreen = false;
        enteredScreen = false;

        mainCamera = Camera.main;

        // Loads crabWalk SFX
        crabWalkID = AudioManager.Instance.AddAudio("crabWalk2");

        // Hardcoded because this just isnt working
        startPos = new Vector2(xPos, yPos);

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If enemy collides into a pit
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 10)
        {
            // Reverses the direction of the enemy
            speed = 0;
            timer = 0;
            edgeOfPlat = true;
            //Debug.Log("Timer started");
            AudioManager.Instance.StopAudio(crabWalkID);
        }

        // If enemy collides with hazard (lava) or wrench
        if(collision.gameObject.layer == 9)
        {
            EnemyDeath(false);
        }

        // If it hits the floor is stops moving
        if(!isAlive && collision.gameObject.layer == 7)
        {
            isMoving = false;
        }

        // If enemy collides with the mech
        if(collision.gameObject.CompareTag("Player - Mech"))
        {
            EnemyDeath(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroys the enemy when the wrench collides with it
        if ((collision.gameObject.layer == 11) && isAlive)
        {
            AudioManager.Instance.StopAudio(crabWalkID);
            EnemyDeath(false);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Keeps the enemy in 
        if (edgeOfPlat)
        {
            if(timer < timerValue)
            {
                timer++;
                //Debug.Log(timer);
            }
            else
            {
                //Debug.Log("Switching Direction");
                speedValue *= -1;
                speed = speedValue;
                // Flips the sprite over the y axis
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 
                    transform.eulerAngles.y + 180, transform.eulerAngles.z);
                edgeOfPlat = false;

                // Starts playing walking sound effect (This one works fine)
                if (isAlive) {
                    AudioManager.Instance.PlayAudio(crabWalkID);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // unused for now
    }

    // Update is called once per frame
    void Update()
    {
        // Gets reference to the player and playerController script
        if (player == null)
        {
            player = GameObject.FindWithTag("Player - Helen");
            playerController = player.GetComponent<PlayerController>();
        }

        //Debug.Log(playerController.wasAlive + " " + playerController.isAlive);

        if (!playerController.isAlive)
        {
            playerController.isAlive = true;
            EnemyRespawn();
        }

        if (isAlive)
        {
            // Moves forward (thats it)
            transform.position = transform.position + new Vector3(speed * Time.deltaTime, 0f, 0f);

            // Plays the walk sound effect if this is the first time the crab has moved
            if (!firstMove)
            {
                AudioManager.Instance.PlayAudio(crabWalkID);
                firstMove = true;
            }

            enteredScreen = onScreen;
            screenPoint = mainCamera.WorldToViewportPoint(this.transform.position);
            // Determines whether or not enemy is on screen
            onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            // If enemy is not on screen, walk SFX is paused
            if (!onScreen)
            {
                AudioManager.Instance.PauseAudio(crabWalkID);
            }
            // Resumes audio on the frame the enemy enters the screen
            else if (!enteredScreen && onScreen)
            {
                AudioManager.Instance.PlayAudio(crabWalkID);
            }
            //Debug.Log(onScreen);
        }
        
        // Stabilize rotation of the z axis
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,
        transform.eulerAngles.y, 0);
    }

    private void EnemyDeath(bool hitByMech)
    {
        // Changes the necessary boolean
        isAlive = false;

        // Flips the sprite over the x axis
        //Vector3 enemyScale = transform.localScale;
        //enemyScale.y = enemyScale.y * -1f;
        //transform.localScale = enemyScale;

        // Pops the enemy up a bit to prevent weird collision
        transform.position = transform.position + new Vector3(0f, 0.2f, 0f);

        // Changes the tag to prevent Helen from getting hit
        // Set to "Ground" specifically to allow Helen to jump off of enemy
        //gameObject.tag = "Ground";

        // Normally, the enemy crab will only die if hit by the mech
        /*if (hitByMech)
        {
            // Disables the collider
            myCollider.enabled = false;
        }*/
        // However, due to a bug, it dies to any attack by Helen (for now)
        //myCollider.enabled = false;

        // Sets the collision layer to "enemy" to prevent from
        // colliding with edge objects
        gameObject.layer = 16;

        spriteRenderer.enabled = false;

        // Death sound effect
        AudioManager.Instance.PlaySoundEffect("crabDeath");
    }

    // Resets the crab when Helen dies
    private void EnemyRespawn()
    {
        isAlive = true;
        spriteRenderer.enabled = true;
        gameObject.layer = 6;
        transform.position = new Vector2(startPos.x, startPos.y);
    }
}
