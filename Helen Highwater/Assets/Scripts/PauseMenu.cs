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

    //Text sliders
    public TextMeshProUGUI masterText;
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;

    // Volume Sliders
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes slider values
        masterSlider.value = AudioManager.Instance.masterVolume;
        musicSlider.value = AudioManager.Instance.musicVolume;
        sfxSlider.value = AudioManager.Instance.sfxVolume;
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
        optionsMenu.SetActive(true);
        if (pauseButton != null)
        {
            pauseMenu.SetActive(false);
        }
        AudioManager.Instance.PlaySoundEffect("buttonPress");
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        AudioManager.Instance.PlaySoundEffect("buttonPress");
    }

    public void OpenControls()
    {
        AudioManager.Instance.PlaySoundEffect("buttonPress");
        controlChangeMenu.SetActive(true);
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
    }
    public void SetMusicVolume(System.Single value)
    {
        AudioManager.Instance.SetMusic(value);
        musicText.text = (int)(value * 100) + "%";
    }
    public void SetSfxVolume(System.Single value)
    {
        AudioManager.Instance.SetSFX(value);
        sfxText.text = (int)(value * 100) + "%";
    }
}
