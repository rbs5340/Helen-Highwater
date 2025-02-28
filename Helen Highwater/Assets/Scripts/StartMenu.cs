using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

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
}
