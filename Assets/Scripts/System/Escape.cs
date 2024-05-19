using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Escape : MonoBehaviour
{
    PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pv.RPC("GameOver", RpcTarget.Others);
            EscapeMe();
        }
    }

    void EscapeMe()
    {
        GameManager.Instance.Escape();
    }

    [PunRPC]
    void RpcGameOver()
    {
        GameManager.Instance.GameOver();
    }
}
