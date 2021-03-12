using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This script finds the water blocks in the level and sets up the shader to display across them.
/// </summary>
public class WaterManager : MonoBehaviour
{
    private GameObject[] waterBlocks;
    private Dictionary<float, WaterBody> bodies = new Dictionary<float, WaterBody>();

    void Awake()
    {
        waterBlocks = GameObject.FindGameObjectsWithTag("Water");
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < waterBlocks.Length; i++)
        {
            Vector3 curPos = waterBlocks[i].transform.position;
            if (!bodies.ContainsKey((float) Math.Round(curPos.y, 3)))
            {
                bodies.Add((float) Math.Round(curPos.y, 3), new WaterBody(curPos, waterBlocks[i].transform.localScale));

                waterBlocks[i].GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                bodies[(float)Math.Round(curPos.y, 3)].AddBlock(waterBlocks[i]);
            }
        }

        PrintDict();

        foreach (KeyValuePair<float, WaterBody> kvp in bodies)
        {
            bodies[kvp.Key].ShowPoints();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    #region helpers
    private void PrintDict()
    {
        foreach (KeyValuePair<float, WaterBody> kvp in bodies)
        {
            Debug.LogFormat("key: {0}\nval: {1}", kvp.Key, kvp.Value);
        }
    }

    static float North(GameObject go)
    {
        return go.transform.position.z + (go.transform.localScale.z / 2.0f);
    }
    static float East(GameObject go)
    {
        return go.transform.position.x + (go.transform.localScale.x / 2.0f);
    }
    static float South(GameObject go)
    {
        return go.transform.position.z - (go.transform.localScale.z / 2.0f);
    }
    static float West(GameObject go)
    {
        return go.transform.position.x - (go.transform.localScale.x / 2.0f);
    }

    #endregion

    public class WaterBody
    {
        private List<GameObject> myBlocks = new List<GameObject>();
        private Dictionary<string, float> edges = new Dictionary<string, float>();
        private Vector3 center = new Vector3();
        private float elevation;
        private float topHeight;

        public WaterBody(Vector3 pos, Vector3 scale)
        {
            edges.Add("north", pos.z + (scale.z / 2.0f));
            edges.Add("east", pos.x + (scale.x / 2.0f));
            edges.Add("south", pos.z - (scale.z / 2.0f));
            edges.Add("west", pos.x - (scale.x / 2.0f));

            elevation = pos.y;
            topHeight = pos.y + (scale.y / 2.0f);
        }

        public void UpdateEdge(string key, float val)
        {
            edges[key] = val;
        }

        public void AddBlock(GameObject curBlock)
        {
            myBlocks.Add(curBlock);

            bool changed = false;
            if (North(curBlock) > edges["north"])
            {
                changed = true;
                edges["north"] = North(curBlock);
            }
            if (East(curBlock) > edges["east"])
            {
                changed = true;
                edges["east"] = East(curBlock);
            }
            if (South(curBlock) < edges["south"])
            {
                changed = true;
                edges["south"] = South(curBlock);
            }
            if (West(curBlock) < edges["west"])
            {
                changed = true;
                edges["west"] = West(curBlock);
            }
            if (changed)
                FindCenter();
        }

        public void FindCenter()
        {
            center.z = (edges["north"] + edges["south"]) / 2.0f;
            center.x = (edges["east"] + edges["west"]) / 2.0f;
            center.y = topHeight;
        }

        public void ShowPoints()
        {
            GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject sphere4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            Vector3 curPos = new Vector3(edges["west"], topHeight, edges["north"]);
            sphere1.transform.position = curPos;

            curPos.x = edges["east"];
            sphere2.transform.position = curPos;

            curPos.z = edges["south"];
            sphere3.transform.position = curPos;

            curPos.x = edges["west"];
            sphere4.transform.position = curPos;

            Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
            sphere1.transform.localScale = scale;
            sphere2.transform.localScale = scale;
            sphere3.transform.localScale = scale;
            sphere4.transform.localScale = scale;

            sphere1.GetComponent<Renderer>().material.color = Color.red;
            sphere2.GetComponent<Renderer>().material.color = Color.red;
            sphere3.GetComponent<Renderer>().material.color = Color.red;
            sphere4.GetComponent<Renderer>().material.color = Color.red;

            GameObject sphereCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            sphereCenter.transform.position = center;
            sphereCenter.transform.localScale = scale;
            sphereCenter.GetComponent<Renderer>().material.color = Color.blue;
        }

        public override string ToString()
        {
            string retString;
            retString = "TopHeight: " + topHeight;
            retString += " | N:" + edges["north"];
            retString += " | E:" + edges["east"];
            retString += " | S:" + edges["south"];
            retString += " | W:" + edges["west"];

            return retString;
        }
    }
}
