using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    #region Singleton
    public static StartManager Instance;

    // Sets up instance of the Start Manager Singleton
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

    // AudioManager Prefab
    public GameObject audioManager;

    // Start is called before the first frame update
    void Start()
    {
        // Creates the audioManager if it doesn't exist
        if (!AudioManager.Instance)
        {
            Instantiate(audioManager);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
