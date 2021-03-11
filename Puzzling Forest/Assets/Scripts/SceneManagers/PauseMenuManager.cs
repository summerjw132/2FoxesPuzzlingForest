using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    //Menu stuff
    private GameObject pauseMenu;
    private GameObject pauseButton;
    private GameObject resetButton;
    private GameObject controlMenu;
    private GameObject audioMenu;

    //Control-lock when menu is open stuff
    private TurnManager turnManager;
    private CameraMovement camScript;

    void Awake()
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

        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();

        camScript = GameObject.Find("CameraControls").GetComponent<CameraMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void openPauseMenu()
    {
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        resetButton.SetActive(false);
    }

    public void closePauseMenu()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        resetButton.SetActive(true);

        controlMenu.SetActive(false);
        audioMenu.SetActive(false);
    }

    public void togglePauseMenu()
    {
        turnManager.TogglePauseMenu();
        camScript.TogglePauseLock();

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
