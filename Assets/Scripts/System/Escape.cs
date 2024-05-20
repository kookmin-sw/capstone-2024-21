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
        Debug.Log("충돌은 일어남!");
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Player")
        {
            EscapeMe();
        }
    }

    void EscapeMe()
    {
        GameManager.Instance.Escape();
    }
}
