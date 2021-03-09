using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script was used to create the tutorial video and is only attached to a specific scene in the CutScenes folder
/// </summary>
public class Intro : MonoBehaviour
{
    private GameObject canvas;
    private Text welcomeText;
    private Text titleText;
    private Text getStartedText;

    private readonly float typingSpeed = 0.08f;
    private string welcomeMessage = "Hello, and welcome to";
    private string title = "2 Foxes and the Puzzling Forest!";
    private string getStartedMessage = "We're very excited to show you the ropes, so let's get started!";
    private string curMessage = "";

    private bool startMessage = true;

    // Start is called before the first frame update
    void Awake()
    {
        canvas = GameObject.Find("Intro_Canvas");
        welcomeText = canvas.transform.Find("Welcome").gameObject.GetComponent<Text>();
        titleText = canvas.transform.Find("Title").gameObject.GetComponent<Text>();
        getStartedText = canvas.transform.Find("GetStarted").gameObject.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (startMessage)
        {
            startMessage = false;
            StartCoroutine(PrintText());
        }
    }

    private IEnumerator PrintText()
    {
        //Welcome
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < welcomeMessage.Length; i++)
        {
            curMessage += welcomeMessage[i];
            welcomeText.text = curMessage + "|";
            yield return new WaitForSeconds(typingSpeed);
        }
        welcomeText.text = curMessage;

        //Title
        curMessage = "";
        for (int i = 0; i < title.Length; i++)
        {
            curMessage += title[i];
            titleText.text = curMessage + "|";
            yield return new WaitForSeconds(typingSpeed);
        }
        titleText.text = curMessage;

        //Let's Get Started
        yield return new WaitForSeconds(0.75f);
        curMessage = "";
        for (int i = 0; i < getStartedMessage.Length; i++)
        {
            curMessage += getStartedMessage[i];
            getStartedText.text = curMessage + "|";
            yield return new WaitForSeconds(typingSpeed);
        }
        getStartedText.text = curMessage;

        yield break;
    }
}
