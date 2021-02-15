using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Slider BackgroundMusicSlider;

    [SerializeField]
    private AudioSource BackgroundMusicAudio;


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
