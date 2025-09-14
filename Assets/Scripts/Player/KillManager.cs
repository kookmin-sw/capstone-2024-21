using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KillManager : MonoBehaviour
{
    public int killCount { get; set; } = 0;

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
        }
    }

    public void AddKillCount()
    {
        pv.RPC("RpcAddKillCount", RpcTarget.All);
    }
}