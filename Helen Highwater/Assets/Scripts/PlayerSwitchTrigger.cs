using UnityEngine;

public class PlayerSwitchTrigger : MonoBehaviour
{
    public PlayerSwitch playerSwitch;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if the player enters the trigger
        if (other.CompareTag("Player - Helen"))
        {
            Debug.Log("Player Switch Triggered");

            if (playerSwitch != null)
            {
                playerSwitch.SwitchPlayer();
            }
        }
    }
}
