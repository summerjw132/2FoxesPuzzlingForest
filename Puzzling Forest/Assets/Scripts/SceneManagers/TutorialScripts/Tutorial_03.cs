using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Please See TutorialScript.cs for notes on these bad boys.
/// </summary>
public class Tutorial_03 : TutorialScript
{
    private Vector3 newLeftSide = new Vector3(1.42f, 1.29f, -4.56f);
    private Vector3 newRightSide = new Vector3(-4.66f, 1.29f, 1.52f);

    protected override void Awake()
    {
        base.Awake();

        //GameObject curChild;
        //for (int i = 0; i < this.transform.childCount; i++)
        //{
        //    curChild = this.transform.GetChild(i).gameObject;
        //    switch (curChild.name)
        //    {
        //        default:
        //            break;
        //    }
        //}
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
        //FairyController.resizeCanvas(850f, 375f);

        yield return new WaitForSeconds(turnManager.Say("Sometimes you'll need to push multiple blocks in a line.") + 0.1f);
        TipsCanvas.transform.Find("TipsMenu/Tip_01").gameObject.SetActive(true);
        if (!isTipsShown)
            ToggleTips();
        alertNoise.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        turnManager.Say("Great job!");
    }
}
