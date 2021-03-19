using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Slider MasterSlider = null;
    [SerializeField] private Slider MusicSlider = null;
    [SerializeField] private Slider SFXSlider = null;

    private GameObject[] allAudioSources;
    private GameObject[] musicSources;
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
        }
    }

    void Start()
    {
        InitializeSliders();
    }

    public void ShowSources()
    {
        for (int i = 0; i < allSourcesLength; i++)
        {
            Debug.LogFormat("Source{0}: {1} - Type: {2}", i, allAudioSources[i], allAudioSources[i].tag);
        }
    }

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
    }
}
