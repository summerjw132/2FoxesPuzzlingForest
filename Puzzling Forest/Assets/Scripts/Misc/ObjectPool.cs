using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool SharedInstances;
    private List<GameObject> pooledObjects;
    public GameObject[] objectToPool;
    public int amountToPool;
    private Vector3 Spawn, Spawn1, Spawn2, Spawn3, Spawn4;
    public float speed = 1;
    private float randRotation;
    private Vector3 StartSpot;
    public GameObject[] Clouds;
    public Transform[] SpawnSpots;
    private int randomClouds;
    private int randomSpawn;
    public GameObject SpawnedClouds;

    private GameObject Clone;
    private Vector3 SavedSpot;
    public Vector3 Size;
    public float IncreaseCloudSize;
    public float DecreaseCloudSize;
    public float CloudTimer = 7f;
    private void Awake()
    {
        SharedInstances = this;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        randRotation = Random.Range(0, 360);
        randomClouds = Random.Range(0, 4);
        StartSpot = SavedSpot;
       speed = speed * Time.deltaTime;
        Spawn = SpawnSpots[0].position;
        Spawn1 = SpawnSpots[1].position;
        Spawn2 = SpawnSpots[2].position;
        Spawn3 = SpawnSpots[3].position;
        Spawn4 = SpawnSpots[4].position;
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            randRotation = Random.Range(0, 360);
            randomClouds = Random.Range(0, 4);
            tmp = Instantiate(objectToPool[randomClouds], SpawnSpots[randomSpawn].position, SpawnSpots[randomSpawn].rotation);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
            

        }

    }

    
    
   
    void Update()
    {
        randomClouds = Random.Range(0, 4);
        randomSpawn = Random.Range(0, 5);
        randRotation = Random.Range(0, 360);
       
       
       


        if (StartSpot == Spawn || StartSpot == Spawn2 || StartSpot == Spawn4)
        {

            SpawnedClouds.transform.Translate(1 * speed, 0, 1 * speed, Space.World);

        }

        if (StartSpot == Spawn1 || StartSpot == Spawn3)
        {

            SpawnedClouds.transform.Translate(-1 * speed, 0, -1 * speed, Space.World);
          
        }
       
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            
            SpawnedClouds.SetActive(false);
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {

           
            GameObject Cloud = ObjectPool.SharedInstances.GetPooledObject();
            if (Cloud != null)
            {
               

                Cloud.SetActive(true);
                SpawnedClouds = GameObject.FindGameObjectWithTag("Cloud");
                Cloud.transform.position = SpawnSpots[randomSpawn].position;
                Cloud.transform.rotation = SpawnSpots[randomSpawn].rotation;
                StartSpot = SpawnedClouds.transform.position;

            }
        }



        /*
        spawnedclouds = gameobject.findgameobjectswithtag("cloud");
        if (input.getkeydown(keycode.alpha9) || spawnedclouds.length == 0)
        {
            randomclouds = random.range(0, 4);

            instantiate(clouds[randomclouds], spawnspots[randomspawn].position, spawnspots[randomspawn].rotation);

            clone = gameobject.findgameobjectwithtag("cloud");
            clone.transform.rotate(0, randrotation, 0);
            startspot = clone.transform.position;
            clone.transform.localscale += new vector3(increasecloudsize, increasecloudsize, increasecloudsize);
            clone.transform.localscale -= new vector3(decreasecloudsize, decreasecloudsize, decreasecloudsize);
            destroy(clone, cloudtimer);

        }

        if (startspot == spawn || startspot == spawn2 || startspot == spawn4)
        {

            clone.transform.translate(1 * speed, 0, 1 * speed, space.world);

        }

        if (startspot == spawn1 || startspot == spawn3)
        {

            clone.transform.translate(-1 * speed, 0, -1 * speed, space.world);

        }

        if (input.getkeydown(keycode.alpha0))
        {

            clone.transform.localscale -= new vector3(-decreasecloudsize, -decreasecloudsize, -decreasecloudsize);

        }

        */

       

    }
    public GameObject GetPooledObject()
    {

        for (int i = 0; i < amountToPool; i++)
        {

            if (!pooledObjects[i].activeInHierarchy)
            {

                return pooledObjects[i];

            }

        }
        return null;
    }

}
