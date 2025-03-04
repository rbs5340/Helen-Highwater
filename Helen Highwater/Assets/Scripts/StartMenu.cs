using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //public GameObject optionsMenu;
    //public GameObject controlChangeMenu;

    // Start is called before the first frame update
    void Start()
    {
          
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame(string scenename)
    { //Change scenename in inspector in the StartMenu scene if you want to test different scenes from Start Menu
        AudioManager.Instance.PlaySoundEffect("buttonPress");
        SceneManager.LoadScene(scenename);
    }

    public void ExitGame()
    {
        AudioManager.Instance.PlaySoundEffect("buttonPress");
        Application.Quit();
        Debug.Log("Game Exit");
    }

    /*
    public void OpenOptions()
    {
        AudioManager.Instance.PlaySoundEffect("buttonPress");
        optionsMenu.SetActive(true);
    }

    public void CloseOptions()
    {
        AudioManager.Instance.PlaySoundEffect("buttonPress");
        optionsMenu.SetActive(false);
    }

    public void OpenControls()
    {
        AudioManager.Instance.PlaySoundEffect("buttonPress");
        controlChangeMenu.SetActive(true);
    }
    */
}
