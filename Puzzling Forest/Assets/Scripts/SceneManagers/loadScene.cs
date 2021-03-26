using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour
{
    public string LevelName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Load()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(LevelName);
    }
}
