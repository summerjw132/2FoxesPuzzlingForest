using System;
using System.Collections;
using System.Collections.Generic;
///using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class IsoCharacterMovement : MonoBehaviour
{

    [SerializeField]
    private float movementUnitSize = .1f;
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private bool moveByMouseInsteadOfKeyboard = true;

    //Move by Mouse-click 
    private Vector3 moveToPosition;
    private float moveSpeed = 5f;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 selectedObjectPosition;
    private Vector3 newPosition;
    private bool isMoving = false;
    

    //Move by Key Input
    private Vector3 moveDirection;
    private Rigidbody rigidbody;
    

    private void Awake()
    {
    }
    private void Update()
    {
        //MoveOnInput();
        if (moveByMouseInsteadOfKeyboard)
        {

            MoveToTileOnMouseClick();

            if (true)
            {

                Vector3 positionX = new Vector3(selectedObjectPosition.x, 0, this.transform.position.z);
                this.transform.position = Vector3.MoveTowards(this.transform.position, positionX, moveSpeed * Time.deltaTime);

                Vector3 positionZ = new Vector3(this.transform.position.x, 0, selectedObjectPosition.z);
                this.transform.position = Vector3.MoveTowards(this.transform.position, positionZ, moveSpeed * Time.deltaTime);

                //while(this.transform.position.z != selectedObjectPosition.z)
                //{
                //    Vector3 positionZ = new Vector3(0, 0, selectedObjectPosition.z);
                //    this.transform.position = Vector3.MoveTowards(this.transform.position, positionZ, moveSpeed * Time.deltaTime);
                //}
                isMoving = false;
            }
        }
        else
        {
            MoveOnInput();
        }
    }

    private void MoveToTileOnMouseClick()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                Debug.Log("Clicked from " + this.gameObject.transform.position + " to " + hit.collider.gameObject.transform.position);
                selectedObjectPosition = hit.collider.gameObject.transform.position;
                //newPosition = new Vector3(selectedObjectPosition.x, 0, selectedObjectPosition.z);
                isMoving = true;
                
            }
        }
    }

    private void MoveOnInput()
    {
        float xMovement = Input.GetAxis("Horizontal");
        float yMovement = Input.GetAxis("Vertical");

        if(xMovement == 0 && yMovement == 0)
        {
            Debug.Log("Standing Still.");
            transform.Translate(Vector3.zero);

        }
        else
        {
            if(xMovement > 0)
            {
                Debug.Log("Moving Forward Horizontal.");
                transform.Translate(Vector3.left * movementUnitSize);
                //this.GetComponent<Rigidbody>().AddForce(Vector3.right * movementUnitSize * Time.deltaTime);
                
            }
            if(yMovement > 0)
            {
                Debug.Log("Moving Forward Vertical");
                transform.Translate(Vector3.back * movementUnitSize);

                //this.GetComponent<Rigidbody>().AddForce(Vector3.forward * movementUnitSize * Time.deltaTime);
            }
            if (xMovement < 0)
            {
                Debug.Log("Moving Backward Horizontal.");
               transform.Translate(Vector3.right * movementUnitSize);
                //this.GetComponent<Rigidbody>().AddForce(Vector3.left * movementUnitSize * Time.deltaTime);
            }
            if (yMovement < 0)
            {
                Debug.Log("Moving Backward Vertical");
                transform.Translate(Vector3.forward * movementUnitSize);
                //this.GetComponent<Rigidbody>().AddForce(Vector3.back * movementUnitSize * Time.deltaTime);
            }
        } 
    }
}
