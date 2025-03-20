using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if Helen collides with the gear
        if(collision.gameObject.CompareTag("Player - Helen") || collision.gameObject.CompareTag("Player - Mech"))
        {
            this.gameObject.SetActive(false);
            GlobalVar.Instance.gearsCollected++;
            Debug.Log(GlobalVar.Instance.gearsCollected + "/" + GlobalVar.Instance.totalGears + " Gears Collected");
        }
    }
}
