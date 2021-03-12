using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{

    [SerializeField]
    public string nextLevel = "level";

    public void GoToNextLevel()
    {
        PlayerPrefs.SetInt("ReachedLevel", PlayerPrefs.GetInt("ReachedLevel") + 1);
        SceneManager.LoadScene(nextLevel);
    }

    public void GoToNextLevelInBuildSettings()
    {
        //Scene editorNextLevel = EditorSceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1);

        try
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        catch { 
            //nothing for now
        }
        
    }
}
