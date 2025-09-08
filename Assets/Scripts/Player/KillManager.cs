using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KillManager : MonoBehaviour
{
    private int _killCount = 0;
    public int killCount
    {
        get { return _killCount; }
        set { _killCount = value; }
    }

    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void RpcAddKillCount()
    {
        if(pv.IsMine)
        {
            killCount += 1;
            Debug.Log("Kill Count: " + killCount);
        }
    }

    public void AddKillCount()
    {
        pv.RPC("RpcAddKillCount", RpcTarget.All);
    }
}