using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//게임 오브젝트에서 아이템에 대한 데이터를 불러 올 수 있도록 하기위한 스크립트. 게임오브젝트 프리팹에 넣어주면됨
public class ItemData : MonoBehaviour
{
    public PhotonView pv;

    public Item itemData;

    void Start()
    {
        if (!GetComponent<PhotonView>())
        {
            pv = gameObject.AddComponent<PhotonView>();
            pv.ViewID = PhotonNetwork.AllocateViewID(0);
        }
    }

    Item getItemData()
    {
        return itemData;
    }

    [PunRPC]
    public void DestroyItem()
    {
        Destroy(gameObject);
    }

    public void DestroyItemRPC()
    {
        pv.RPC("DestroyItem", RpcTarget.AllBuffered);
    }
}
