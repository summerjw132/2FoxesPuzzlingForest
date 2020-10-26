using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObject : MonoBehaviour
{
    [SerializeField]
    private float pushForce = 5f;
    [SerializeField]
    private Camera camera;

    private Ray ray;
    private RaycastHit hit;

    private void Start()
    {
        
    }
    private void Update()
    {
        AddForceToPushableObjectViaMouseClick();
    }

    private void AddForceToPushableObjectViaMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("Clicked from " + this.gameObject.transform.position + " to " + hit.collider.gameObject.transform.position);
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject.tag.Equals("Pushable"))
                {
                    clickedObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0));
                    Debug.Log("Push Successful!");
                }

            }
        }
    }
}
