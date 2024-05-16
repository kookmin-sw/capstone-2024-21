using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KillManager : MonoBehaviour
{
    public string playerId;
    public int killCount { get; set; } = 0;


    private PhotonView pv;
    private UIManager uiManager;

    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    private void Start()
    {
        playerId = GameManager.Instance.UserId;
        Rename();
    }

    [PunRPC]
    public void RpcRename()
    {
        this.name = pv.Owner.NickName;
    }

    public void Rename()
    {
        pv.RPC("RpcRename", RpcTarget.All);
    }

    [PunRPC]
    public void RpcAddKillCount()
    {
        if(pv.IsMine)
        {
            killCount += 1;
            uiManager.killCount = killCount;
            Debug.Log("Kill Count: " + killCount);
        }
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