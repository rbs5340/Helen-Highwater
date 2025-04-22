using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeTimer : MonoBehaviour
{
    public Slider slider;
    private bool ac;

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
        this.gameObject.SetActive(false);
        ac = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Slider getSlider()
    {
        ac = true;
        enable();
        return this.slider;
    }

    public void disable()
    {
        if (ac)
        {
            this.gameObject.SetActive(false);
        }

    }

    public void enable()
    {
        if (ac)
        {
            this.gameObject.SetActive(true);
        }
    }
    
}
