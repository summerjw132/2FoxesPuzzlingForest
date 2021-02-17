using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public float Turning = 70;
    public GameObject CameraUI;
    public float Speed = 10f;
    public float TurnSpeed = .5f;
    public GameObject CameraPivot;
    public float Zoom;
    public Camera cam;
    public float zoomSpeed = 20f;
    public float Max = 70, Min = 40;
    public float ZoomLerp = 10;
    static public bool CamOn = false;
    public bool ImThere = false;
    float Step;

    // Camera Walking Varaiables
    public float Forwards = 0;
    public float Backwards = 4;
    
    Quaternion StartRotation;
    Vector3 StartPosition;
    // Start is called before the first frame update
    void Start()
    {
        CameraUI = GameObject.Find("CameraActive");
        CameraUI.SetActive(false);
        Zoom = cam.fieldOfView;
        StartRotation = CameraPivot.transform.rotation;
        StartPosition = cam.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        Step = Speed * Time.deltaTime;
        if (CamOn == false)
        {
            CameraReset();
           
            CameraUI.SetActive(false);

        }
        if (Input.GetKeyDown(KeyCode.C))
        {

            
            if(CamOn == true)
            {
                cam.fieldOfView = 60;
                CamOn = false;
                


            }
            else
            {

                CamOn = true;

            }
            if (Vector3.Distance(transform.position, StartPosition) < 0.001f)
            {
                // Swap the position of the cylinder.
                StartPosition *= -1.0f;
            }

        }
        if (CamOn == true)
        {
            CameraUI.SetActive(true);
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
            
            float xAxisValue = Input.GetAxis("Horizontal");
            Debug.Log(xAxisValue);
            float yAxisValue = Input.GetAxisRaw("Vertical");
            

            if (CameraPivot != null)
            {
                 cam.transform.Translate(new Vector3(xAxisValue, yAxisValue, 0.0f) *Speed * Time.deltaTime);
                Mathf.Clamp(xAxisValue,Forwards, Backwards);
                Mathf.Clamp(yAxisValue, Forwards, Backwards);
            }
            
        }

        
    }
    void CameraReset()
    {
        if (cam.transform.position != StartPosition)
        {
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, StartPosition, Step);
        }
        CameraPivot.transform.rotation = Quaternion.RotateTowards(transform.rotation, StartRotation, Turning * Time.deltaTime);

    }

}

