using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeTimer : MonoBehaviour
{
    #region Singleton
    public static EscapeTimer Instance;
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
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Slider getSlider()
    {
        GetComponent<Renderer>().enabled = true;
        return this.gameObject.GetComponent<Slider>();
    }
}
