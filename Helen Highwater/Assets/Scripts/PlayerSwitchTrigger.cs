using UnityEngine;

public class PlayerSwitchTrigger : MonoBehaviour
{
    public PlayerSwitch playerSwitch;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player - Helen")) // Ensure Helen has the tag "Helen"
        {
            Debug.Log("Player Switch Triggered");

            if (playerSwitch != null)
            {
                playerSwitch.SwitchPlayer();
            }
        }
    }
}
