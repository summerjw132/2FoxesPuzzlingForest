using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ResetScene : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //ResetCurrentScene();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            QuitGameInEditor();
        }
    }
    public void ResetCurrentScene()
    {
        //For TEST LOGS
        //Adding a delay here because we now want to log user test data. Remove this after test builds.
        // The logging method itself is handled in the turn manager.
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(DelayReset());
    }

    //For TEST LOGS
    private IEnumerator DelayReset()
    {
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void QuitGame()
    {
        #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void QuitGameInEditor()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
