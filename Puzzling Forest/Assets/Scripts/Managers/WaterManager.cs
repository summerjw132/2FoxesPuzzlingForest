using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    Material waterMat;
    [SerializeField] private Vector2 currentDirection = new Vector2(0f, 0f);
    [SerializeField] private float currentSpeed = 0f;

    void Awake()
    {
        waterMat = Resources.Load<Material>("Material/Water");

        waterMat.SetFloat("Vector1_62556560", currentSpeed);
        waterMat.SetVector("Vector2_F45F7D08", currentDirection);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
