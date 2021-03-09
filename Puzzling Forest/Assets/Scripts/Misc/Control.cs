using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// This script was used to create the video in the tutorial for showing the controls. It is only attached to one scene in the CutScene folder.
/// </summary>
public class Control : MonoBehaviour
{
    private GameObject allCanvases;
    private float typingSpeed = 0.08f;
    private bool justStarted = true;

    //KeyPressIndicator
    private GameObject keyPressCanvas;
    private Text keyText;
    private KeyCode[] keysToWatchFor = new KeyCode[9] { KeyCode.W, KeyCode.A, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.U, KeyCode.Escape, KeyCode.C, KeyCode.R};

    //Indicator explanation
    private GameObject indicatorCanvas;
    private Text indicatorText;
    private readonly string indicatorString = "This yellow ball indicates which fox is being controlled.";
    private readonly string foxSwapString = "Press 'E' to change which fox you're controlling.";

    //Movement controls
    private GameObject moveCanvas;
    private Text moveText;
    private readonly string moveString = "Press 'A' or 'D' to turn the fox left or right, respectively.";
    private readonly string moveString2 = "Press 'W' to move the fox forward in the direction it's facing."; 
    private readonly string moveString3 = "You can push some blocks like this!";

    //Undo controls
    private GameObject undoCanvas;
    private Text undoText;
    private readonly string undoString = "Press 'U' to undo your move.";


    // Start is called before the first frame update
    void Awake()
    {
        allCanvases = GameObject.Find("AllCanvases");

        //Finds each canvas
        for (int i = 0; i < allCanvases.transform.childCount; i++)
        {
            Transform child = allCanvases.transform.GetChild(i);

            switch (child.name)
            {
                case "KeyPressCanvas":
                    keyPressCanvas = child.gameObject;
                    break;

                case "IndicatorCanvas":
                    indicatorCanvas = child.gameObject;
                    break;

                case "MoveCanvas":
                    moveCanvas = child.gameObject;
                    break;

                case "UndoCanvas":
                    undoCanvas = child.gameObject;
                    break;

                default:
                    break;
            }
        }

        //key press indicator
        keyText = keyPressCanvas.transform.Find("KeyText").gameObject.GetComponent<Text>();

        //indicator
        indicatorText = indicatorCanvas.transform.Find("IndicatorText").gameObject.GetComponent<Text>();

        //move
        moveText = moveCanvas.transform.Find("MoveText").gameObject.GetComponent<Text>();

        //undo
        undoText = undoCanvas.transform.Find("UndoText").gameObject.GetComponent<Text>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (justStarted)
        {
            justStarted = false;
            StartCoroutine(IndicatorSection());
        }
        detectKeyPress();
    }

    private IEnumerator IndicatorSection()
    {
        yield return new WaitForSeconds(5f);
        indicatorCanvas.SetActive(true);
        string msg = "";

        for (int i = 0; i < indicatorString.Length; i++)
        {
            msg += indicatorString[i];
            indicatorText.text = msg + "|";
            yield return new WaitForSeconds(typingSpeed);
        }
        indicatorText.text = msg;

        yield return new WaitForSeconds(2f);
        msg = "";
        for (int i = 0; i < foxSwapString.Length; i++)
        {
            msg += foxSwapString[i];
            indicatorText.text = msg + "|";
            yield return new WaitForSeconds(typingSpeed);
        }
        indicatorText.text = msg;

        yield return new WaitForSeconds(4f);
        indicatorCanvas.SetActive(false);
        StartCoroutine(MoveSection());
    }

    private IEnumerator MoveSection()
    {
        moveCanvas.SetActive(true);
        string msg = "";

        for (int i = 0; i < moveString.Length; i++)
        {
            msg += moveString[i];
            moveText.text = msg + "|";
            yield return new WaitForSeconds(typingSpeed);
        }
        moveText.text = msg;

        yield return new WaitForSeconds(4f);
        msg = "";
        for (int i = 0; i < moveString2.Length; i++)
        {
            msg += moveString2[i];
            moveText.text = msg + "|";
            yield return new WaitForSeconds(typingSpeed);
        }
        moveText.text = msg;

        yield return new WaitForSeconds(4f);
        msg = "";
        for (int i = 0; i < moveString3.Length; i++)
        {
            msg += moveString3[i];
            moveText.text = msg + "|";
            yield return new WaitForSeconds(typingSpeed);
        }
        moveText.text = msg;

        yield return new WaitForSeconds(4f);
        moveCanvas.SetActive(false);
        StartCoroutine(UndoSection());
    }

    private IEnumerator UndoSection()
    {
        undoCanvas.SetActive(true);
        string msg = "";

        for (int i = 0; i < undoString.Length; i++)
        {
            msg += undoString[i];
            undoText.text = msg + "|";
            yield return new WaitForSeconds(typingSpeed);
        }
        undoText.text = msg;

        yield return new WaitForSeconds(4f);
        undoCanvas.SetActive(false);
    }

    private IEnumerator ShowKey(KeyCode key)
    {
        keyText.text = " Just Pressed: " + key;
        yield return new WaitForSeconds(0.5f);
        keyText.text = " Just Pressed: ";
    }

    private void detectKeyPress()
    {
        foreach (KeyCode keyCode in keysToWatchFor)
        {
            if (Input.GetKeyDown(keyCode))
            {
                StartCoroutine(ShowKey(keyCode));
            }
        }
    }
}
