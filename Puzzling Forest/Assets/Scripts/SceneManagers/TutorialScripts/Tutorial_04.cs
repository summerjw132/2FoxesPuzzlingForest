using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_04 : TutorialScript
{
	private Vector3 newLeftSide = new Vector3(1.42f, 1.29f, -4.56f);
	private Vector3 newRightSide = new Vector3(-4.66f, 1.29f, 1.52f);


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
		//FairyController.resizeCanvas(850f, 375f);

		yield return new WaitForSeconds(turnManager.Say("This puzzle is a bit trickier.") - 0.1f);

		yield return new WaitForSeconds(turnManager.Say("If you get stuck, you can press 'U' to undo your most recent move.") + 0.1f);
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
