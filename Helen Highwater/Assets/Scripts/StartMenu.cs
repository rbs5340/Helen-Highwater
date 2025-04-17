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
        AudioManager.Instance.StopAllSounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame(string scenename)
    { //Change scenename in inspector in the StartMenu scene if you want to test different scenes from Start Menu
        AudioManager.Instance.PlaySoundEffect("buttonPress");
        // Restarts the game soundtrack
        AudioManager.Instance.StopAudio(0);
        AudioManager.Instance.StopAudio(1);
        // Plays the main theme
        AudioManager.Instance.PlayMusic(0, 0.2f);
        SceneManager.LoadScene(scenename);
    }

    public void ExitGame()
    {
        AudioManager.Instance.PlaySoundEffect("buttonPress");
        Application.Quit();
        Debug.Log("Game Exit");
    }
}
