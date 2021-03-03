using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private GameObject timeUI = null;

    private int timer;
    [HideInInspector]
    public bool isLevelComplete;

    private float timeAtStartOfThisLevel;

    private float curTime;

    public int totaltime {get{return timer;} set{}}

    // Start is called before the first frame update
    void Start()
    {
        timeAtStartOfThisLevel = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLevelComplete)
        {
            curTime = Time.time - timeAtStartOfThisLevel;
            timer = (int)curTime;
        }
        timeUI.GetComponent<Text>().text = timer.ToString();
    }
}
