using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelenGlowController : MonoBehaviour
{

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(-2000f, -200f);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(player.gameObject.GetComponent<PlayerController>().playerState);
        Debug.Log(player.gameObject.GetComponent<PlayerController>().parryAvailable);
        if(player.gameObject.GetComponent<PlayerController>().playerState == PlayerController.state.dash && player.gameObject.GetComponent<PlayerController>().parryAvailable)
        {
            Vector3 pos=player.transform.position;
            pos.z++;
            transform.position = pos;
        }
        else
        {
            transform.position = new Vector3(-2000f, -200f, 1f);
        }
    }
}
