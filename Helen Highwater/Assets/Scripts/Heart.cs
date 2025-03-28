using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Heart : MonoBehaviour
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
        if (collision.gameObject.CompareTag("Player - Helen") || collision.gameObject.CompareTag("Player - Mech"))
        {
            AudioManager.Instance.PlaySoundEffect("heartPickup");
            this.gameObject.SetActive(false);
        }
    }
}
