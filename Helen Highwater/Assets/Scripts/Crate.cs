using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroys the crate when the wrench collides with it
        if (collision.gameObject.layer == 11)
        {
            DestroyThis();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to destroy this crate when necessary
    private void DestroyThis()
    {
        // Play the crate destroying sound effect
        AudioManager.Instance.PlaySoundEffect("crateDestroy");

        Destroy(this.gameObject);

        // Could alternatively use setActive instead of destroy
    }
}
