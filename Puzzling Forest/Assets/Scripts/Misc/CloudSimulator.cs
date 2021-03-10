using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSimulator : MonoBehaviour
{
    public GameObject[] Clouds;
    public Transform[] SpawnSpots;
    public int randomClouds;
    public int randomSpawn;
    public GameObject[] SpawnedClouds;
    private Vector3 StartSpot;
    private GameObject Clone;
    public Vector3 Spawn, Spawn1, Spawn2, Spawn3, Spawn4;
    public float speed = 1;
    public Vector3 Size;
    public float IncreaseCloudSize;
    public float DecreaseCloudSize;
    private float randRotation;
    public float CloudTimer = 7f;
    // Start is called before the first frame update
    void Start()
    {
       
        speed = speed * Time.deltaTime;
        Spawn = SpawnSpots[0].position;
        Spawn1 = SpawnSpots[1].position;
        Spawn2 = SpawnSpots[2].position;
        Spawn3 = SpawnSpots[3].position;
        Spawn4 = SpawnSpots[4].position;
    }

    // Update is called once per frame
    void Update()
    {
     
        SpawnedClouds = GameObject.FindGameObjectsWithTag("Cloud");
        if (Input.GetKeyDown(KeyCode.Alpha9) || SpawnedClouds.Length == 0)
        {
           randomClouds = Random.Range(0, 4);
           randomSpawn = Random.Range(0, 5);
            randRotation = Random.Range(0,360);
           Instantiate(Clouds[randomClouds], SpawnSpots[randomSpawn].position, SpawnSpots[randomSpawn].rotation);
          
            Clone = GameObject.FindGameObjectWithTag("Cloud");
            Clone.transform.Rotate(0, randRotation, 0);
            StartSpot = Clone.transform.position;
            Clone.transform.localScale += new Vector3(IncreaseCloudSize, IncreaseCloudSize, IncreaseCloudSize);
            Clone.transform.localScale -= new Vector3(DecreaseCloudSize, DecreaseCloudSize, DecreaseCloudSize);
            Destroy(Clone, CloudTimer);

        }
        if(StartSpot == Spawn || StartSpot == Spawn2 || StartSpot == Spawn4)
        {

            Clone.transform.Translate(1*speed,0,1*speed,Space.World);

        }

        if (StartSpot == Spawn1 || StartSpot == Spawn3)
        {

            Clone.transform.Translate(-1 * speed, 0, -1 * speed,Space.World);
          
        }
       
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {

            Clone.transform.localScale -= new Vector3(-DecreaseCloudSize, -DecreaseCloudSize, -DecreaseCloudSize);

        }
        
    }
}
