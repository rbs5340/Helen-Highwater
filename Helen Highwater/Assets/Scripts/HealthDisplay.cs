using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    #region Singleton
    public static HealthDisplay Instance;

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

    public GameObject hImg;
    public Sprite[] sprites;
    /*
    0 = 0 health/dead, likely unneeded sprite
    1 = 1 health
    2 = 2 health
    3 = 3 health (max)
    4 = infinite health (mech)
    */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void healthChange(int health)
    {
        if (sprites.Length >= health)
        {
            hImg.gameObject.GetComponent<UnityEngine.UI.Image>().sprite = sprites[health];
        }
        else
        {
            Debug.Log("Health Sprite Broke");
        }
    }
}
