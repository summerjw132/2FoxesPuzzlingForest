using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    CloudManager CloudManager;
    public int pos;

    MeshFilter meshFilter;
    bool isPlaying;
    float speed;
    Vector3 dir;
    float distance;

    float defaultSize;

    public struct CloudInfo
    {
        public Vector3 start;
        public int rotate;
        public float speed;
        public float distance;
        public float size;
        public float scale;
        public Mesh mesh;
        public int edgePos;
    }


    public void SetCloudManager(CloudManager _cm) => CloudManager = _cm;
    // Start is called before the first frame update
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        gameObject.SetActive(false);
        defaultSize = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlaying)
        {
            dir = Vector3.forward;
            Vector3 vec = dir * speed * Time.deltaTime;
            distance -= speed * Time.deltaTime;
            transform.Translate(vec, Space.World);
            if(distance < 0)
            {
                isPlaying = false;
                gameObject.SetActive(false);
                CloudManager.GetBackToPool(this);
            }
        }
    }

    public void SpawnNewCloud(CloudInfo info)
    {
        gameObject.SetActive(true);
        isPlaying = true;
        transform.position = info.start;

        /// rotate
        /// 0: y 0  , z 0, 2: y 0  , z 90, 4: y 0  , z 180, 6: y 0  , z 270
        /// 1: y 180, z 0, 3: y 180, z 90, 5: y 180, z 180, 7: y 180, z 270
        /// 1 - y 180, z 0
        float y = info.rotate % 2 == 0 ? 0 : 180f;
        y += 10f;
        float z = (info.rotate / 2) * 90f;
        transform.rotation = Quaternion.Euler(0, y, z);

        float newSize = defaultSize * (info.size + 2f) / 2f;
        newSize *= info.scale;
        transform.localScale = new Vector3(newSize, newSize, newSize);

        this.speed = info.speed * (-info.size + 2f) / 2f;

        this.distance = info.distance;

        meshFilter.mesh = info.mesh;

        this.pos = info.edgePos;
    }
}
