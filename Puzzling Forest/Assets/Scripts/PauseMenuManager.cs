using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    private GameObject pauseMenu;
    private GameObject pauseButton;
    private GameObject resetButton;
    private GameObject controlMenu;
    private GameObject audioMenu;

    private GameObject[] PlayerGroup;


    // Start is called before the first frame update
    void Start()
    {
        //The reason I'm doing this instead of using GameObject.Find() is because disabled GOs won't
        // be found. This method will also find disabled GOs
        foreach (Transform child in this.gameObject.transform)
        {
            switch (child.gameObject.name)
            {
                case "PauseMenu":
                    pauseMenu = child.gameObject;
                    break;

                case "LevelSelect_Button":
                    pauseButton = child.gameObject;
                    break;

                case "ResetLevel_Button":
                    resetButton = child.gameObject;
                    break;

                case "Control_Panel":
                    controlMenu = child.gameObject;
                    break;

                case "Audio Menu":
                    audioMenu = child.gameObject;
                    break;

                default:
                    break;
            }    
        }

        StartCoroutine("GetPlayers");
    }

    //give the turnmanager script 0.05 seconds to set up the playergroup before getting it
    private IEnumerator GetPlayers()
    {
        yield return new WaitForSeconds(0.05f);
        PlayerGroup = GameObject.Find("Turn-Based System").GetComponent<TurnManager>().GetPlayers();
        yield break;
    }

    public void openPauseMenu()
    {
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        resetButton.SetActive(false);

        foreach (GameObject player in PlayerGroup)
        {
            player.GetComponent<TurnBasedCharacter>().togglePauseMenuBlock();
        }
    }

    public void closePauseMenu()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        resetButton.SetActive(true);

        controlMenu.SetActive(false);
        audioMenu.SetActive(false);

        foreach (GameObject player in PlayerGroup)
        {
            player.GetComponent<TurnBasedCharacter>().togglePauseMenuBlock();
        }
    }

    public void togglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy || audioMenu.activeInHierarchy || controlMenu.activeInHierarchy)
        {
            closePauseMenu();
        }
        else
        {
            openPauseMenu();
        }
    }

    public void openControlsMenu()
    {
        pauseMenu.SetActive(false);
        controlMenu.SetActive(true);
    }

    public void closeControlsMenu()
    {
        controlMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void openAudioMenu()
    {
        pauseMenu.SetActive(false);
        audioMenu.SetActive(true);
    }

    public void closeAudioMenu()
    {
        audioMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
