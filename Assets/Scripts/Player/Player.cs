using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Player : MonoBehaviour
{
    [SerializeField]
    Transform[] points;
    private PhotonView pv;

    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        Rename();
    }

    [PunRPC]
    public void RpcGo2Map(Vector3 pos)
    {
        if (pv.IsMine)
        {
            this.transform.position = pos;
        }
    }

    public void Go2Map(Vector3 pos)
    {
        pv.RPC("RpcGo2Map", RpcTarget.All, pos);
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
}
