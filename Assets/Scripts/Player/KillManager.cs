using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KillManager : MonoBehaviour
{
    public string playerId;
    public int killCount { get; set; } = 0;

    private PhotonView pv;

    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            playerId = GameManager.Instance.UserId;
        }
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