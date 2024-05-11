using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    Transform[] points;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Go2Map();
        }
    }

    void Go2Map()
    {
        Transform[] points = GameObject.Find("WarpPointGroup").GetComponentsInChildren<Transform>();
        if (points != null)
        {
            int idx = Random.Range(1, points.Length);
            this.transform.position = points[idx].position;
        }
        else
        {
            Debug.Log("points가 왜 null임??");
        }
    }
}
