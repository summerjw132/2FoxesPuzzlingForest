using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_05 : TutorialScript
{
    private UnityEngine.UI.Button nextButton;

    private string welcomeMessage = "Whoa! Now there's two foxes. We'll have to help them both to reach the house!";

    private const int numSections = 2;
    private int curSection = 0;
    private string[] sections = new string[numSections] { "Welcome", "JustTips" };
    private float timeAtLastPress = -1f;

    private bool isFirstBlockDone = false;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        nextButton = FairyCanvas.transform.Find("Background/NextButton").GetComponent<UnityEngine.UI.Button>();
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

        FairyAnchor.SetActive(true);
        nextButton.interactable = false;

        yield return StartCoroutine(Type(FairyText, welcomeMessage));
        nextButton.interactable = true;
    }

    private IEnumerator JustTips()
    {
        ClearText(FairyText);

        FairyController.FlyOff();
        yield return flyPause;

        FairyAnchor.SetActive(false);

        turnManager.ResumeControl();
        timer.SetPause(false);

        yield return new WaitForSeconds(turnManager.Say("Press 'E' to change which fox we're helping.", typingNoise) - 3f);

        TipsCanvas.transform.Find("TipsMenu/Tip_01").gameObject.SetActive(true);
        TipsCanvas.transform.Find("TipsMenu/Tip_02").gameObject.SetActive(true);
        if (!isTipsShown)
            ToggleTips();
        alertNoise.Play();
    }    

    void OnTriggerEnter(Collider other)
    {
        if (!isFirstBlockDone)
        {
            isFirstBlockDone = true;
            turnManager.Say("One down, one to go.", typingNoise);
        }
        else
        {
            turnManager.Say("Great job.", typingNoise);
        }
    }
}
