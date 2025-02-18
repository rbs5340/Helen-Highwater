using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Audio Manager Class, Created by Will Doyle
// Contains methods for playing music and SFX, as well as
// fields that determine the volume of the game sounds
public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance;

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
    
    // Single Audio Source that can be used to play multiple sounds
    // Used to handle all one shot sound effects
    [SerializeField] private AudioSource source;
    // Determines the volume (0.0f to 1.0f)
    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;

    // List of all looping audio files
    private List<AudioClip> audioClips = new List<AudioClip>();
    // List of audio sources to correspond to these files
    private List<AudioSource> audioSources = new List<AudioSource>();

    // Ints for sound indexes
    private int musicSFX;

    // Start is called before the first frame update
    void Start()
    {
        // Volume will need to be hooked up to the sound settings
        masterVolume = 1.0f;
        musicVolume = 1.0f;
        sfxVolume = 1.0f;

        // Clear track lists
        audioClips.Clear();
        audioSources.Clear();

        // Sound Index variables should be initialized to a value less than 0
        musicSFX = -1;

        // Testing junk
        musicSFX = AddAudio("weezerRiff");
        //PlayAudio(musicSFX);
    }

    // Update is called once per frame
    void Update()
    {
        // Empty for now
    }

    // PlaySoundEffect: For playing sound effects that only play once
    public void PlaySoundEffect(string soundEffectName)
    {
        // Stores the selected sound file
        AudioClip soundEffect = Resources.Load("Audio/" + soundEffectName) as AudioClip;

        // Plays the sound file at the appropriate volume
        source.PlayOneShot(soundEffect, masterVolume * sfxVolume);

        // Prints the name of the sound file that is being played
        Debug.Log("Now Playing: " + soundEffectName);
    }

    // AddSoundEffect: Setup for sound effects that need to be looped
    public int AddAudio(string audioName)
    {
        // Adds the requested AudioClip to the list
        AudioClip soundEffect = Resources.Load("Audio/" + audioName) as AudioClip;
        audioClips.Add(soundEffect);

        // Sets up the AudioSource (looping enabled)
        AudioSource source = this.AddComponent<AudioSource>();
        source.loop = true;
        audioSources.Add(source);

        // Adds the clip to the source
        source.clip = soundEffect;

        // Returns the index of the added sound effect
        // Store this index in order to call to Play/Pause/Stop
        Debug.Log(audioClips.Count - 1);
        return audioClips.Count - 1;
    }

    // PlayAudio: Plays a looping sound effect at a specified index
    public void PlayAudio(int audioID)
    {
        // Return if audioID is invalid
        if(audioID < 0)
        {
            return;
        }

        audioSources[audioID].Play();
    }

    // PauseAudio: Pauses a looping sound effect at a specified index
    // Pause will allow PlayAudio to resume where it left off
    public void PauseAudio(int audioID)
    {
        // Return if audioID is invalid
        if (audioID < 0)
        {
            return;
        }

        audioSources[audioID].Pause();
    }

    // StopAudio: Stops a looping sound effect at a specified index
    // Stop will cause PlayAudio to restart the audio
    public void StopAudio(int audioID)
    {
        // Return if audioID is invalid
        if (audioID < 0)
        {
            return;
        }

        audioSources[audioID].Stop();
    }

    #region Volume Control
    public void SetMaster(float value)
    {
        masterVolume = value;
    }

    public void SetMusic(float value)
    {
        musicVolume = value;
    }

    public void SetSFX(float value)
    {
        sfxVolume = value;
    }
    #endregion
}
