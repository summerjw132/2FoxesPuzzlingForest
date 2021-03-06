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
}
