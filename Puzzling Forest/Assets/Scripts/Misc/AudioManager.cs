using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Slider BackgroundMusicSlider = null;

    [SerializeField]
    private AudioSource BackgroundMusicAudio = null;


    // Start is called before the first frame update
    void Start()
    {
        BackgroundMusicSlider.value = .7f;
    }

    public void SetBackgroundMusicValue()
    {
        BackgroundMusicAudio.volume = BackgroundMusicSlider.value;
    }
}
