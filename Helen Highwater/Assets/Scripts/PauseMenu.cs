using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //Gameplay Objects is an empty game object that all gameplay elements can go into. If this is done then everything will deactivate
    public GameObject gameplayObjects;
    public GameObject pauseButton;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject controlChangeMenu;
    public GameObject endScreen;

    //Text sliders
    public TextMeshProUGUI masterText;
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;

    // Volume Sliders
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // End Screen Objects
    public TextMeshProUGUI timeTakenText;
    public Sprite gearCollected;
    public GameObject gear1;
    public GameObject gear2;
    public GameObject gear3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pause()
    {
        Time.timeScale = 0f;
        if(gameplayObjects != null)
        {
            gameplayObjects.SetActive(false);
        }
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
        AudioManager.Instance.PlaySoundEffect("buttonPress");
    }

    public void OpenOptions()
    {
        masterSlider.value = AudioManager.Instance.masterVolume;
        musicSlider.value = AudioManager.Instance.musicVolume;
        sfxSlider.value = AudioManager.Instance.sfxVolume;
        optionsMenu.SetActive(true);
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        AudioManager.Instance.PlaySoundEffect("buttonPress");
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }   
        AudioManager.Instance.PlaySoundEffect("buttonPress");
    }

    public void OpenControls()
    {
        AudioManager.Instance.PlaySoundEffect("buttonPress");
        controlChangeMenu.SetActive(true);
    }

    public void EndScreen()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
        }
        endScreen.SetActive(true);
        float timeTaken = Time.time - GlobalVar.Instance.startTime;
        //Debug.Log("Time Taken Test:"+timeTaken);
        int minsTaken = (int)(timeTaken / 60f);
        float secsTaken = timeTaken % 60;
        int gears = GlobalVar.Instance.gearsCollected;
        timeTakenText.text = "Clear Time: "+minsTaken.ToString("00")+"m:"+secsTaken.ToString("#.00")+"s";
        //Debug.Log("Gears:" + gears);
        if (gears > 0)
        {
            gear1.gameObject.GetComponent<UnityEngine.UI.Image>().sprite = gearCollected;
        }
        if (gears > 1)
        {
            gear2.gameObject.GetComponent<UnityEngine.UI.Image>().sprite = gearCollected;
        }
        if (gears > 2)
        {
            gear3.gameObject.GetComponent<UnityEngine.UI.Image>().sprite = gearCollected;
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        if (gameplayObjects != null)
        {
            gameplayObjects.SetActive(true);
        }
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
        AudioManager.Instance.PlaySoundEffect("buttonPress");
    }

    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
        AudioManager.Instance.PlaySoundEffect("buttonPress");
    }

    public void SetMasterVolume(System.Single value)
    {
        AudioManager.Instance.SetMaster(value);
        masterText.text = (int)(value * 100) + "%";
        AudioManager.Instance.PlaySoundEffect("volumeSlider");
    }
    public void SetMusicVolume(System.Single value)
    {
        AudioManager.Instance.SetMusic(value);
        musicText.text = (int)(value * 100) + "%";
        // Currently this effect doesn't get quieter as the volume decreases,
        // While the other two do, can fix this by making this sound scale with music volume
        AudioManager.Instance.PlaySoundEffect("volumeSlider");
    }
    public void SetSfxVolume(System.Single value)
    {
        AudioManager.Instance.SetSFX(value);
        sfxText.text = (int)(value * 100) + "%";
        AudioManager.Instance.PlaySoundEffect("volumeSlider");
    }
}
