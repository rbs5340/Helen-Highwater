using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speedValue; // How much distance the enemy will cover each frame
    [SerializeField] private int timerValue; // Number of frames that the enemy will pause for
    [SerializeField] private BoxCollider2D myCollider;

    private float speed;
    private int timer;
    private bool edgeOfPlat;
    private bool isAlive;

    private Camera gameCamera;

    // Start is called before the first frame update
    void Start()
    {
        // Centers the "enemy"
        transform.position = Vector3.zero;

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

        // Initializes game camera
        gameCamera = Camera.main;
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
            Debug.Log("Timer started");
        }

        // If enemt collides with hazard (lava)
        if(collision.gameObject.layer == 9)
        {
            EnemyDeath();
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
        }
        else
        {
            // Drop down (thats it again)
            transform.position = transform.position + new Vector3(0f, -(speed), 0f);
        }
        
        // Stabilize rotation of the z axis
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,
        transform.eulerAngles.y, 0);

        // To Do: track bounds of camera and destroy/deactivate
        // enemy if it leave camera view while dead

        // Camera testing
        //gameCamera.transform.position = new Vector3(gameCamera.transform.position.x, 
            //gameCamera.transform.position.y + 0.001f, gameCamera.transform.position.z);
    }

    private void EnemyDeath()
    {
        // Changes the necessary boolean
        isAlive = false;

        // Flips the sprite over the x axis
        Vector3 enemyScale = transform.localScale;
        enemyScale.y = enemyScale.y * -1f;
        transform.localScale = enemyScale;
        
        // Disables the collider
        myCollider.enabled = false;
    }
}
