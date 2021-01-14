using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float TurnSpeed = .5f;
    public GameObject CameraPivot;
    public float Zoom;
    public Camera cam;
    public float zoomSpeed = 20f;
    public float Max = 60, Min = 40;
    public float ZoomLerp = 10;
    // Start is called before the first frame update
    void Start()
    {
        Zoom = cam.fieldOfView;
      
    }

    // Update is called once per frame
    void Update()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");
        Zoom -= scrollData * zoomSpeed;
        Zoom = Mathf.Clamp(Zoom, Min, Max);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Zoom, Time.deltaTime * ZoomLerp);
        if (Input.GetKey(KeyCode.Z))
        {
            CameraPivot.transform.Rotate(0f, 1f, 0f * TurnSpeed);
        }

        if (Input.GetKey(KeyCode.X))
        {
            CameraPivot.transform.Rotate(0f, -1f, 0f * TurnSpeed);
        }

       

    }
}
