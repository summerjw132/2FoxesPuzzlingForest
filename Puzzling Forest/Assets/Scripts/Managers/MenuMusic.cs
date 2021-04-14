using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusic : MonoBehaviour
{
    private static GameObject instance;
    private AudioSource source;
    private readonly string[] menuNames = new string[3] { "Main Menu 1", "Level Select", "Credits Menu" };

    // Start is called before the first frame update
    void Awake()
    {
        //These 5 lines ensure there's always one instance of this script
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this.gameObject;
        else
            Destroy(this.gameObject);

        source = instance.transform.Find("BGM").GetComponent<AudioSource>();
        source.loop = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (MenuNamesContains(scene.name))
        {
            Play();
        }
        else
        {
            Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Play()
    {
        if (source.isPlaying)
            return;
        source.Play();
    }

    private void Stop()
    {
        source.Stop();
    }

    private bool MenuNamesContains(string str)
    {
        for (int i = 0; i < menuNames.Length; i++)
        {
            if (menuNames[i] == str)
                return true;
        }
        return false;
    }
}
