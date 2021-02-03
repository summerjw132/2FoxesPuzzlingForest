using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningMessagesController : MonoBehaviour
{
    private GameObject warningPanel;
    private Text warningText;

    private static string WarningMessagePanelName = "WarningMessagesPanel";

    private static float duration = 2f;
    private float startTime;
    private bool isActive = false;


    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
            {
            if (this.transform.GetChild(i).name == WarningMessagePanelName)
            {
                warningPanel = this.transform.GetChild(i).gameObject;
            }
        }

        warningText = warningPanel.transform.GetChild(0).GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isActive)
        {
            if (Time.time - startTime >= duration)
            {
                warningPanel.SetActive(false);
                isActive = false;
            }
        }
    }

    public void Warn(string msg)
    {
        startTime = Time.time;
        warningPanel.SetActive(true);
        warningText.text = msg;
        isActive = true;
    }
}
