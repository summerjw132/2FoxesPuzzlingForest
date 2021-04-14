using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the parent class for all of the Tutorial scripts. I'm not going to lie, since these are used in individual levels,
///  it's not very re-usable, encapsulated code. Instead, these scripts pretty much manually call things when they need to happen and then
///  wait for hard coded intervals before moving on. 
///  
/// Open up any Tutorial Scene and you'll see a "Tutorial_??" Game Object. Those only exist on the tutorial levels. This script (or one of
///  its children) are attached to the DriftAnchor that is a part of the Tutorial Game Object.
///  
/// There's not much to say other than that. You'll generally have to go line by line to see what's happening, but the flip side to that coin
///  is that there's no complicated inter-weaving of systems to worry about!
/// </summary>
public class TutorialScript : MonoBehaviour
{
    protected TurnManager turnManager;
    protected GameObject FairyAnchor;
    protected GameObject Fairy;
    protected TutFairyController FairyController;
    protected GameObject FairyCanvas;
    protected Text FairyText;
    protected GameObject TipsCanvas;

    protected AudioSource typingNoise;
    protected AudioSource alertNoise;

    protected readonly float typingSpeed = 0.065f;
    protected readonly float puncSpeed = 0.5f;

    protected WaitForSeconds typingPause;
    protected WaitForSeconds puncPause;
    protected WaitForSeconds rapidPause = new WaitForSeconds(0.05f);
    protected WaitForSeconds flyPause = new WaitForSeconds(0.75f);

    protected Timer timer;

    protected bool isTipsShown = false;

    protected virtual void Awake()
    {
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();

        typingPause = new WaitForSeconds(typingSpeed);
        puncPause = new WaitForSeconds(puncSpeed);

        GameObject curChild;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            curChild = this.transform.GetChild(i).gameObject;
            switch (curChild.name)
            {
                case "DriftAnchor":
                    FairyAnchor = curChild;
                    break;

                case "TypingNoise":
                    typingNoise = curChild.GetComponent<AudioSource>();
                    break;

                case "ArrowNoise":
                    alertNoise = curChild.GetComponent<AudioSource>();
                    break;

                case "TipsCanvas":
                    TipsCanvas = curChild;
                    break;

                default:
                    break;
            }
        }

        Fairy = FairyAnchor.transform.Find("Fairy").gameObject;
        FairyController = Fairy.GetComponent<TutFairyController>();
        FairyCanvas = FairyAnchor.transform.Find("FairyCanvas").gameObject;
        try
        {
            FairyText = FairyCanvas.transform.Find("FairyText").GetComponent<Text>();
        }
        catch (System.NullReferenceException)
        {
            FairyText = FairyCanvas.transform.Find("Background/FairyText").GetComponent<Text>();
        }
        

        timer = GameObject.Find("Turn-Based System").GetComponent<Timer>();
    }

    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    //Used for the typing effect for the dialogue.
    protected IEnumerator Type(Text text, string msg)
    {
        //if (typingNoise.volume > 0.05f)
        //    typingNoise.volume = 0.05f;
        text.text = "";
        string current = "";

        for (int i = 0; i < msg.Length; i++)
        {
            current += msg[i];
            text.text = current + "|";
            typingNoise.Play();
            if (msg[i] == '.' || msg[i] == '!')
                yield return puncPause;
            else
                yield return typingPause;
        }
        text.text = current;
    }

    protected void ClearText(Text text)
    {
        text.text = "";
    }

    protected void SetNewPosition(Vector3 newPos, bool isTextAbove)
    {
        FairyAnchor.transform.position = newPos;

        Fairy.transform.localPosition = new Vector3(0f, 0f, 0f);

        if (!isTextAbove)
        {
            FairyCanvas.transform.localPosition = new Vector3(0.59f, -1.18f, 0.69f);
        }
        else
            FairyCanvas.transform.localPosition = new Vector3(-0.909f, 0.94f, -0.809f);
    }

    //Plays the alert noise and turns the given arrow visible. Used in some tutorial levels to point to places.
    protected void ShowArrow(GameObject arrow, bool val)
    {
        arrow.SetActive(val);
        if (val)
            alertNoise.Play();
    }

    //Used in conjunction with the on-screen toggle button to hide/show tips during tutorials.
    public void ToggleTips()
    {
        isTipsShown = !isTipsShown;

        if (isTipsShown)
        {
            TipsCanvas.transform.Find("TipsMenu").gameObject.SetActive(true);
            TipsCanvas.transform.Find("TipsToggle").transform.Find("Toggle/Background/Checkmark").gameObject.SetActive(false);
        }
        else
        {
            TipsCanvas.transform.Find("TipsMenu").gameObject.SetActive(false);
            TipsCanvas.transform.Find("TipsToggle").transform.Find("Toggle/Background/Checkmark").gameObject.SetActive(true);
        }
    }
}
