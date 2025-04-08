using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    #region Singleton
    public static CameraFollowPlayer Instance;
    private bool isShaking; //If screen is shaking
    private float shakeTime; //How long it will shake for
    private float shakeStartTime; //When shaking started

    // Sets up instance of the Game Manager Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Instance already exists");
            Destroy(Instance);
        }
    }
    #endregion


    public Transform player;
    public Transform background;

    // Start is called before the first frame update
    void Start()
    {
        isShaking = false;
        shakeTime = 0.1f;
        shakeStartTime = -1f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos=transform.position;
        pos.x = player.position.x;
        pos.y = player.position.y / 1.5f;
        if (pos.y < 0) pos.y = 0;
        if (isShaking)
        {
            pos += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
            if(Time.time-shakeStartTime > shakeTime)
            {
                isShaking=false;
            }
        }
        transform.position = pos;
        pos.x = (player.position.x*.965f) + 2f;
        pos.z = 10f;
        background.position = pos;
        
    }

    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer.transform;
    }

    public void Shake()
    {
        isShaking=true;
        shakeStartTime = Time.time;
    }

    public Vector3 GetCameraPos()
    {
        return transform.position;
    }
}
