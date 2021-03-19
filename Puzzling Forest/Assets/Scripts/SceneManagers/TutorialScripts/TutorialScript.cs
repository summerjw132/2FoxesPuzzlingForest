using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
    protected TurnManager turnManager;
    protected GameObject FairyAnchor;
    protected GameObject Fairy;
    protected IndicatorAnimationController FairyController;
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

    protected bool isTipsShown = true;

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
        FairyController = Fairy.GetComponent<IndicatorAnimationController>();
        FairyCanvas = FairyAnchor.transform.Find("FairyCanvas").gameObject;
        FairyText = FairyCanvas.transform.Find("FairyText").GetComponent<Text>();

        timer = GameObject.Find("Turn-Based System").GetComponent<Timer>();
    }

    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected IEnumerator Type(Text text, string msg)
    {
        if (typingNoise.volume > 0.05f)
            typingNoise.volume = 0.05f;
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

    protected void ShowArrow(GameObject arrow, bool val)
    {
        arrow.SetActive(val);
        if (val)
            alertNoise.Play();
    }

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
