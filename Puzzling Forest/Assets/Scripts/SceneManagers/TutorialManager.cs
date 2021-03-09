using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
    private readonly string goalString = "The goal in 2 Foxes and the Puzzling Forest is to help all of the foxes reach the house. Let's cover how to do that!";

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

                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (justStarted)
        {
            justStarted = false;
            //StartCoroutine(StartGoalTutorial());
            StartCoroutine(StartIntro());
        }
    }

    private IEnumerator StartIntro()
    {
        yield return new WaitForSeconds(0.2f);
   
        introOutput.SetActive(true);
        introPlayer.Play();
        firstFrame.SetActive(false);

        yield return new WaitForSeconds((float)introPlayer.length);

        StartCoroutine(StartGoalTutorial());
    }

    private IEnumerator StartGoalTutorial()
    {
        introCanvas.SetActive(false);
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
    }
}
