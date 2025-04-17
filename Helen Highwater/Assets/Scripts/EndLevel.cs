using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public PauseMenu pauseMenu; // reference to the PauseMenu script

    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if player enters the trigger in the mech
        if (other.CompareTag("Player - Mech"))
        {
            Debug.Log("End Goal Triggered");

            if (pauseMenu != null)
            {
                EscapeTimer.Instance.disable();
                pauseMenu.EndScreen(); // call the EndScreen method from PauseMenu
            }
        }
    }
}
