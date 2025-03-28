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

    // Integers to store looping SFX indexes
    private int crabWalkID;

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
            speedValue = 0.002f;
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

        // Loads crabWalk SFX
        crabWalkID = AudioManager.Instance.AddAudio("crabWalk2");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If enemy collides into a pit
        if (collision.gameObject.layer == 8)
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
        if (collision.gameObject.layer == 11 && isAlive)
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
        if (isAlive)
        {
            // Moves forward (thats it)
            transform.position = transform.position + new Vector3(speed, 0f, 0f);

            // Plays the walk sound effect if this is the first time the crab has moved
            if (!firstMove)
            {
                AudioManager.Instance.PlayAudio(crabWalkID);
                firstMove = true;
            }
        }
        else if(isMoving)
        {
            // Drop down (thats it again)
            transform.position = transform.position + new Vector3(0f, -(speed/2), 0f);
        }
        
        // Stabilize rotation of the z axis
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,
        transform.eulerAngles.y, 0);

        // To Do: track bounds of camera and destroy/deactivate
        // enemy if it leave camera view while dead
    }

    private void EnemyDeath(bool hitByMech)
    {
        // Changes the necessary boolean
        isAlive = false;

        // Flips the sprite over the x axis
        Vector3 enemyScale = transform.localScale;
        enemyScale.y = enemyScale.y * -1f;
        transform.localScale = enemyScale;

        // Pops the enemy up a bit to prevent weird collision
        transform.position = transform.position + new Vector3(0f, 0.2f, 0f);

        // Changes the tag to prevent Helen from getting hit
        // Set to "Ground" specifically to allow Helen to jump off of enemy
        gameObject.tag = "Ground";

        if (hitByMech)
        {
            // Disables the collider
            myCollider.enabled = false;
        }

        // Death sound effect
        AudioManager.Instance.PlaySoundEffect("crabDeath");
    }
}
