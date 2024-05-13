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
        playerId = GameManager.Instance.UserId;
    }

    [PunRPC]
    public void RpcRename(string playerId)
    {
        this.name = playerId;
    }

    public void Rename(string playerId)
    {
        pv.RPC("RpcRename", RpcTarget.All, playerId);
    }

    [PunRPC]
    public void RpcAddKillCount()
    {
        killCount += 1;
        Debug.Log("Kill Count: " + killCount);
    }

    public void AddKillCount()
    {
        pv.RPC("RpcAddKillCount", RpcTarget.All);
    }

    // Update is called once per frame
    void Update()
    {

    }
}