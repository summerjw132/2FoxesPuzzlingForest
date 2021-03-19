using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private GameObject timeUI = null;
    private Text timeText = null;

    [HideInInspector]
    public bool isLevelComplete;

    private float timeAtStartOfThisLevel;

    private int minutes = 0;
    private int seconds = 0;
    private float curTime;
    private string displayTime = "00:00";

    private bool paused = false;
    private WaitForSeconds oneSecond;

    public int totaltime {get{return seconds;} set{}}

    void Awake()
    {
        timeText = timeUI.GetComponent<Text>();
        oneSecond = new WaitForSeconds(1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        timeAtStartOfThisLevel = Time.time;
        StartCoroutine(KeepTime());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator KeepTime()
    {
        while (true)
        {
            if (paused)
            {

            }

            else
            {
                curTime = Time.time - timeAtStartOfThisLevel + 55;

                minutes = 0;
                for (int i = (int)curTime; i >= 60; i = i - 60)
                {
                    minutes++;
                }
                seconds = (int)(curTime - (minutes * 60f));
                if (minutes > 9)
                {
                    if (seconds > 9)
                        displayTime = minutes.ToString() + ":" + seconds.ToString();
                    else
                        displayTime = minutes.ToString() + ":0" + seconds.ToString();
                }
                else
                {
                    if (seconds > 9)
                        displayTime = "0" + minutes.ToString() + ":" + seconds.ToString();
                    else
                        displayTime = "0" + minutes.ToString() + ":0" + seconds.ToString();
                }
            }

            timeText.text = displayTime;
            yield return oneSecond;
        }
    }

    public void ResetTimer()
    {
        timeAtStartOfThisLevel = Time.time;
        minutes = 0;
    }

    public void TogglePause()
    {
        paused = !paused;
    }

    public void SetPause(bool val)
    {
        paused = val;
    }
}
