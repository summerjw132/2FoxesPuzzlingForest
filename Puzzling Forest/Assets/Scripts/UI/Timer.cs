using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private GameObject timeUI;

    private int timer;
    private float rt;
    [HideInInspector]
    public bool isLevelComplete;

    public int totaltime {get{return timer;} set{}}

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;      
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLevelComplete)
        {
            rt = Time.time;
            timer = (int)rt;
        }
        timeUI.GetComponent<Text>().text = timer.ToString();
    }
}
