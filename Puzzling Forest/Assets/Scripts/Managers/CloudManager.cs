using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] CloudsModel; // Const
    [SerializeField]
    [Range(0.1f, 10f)]
    [Tooltip("Time between each cloud to spawn")]
    float frequency;
    [SerializeField]
    [Range(0.1f, 10f)]
    [Tooltip("Overall Speed of all clouds")]
    float OverallSpeed = 1f;
    [SerializeField]
    [Range(0.1f, 5f)]
    [Tooltip("Overall scale of all clouds")]
    float OverallScale = 1f;
    [SerializeField]
    [Range(0, 30)]
    [Tooltip("How many clouds can appear at the same time")]
    uint MaxCloudOnScreen = 3;
    [SerializeField]
    [Range(0, 3000)]
    [Tooltip("How close they are apart from each other. Lower number closer distance. Dont change during game")]
    int cloudDistance = 3;

    //[SerializeField]
    //[Range(3, 30)]
    float padding = 4f;
    List<Mesh> meshList = new List<Mesh>();

    //Obj pool
    Queue<Cloud> cloudsScript = new Queue<Cloud>();
    List<int> cloudPos = new List<int>();

    Camera cam;
    int camWidth, camHeight;
    float cloudHeight;
    float dis;

    float timer;
    int liveCloudCount;
    void Awake()
    {
        foreach (var cloud in CloudsModel)
        {
            meshList.Add(cloud.GetComponent<MeshFilter>().sharedMesh);
        }
        cam = transform.parent.GetComponentInChildren<Camera>();
        camWidth = (int)(cam.pixelWidth * 0.9f);
        camHeight = (int)(cam.pixelHeight * 0.9f);

    }
    private void Start()
    {   
        Vector3 pointA = new Vector3();
        Vector3 pointB = new Vector3();
        pointA = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane + 3f));
        pointB = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane + 3f));
        dis = Vector3.Distance(pointA, pointB);
        padding *= OverallScale;
        dis += padding * 2;

        cloudHeight = pointA.y;

        for (int i = -camHeight; i < camWidth; i++)
        {
            cloudPos.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <0 && liveCloudCount< MaxCloudOnScreen)
        {
            SpawnNewCloud();
        }
        timer -= Time.deltaTime;
    }

    Cloud spawnNewObjToPool()
    {
        GameObject temp = Instantiate(CloudsModel[0], transform);
        Cloud cloud = temp.AddComponent<Cloud>();
        cloud.SetCloudManager(this);
        return cloud;
    }

    public void GetBackToPool(Cloud cloud)
    {
        cloudsScript.Enqueue(cloud);
        liveCloudCount--;
        int val = cloud.pos;
        for (int i = val - cloudDistance; i <= val + cloudDistance; i++)
        {
            cloudPos.Add(i);
        }
    }

    void SpawnNewCloud()
    {
        int rotate = Random.Range(0, 8);
        float size = Random.Range(-0.75f, 1f);
        int model = Random.Range(0, meshList.Count);

        #region pos
        Vector3 point = new Vector3();
        int rand = RandomPos();
        if (rand != int.MinValue)
        {
            if (rand > 0)
            {
                point = cam.ScreenToWorldPoint(new Vector3(rand, 0, 1.0f));
            }
            else
            {
                rand *= -1;
                point = cam.ScreenToWorldPoint(new Vector3(0, rand, 1.0f));
            }
            point.y = cloudHeight;
            point.z -= padding;
            #endregion

            Cloud.CloudInfo info = new Cloud.CloudInfo();
            info.start = point;
            info.rotate = rotate;
            info.speed = OverallSpeed;
            info.size = size;
            info.scale = OverallScale;
            info.mesh = meshList[model];
            info.distance = dis;
            info.edgePos = rand;

            liveCloudCount++;
            if (cloudsScript.Count > 0)
            {
                Cloud cloud = cloudsScript.Dequeue();
                cloud.SpawnNewCloud(info);
            }
            else
            {
                spawnNewObjToPool().SpawnNewCloud(info);
            }
        }
        else
            Debug.Log("Too packed to spawn new cloud");
    }

    int CheckCloudDistance(int check)
    { 
    //    if (cloudPos.Count == 0) return check;

    //    //

    //    for (int i = 0; i < cloudPos.Count; i++)
    //    {
    //        if (check < cloudPos[i] + (int)cloudDistance)
    //        {
    //            return check;
    //        }
    //        else
    //        {
    //            check = cloudPos[i] + (int)cloudDistance;
    //        }
    //    }
    //    if (check < camWidth)
            return check;
        //else return int.MinValue;
    }

    int RandomPos()
    {
        if (cloudPos.Count == 0)
            return int.MinValue;
        int val = Random.Range(0, cloudPos.Count);

        val = cloudPos[val];
        cloudPos.RemoveAll(x => x <= val + cloudDistance && x >= val - cloudDistance);
        return val;
    }
}
