using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Please See TutorialScript.cs for notes on these bad boys.
/// </summary>
public class Tutorial_01 : TutorialScript
{
    private Vector3 startLocation = new Vector3(1f, 1.89f, 2.45f);
    private Vector3 houseLocation = new Vector3(-2.5f, 2.73000002f, -0.270000011f);
    private Vector3 foxLocation = new Vector3(5.48999977f, 6.57000017f, 3.13000011f);

    private GameObject arrowHouse;
    private GameObject arrowFox;
    private UnityEngine.UI.Button nextButton;

    private string welcomeMessage = "Hello and welcome to 2 Foxes and the Puzzling Forest. Call me Summer, I'll show you the ropes!";
    //private string houseMessage = "This is a house. It's quite lonely without occupants.";
    //private string foxMessage = "This is a fox. It's quite unsheltered without a house. Help me guide the fox to the house!";
    private string houseMessage = "This house is the goal.";
    private string foxMessage = "Help me guide this fox to the house!";

    private const int numSections = 4;
    private int curSection = 0;
    private string[] sections = new string[numSections] { "Welcome", "House", "Fox", "JustTips" };
    private float timeAtLastPress = -1f;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        GameObject curChild;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            curChild = this.transform.GetChild(i).gameObject;
            switch (curChild.name)
            {
                case "ArrowHouse":
                    arrowHouse = curChild;
                    break;

                case "ArrowFox":
                    arrowFox = curChild;
                    break;

                default:
                    break;
            }
        }

        nextButton = FairyCanvas.transform.Find("NextButton").GetComponent<UnityEngine.UI.Button>();
    }

    protected override void Start()
    {
        StartNextSection();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void StartNextSection()
    {
        if (Time.time - timeAtLastPress < 1f)
            return;
        else
        {
            timeAtLastPress = Time.time;
            StartCoroutine(sections[curSection]);
            curSection++;
        }
    }

    private IEnumerator Welcome()
    {
        yield return rapidPause;
        turnManager.StealControl();
        timer.SetPause(true);
        SetNewPosition(startLocation, true);
        FairyAnchor.SetActive(true);
        nextButton.interactable = false;


        yield return StartCoroutine(Type(FairyText, welcomeMessage));
        nextButton.interactable = true;
    }

    private IEnumerator House()
    {
        ClearText(FairyText);
        FairyController.FlyOff();
        yield return flyPause;

        FairyCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 1200);
        SetNewPosition(houseLocation, false);
        nextButton.interactable = false;
        FairyController.FlyDown();
        yield return flyPause;

        ShowArrow(arrowHouse, true);
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(Type(FairyText, houseMessage));
        nextButton.interactable = true;
    }

    private IEnumerator Fox()
    {
        ClearText(FairyText);
        ShowArrow(arrowHouse, false);

        FairyController.FlyOff();
        yield return flyPause;

        FairyCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(2500, 1200);
        SetNewPosition(foxLocation, true);
        nextButton.interactable = false;
        FairyController.FlyDown();
        yield return flyPause;

        ShowArrow(arrowFox, true);
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(Type(FairyText, foxMessage));
        nextButton.interactable = true;
    }

    private IEnumerator JustTips()
    {
        ClearText(FairyText);
        ShowArrow(arrowFox, false);

        FairyController.FlyOff();
        yield return flyPause;

        FairyAnchor.SetActive(false);

        turnManager.ResumeControl();
        timer.SetPause(false);

        turnManager.Say("A/D turns the fox.\nW moves forwards.");

        yield return new WaitForSeconds(3f);
        TipsCanvas.transform.Find("TipsMenu/Tip_01").gameObject.SetActive(true);
        TipsCanvas.transform.Find("TipsMenu/Tip_02").gameObject.SetActive(true);
        if (!isTipsShown)
            ToggleTips();
        alertNoise.Play();

        yield return new WaitForSeconds(3f);
        turnManager.Say("Try pushing the block to complete the bridge.");
        yield return new WaitForSeconds(3f);
        TipsCanvas.transform.Find("TipsMenu/Tip_03").gameObject.SetActive(true);
        if (isTipsShown)
            alertNoise.Play();
    }

    private IEnumerator GoalOfGame()
    {
        SetNewPosition(houseLocation, false);
        FairyController.FlyDown();
        yield return flyPause;

        ShowArrow(arrowHouse, true);
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(Type(FairyText, houseMessage));
        yield return new WaitForSeconds(2f);
        ClearText(FairyText);
        ShowArrow(arrowHouse, false);

        FairyController.FlyOff();
        yield return flyPause;



        SetNewPosition(foxLocation, true);
        FairyController.FlyDown();
        yield return flyPause;

        ShowArrow(arrowFox, true);
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(Type(FairyText, foxMessage));
        yield return new WaitForSeconds(2f);
        ClearText(FairyText);
        ShowArrow(arrowFox, false);

        FairyController.FlyOff();
        yield return flyPause;

        FairyAnchor.SetActive(false);
        StartCoroutine(JustTips());
    }

    
}
