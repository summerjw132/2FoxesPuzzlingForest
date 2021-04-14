using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script handles updating the volume for all of the noises based on the audio settings menu. Also uses PlayerPrefs to
///  keep this constant between sessions.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private Slider MasterSlider = null;
    [SerializeField] private Slider MusicSlider = null;
    [SerializeField] private Slider SFXSlider = null;

    //All audio sources
    private GameObject[] allAudioSources;
    //All audio sources tagged "Music"
    private GameObject[] musicSources;
    //All audio sources tagged "SFX"
    private GameObject[] sfxSources;

    private int musicLength = 0;
    private int sfxLength = 0;
    private int allSourcesLength = 0;

    void Awake()
    {
        musicSources = GameObject.FindGameObjectsWithTag("Music");
        sfxSources = GameObject.FindGameObjectsWithTag("SFX");

        musicLength = musicSources.Length;
        sfxLength = sfxSources.Length;
        allSourcesLength = musicLength + sfxLength;
        allAudioSources = new GameObject[allSourcesLength];

        for (int i = 0; i < musicLength; i++)
        {
            allAudioSources[i] = musicSources[i];
        }
        for (int i = 0; i < sfxLength; i++)
        {
            allAudioSources[musicLength + i] = sfxSources[i];
            //if (sfxSources[i].name == "typingNoise")
            //    Debug.LogFormat("AudioManager just found: {0}", sfxSources[i].transform.parent.transform.parent.name);
            //else
            //    Debug.LogFormat("AudioManager just found: {0}", sfxSources[i]);
        }
    }

    void Start()
    {
        InitializeSliders();
    }

    // Iterates through the audio sources and updates their volumes based on the slider values.
    public void UpdateVolume()
    {
        float musicVolume = MasterSlider.value * MusicSlider.value;
        float sfxVolume = MasterSlider.value * SFXSlider.value;

        for (int i = 0; i < musicLength; i++)
        {
            musicSources[i].GetComponent<AudioSource>().volume = musicVolume;
        }

        for (int i = 0; i < sfxLength; i++)
        {
            sfxSources[i].GetComponent<AudioSource>().volume = sfxVolume;
        }
    }

    // These functions update the PlayerPrefs based on the slider values and then update the volumes accordingly.
    public void UpdateMaster()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterSlider.value);
        PlayerPrefs.Save();

        UpdateVolume();
    }

    public void UpdateMusic()
    {
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
        PlayerPrefs.Save();

        UpdateVolume();
    }

    public void UpdateSFX()
    {
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
        PlayerPrefs.Save();

        UpdateVolume();
    }

    /// <summary>
    /// Updates the sliders in the audio menu to reflect the values from the playerprefs.
    ///  Also updates the actual volume of the audio sources accordingly.
    /// </summary>
    private void InitializeSliders()
    {
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            //Debug.LogFormat("Has SFX Key: {0}", PlayerPrefs.GetFloat("SFXVolume"));
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
        else
            SFXSlider.value = 1.0f;

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            //Debug.LogFormat("Has Music Key: {0}", PlayerPrefs.GetFloat("MusicVolume"));
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
            MusicSlider.value = 1.0f;

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            //Debug.LogFormat("Has Master Key: {0}", PlayerPrefs.GetFloat("MasterVolume"));
            MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
        else
            MasterSlider.value = 0.1f;

        UpdateVolume();
    }

    //Debugging function for when an audio source just doesn't want to be found
    public void ShowSources()
    {
        for (int i = 0; i < allSourcesLength; i++)
        {
            Debug.LogFormat("Source{0}: {1} - Type: {2}", i, allAudioSources[i], allAudioSources[i].tag);
        }
    }

}
