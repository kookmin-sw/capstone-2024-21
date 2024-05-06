using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillManager : MonoBehaviour
{

    public int killCount { get; set; } = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AddKillCount()
    {
        killCount += 1;
        Debug.Log("Kill Count: " + killCount);
    }

    // Update is called once per frame
    void Update()
    {

    }
}