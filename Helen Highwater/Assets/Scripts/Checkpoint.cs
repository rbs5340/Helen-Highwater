using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Logic for colliding with Helen
        if (collision.gameObject.CompareTag("Player - Helen"))
        {
            this.GetComponent<Collider2D>().enabled = false;
            AudioManager.Instance.PlaySoundEffect("checkpoint");
            //this.gameObject.SetActive(false);
        }
    }
}
