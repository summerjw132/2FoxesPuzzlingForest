using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// This script is just used for running the tutorial scene.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    private GameObject allCanvases;
    private float typingSpeed = 0.08f;

    //Intro video stuff
    private GameObject introCanvas;
    private VideoPlayer introPlayer;
    private GameObject introOutput;
    private GameObject firstFrame;

    //Goal of the game stuff
    private GameObject houseCanvas;
    private Text houseText;
    private Text foxText;
    private Text goalText;
    private readonly string houseString = "This is a house. It's quite lonely without occupants...";
    private readonly string foxString = "This is a fox. It's quite unsheltered without a house...";
    private readonly string goalString = "The goal in 2 Foxes and the Puzzling Forest is to help all of the foxes reach the house. Now we'll show you how to do that!";
    private GameObject continueButton;

    //Controls stuff
    private GameObject controlsCanvas;
    private VideoPlayer controlsPlayer;
    private GameObject controlsOutput;
    private GameObject lastFrame;

    //Controls List
    private GameObject controlsList;

    //Control
    private LevelManager levelManager;
    private List<MyLevel> allLevels;
    private bool justStarted = true;

    // Start is called before the first frame update
    void Awake()
    {
        allCanvases = GameObject.Find("Canvases");

        //This finds all the canvases (one for each section of tutorial)
        for (int i = 0; i < allCanvases.transform.childCount; i++)
        {
            Transform child = allCanvases.transform.GetChild(i);

            switch (child.name)
            {
                case "IntroCanvas":
                    introCanvas = child.gameObject;
                    break;

                case "HouseCanvas":
                    houseCanvas = child.gameObject;
                    break;

                case "ControlsCanvas":
                    controlsCanvas = child.gameObject;
                    break;

                case "ControlsList":
                    controlsList = child.gameObject;
                    break;

                default:
                    break;
            }
        }

        //This gets each component of the IntroCanvas
        for (int i = 0; i < introCanvas.transform.childCount; i++)
        {
            Transform child = introCanvas.transform.GetChild(i);

            switch (child.name)
            {
                case "IntroPlayer":
                    introPlayer = child.gameObject.GetComponent<VideoPlayer>();
                    break;

                case "IntroOutput":
                    introOutput = child.gameObject;
                    break;

                case "FirstFrame":
                    firstFrame = child.gameObject;
                    break;

                default:
                    break;
            }
        }

        //This gets each component of the HouseCanvas
        for (int i = 0; i < houseCanvas.transform.childCount; i++)
        {
            Transform child = houseCanvas.transform.GetChild(i);

            switch (child.name)
            {
                case "HouseText":
                    houseText = child.gameObject.GetComponent<Text>();
                    break;

                case "FoxText":
                    foxText = child.gameObject.GetComponent<Text>();
                    break;

                case "GoalText":
                    goalText = child.gameObject.GetComponent<Text>();
                    break;

                case "Continue":
                    continueButton = child.gameObject;
                    break;

                default:
                    break;
            }
        }

        //Controls stuff
        controlsPlayer = controlsCanvas.transform.Find("ControlsPlayer").gameObject.GetComponent<VideoPlayer>();
        controlsOutput = controlsCanvas.transform.Find("ControlsOutput").gameObject;
        lastFrame = controlsCanvas.transform.Find("LastFrame").gameObject;

        //For loading the next scene
        try
        {
            levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        }
        catch
        {
            Debug.Log("levelManager not found. Expected if you loaded a scene/level directly");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (justStarted)
        {
            justStarted = false;
            StartCoroutine(StartIntro());
        }
    }

    private IEnumerator StartIntro()
    {
        introCanvas.SetActive(true);
        yield return new WaitForSeconds(0.2f);
   
        introOutput.SetActive(true);
        introPlayer.Play();
        firstFrame.SetActive(false);

        yield return new WaitForSeconds((float)introPlayer.length);

        introCanvas.SetActive(false);
        StartCoroutine(StartGoalTutorial());
    }

    private IEnumerator StartGoalTutorial()
    {
        houseCanvas.SetActive(true);
        string msg = "";

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < houseString.Length; i++)
        {
            msg += houseString[i];
            houseText.text = msg + "|";
            if (i == 15)
            {
                yield return new WaitForSeconds(1.5f);
            }                
            yield return new WaitForSeconds(typingSpeed);
        }
        houseText.text = msg;

        yield return new WaitForSeconds(2f);
        msg = "";
        for (int i = 0; i < foxString.Length; i++)
        {
            msg += foxString[i];
            foxText.text = msg + "|";
            if (i == 13)
            {
                yield return new WaitForSeconds(1.5f);
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        foxText.text = msg;

        yield return new WaitForSeconds(2f);
        msg = "";
        for (int i = 0; i < goalString.Length; i++)
        {
            msg += goalString[i];
            goalText.text = msg + "|";
            if (goalString[i] == '.')
                yield return new WaitForSeconds(1.5f);
            yield return new WaitForSeconds(typingSpeed);
        }
        goalText.text = msg;

        yield return new WaitForSeconds(2f);
        continueButton.SetActive(true);
    }

    private IEnumerator StartControlsTutorial()
    {
        controlsCanvas.SetActive(true);
        houseCanvas.SetActive(false);

        controlsOutput.SetActive(true);
        controlsPlayer.Play();
        lastFrame.SetActive(false);

        yield return new WaitForSeconds((float)controlsPlayer.length);

        controlsOutput.SetActive(false);
        lastFrame.SetActive(true);
    }

    public void Continue()
    {
        StartCoroutine(StartControlsTutorial());
    }

    public void StartGame()
    {
        if (levelManager)
        {
            allLevels = levelManager.giveList();

            UnityEngine.SceneManagement.SceneManager.LoadScene(allLevels[0].LevelName);
        }
    }

    public void ShowControls()
    {
        controlsCanvas.SetActive(false);
        controlsList.SetActive(true);
    }
}
