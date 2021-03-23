using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_FoxHole : TutorialScript
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
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
        FairyController = turnManager.GetCurrentFairy().GetComponent<IndicatorAnimationController>();

		yield return new WaitForSeconds(turnManager.Say("Hello again! There's something new here.", typingNoise) + 0.1f);

		yield return new WaitForSeconds(turnManager.Say("Those black circles are 'Fox Holes'.", typingNoise) + 0.5f);
		yield return new WaitForSeconds(turnManager.Say("They allow you to travel between them instantly!", typingNoise) + 0.1f);
		TipsCanvas.transform.Find("TipsMenu/Tip_01").gameObject.SetActive(true);
        if (!isTipsShown)
            ToggleTips();
        alertNoise.Play();

        yield return new WaitForSeconds(turnManager.Say("Just walk over one and press 'F' to move through them.", typingNoise) + 0.1f);
        TipsCanvas.transform.Find("TipsMenu/Tip_02").gameObject.SetActive(true);
        if (isTipsShown)
            alertNoise.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        turnManager.Say("Great job!", typingNoise);
    }
}
