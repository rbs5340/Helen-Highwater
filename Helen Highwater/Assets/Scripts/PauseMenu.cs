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

    //Controls Change Objects
    public GameObject controlButton;
    public TextMeshProUGUI controlName;
    public TextMeshProUGUI oldKey;
    public TextMeshProUGUI newKey;

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
        if (controlChangeMenu.activeSelf == true)
        {
            KeyCode currentKey = getCurrentKeyDown();
            if(currentKey != KeyCode.None && currentKey != KeyCode.Mouse0)
            {
                controlButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text=currentKey.ToString();
                controlChangeMenu.SetActive(false);
            }
        }
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
        pauseMenu.SetActive(false);
        AudioManager.Instance.PlaySoundEffect("buttonPress");
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        AudioManager.Instance.PlaySoundEffect("buttonPress");
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

    public void controlButtonPressed(GameObject button)
    {
        TextMeshProUGUI buttonText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        controlButton = button;
        controlName.text = button.name;
        oldKey.text = buttonText.text;
        controlChangeMenu.SetActive(true);
    }

    public KeyCode getCurrentKeyDown()
    {
        KeyCode finalKeyCode = KeyCode.None;
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode))) { if (Input.GetKeyDown(kcode)) { finalKeyCode = kcode; } }
        if (finalKeyCode == KeyCode.None)
        {
            //Couldn't find key
        }
        return finalKeyCode;
    }
}
