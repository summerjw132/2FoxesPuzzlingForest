using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHider : MonoBehaviour
{
    private GameObject water;

    void Awake()
    {
        water = this.transform.parent.transform.GetChild(0).gameObject;
        Debug.LogFormat("Me: {0}\tMy water: {1}", this.transform.parent.name, water.name);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter()
    {
        water.SetActive(false);
    }

    void OnTriggerExit()
    {
        water.SetActive(true);
    }
}
