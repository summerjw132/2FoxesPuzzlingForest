using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Please See TutorialScript.cs for notes on these bad boys.
/// </summary>
public class Tutorial_02 : TutorialScript
{
    private Vector3 newLeftSide = new Vector3(1.42f, 1.29f, -4.56f);
    private Vector3 newRightSide = new Vector3(-4.66f, 1.29f, 1.52f);

    private GameObject arrowOpenSpace;

    private bool alreadyTriggered = false;

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
                case "ArrowOpenSpace":
                    arrowOpenSpace = curChild;
                    break;

                default:
                    break;
            }
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        StartCoroutine(Begin());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private IEnumerator Begin()
    {
        yield return new WaitForSeconds(0.1f);
        FairyController = turnManager.GetCurrentFairy().GetComponent<TutFairyController>();
        //FairyController.ResizeCanvas(850f, 375f);

        yield return new WaitForSeconds(turnManager.Say("Sometimes you have to carefully maneuver blocks.") + 0.1f);
		StartCoroutine(ShowWhere());
    }

    private IEnumerator ShowWhere()
    {
        ShowArrow(arrowOpenSpace, true);

        yield return new WaitForSeconds(turnManager.Say("Move the block here first in order to turn it around!") - 3f);

        TipsCanvas.transform.Find("TipsMenu/Tip_01").gameObject.SetActive(true);
        if (!isTipsShown)
            ToggleTips();
        alertNoise.Play();
    }

    private IEnumerator GoodJob()
    {
        ShowArrow(arrowOpenSpace, false);
        yield return new WaitForSeconds(turnManager.Say("Great! Now use the block to complete the bridge.") + 0.1f);
    }

    private IEnumerator GoodJob_2()
    {
        yield return new WaitForSeconds(turnManager.Say("Well done!") + 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") || other.CompareTag("ScriptTrigger"))
            return;
        if (!alreadyTriggered)
        {
            Debug.Log("TriggerEnter by: " + other.name);
            alreadyTriggered = true;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(GoodJob());
        }
        else
        {
            this.gameObject.GetComponent<SphereCollider>().enabled = false;
            StartCoroutine(GoodJob_2());
        }
    }
}
