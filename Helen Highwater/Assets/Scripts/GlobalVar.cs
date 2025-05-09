using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used to store any variables that need to be globally accessible
public class GlobalVar : MonoBehaviour
{
    #region Singleton
    public static GlobalVar Instance;

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

    public int gearsCollected; // Number of gears the player has collected
    public int totalGears; // The total number of gears in the level
    public int helenState; // 0 for Helen, 1 for Mech
    public float startTime;

    // Start is called before the first frame update
    void Start()
    {
        gearsCollected = 0;
        helenState = 0;
        totalGears = 3;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Current Time Taken:" + (Time.time-startTime));
    }

}
