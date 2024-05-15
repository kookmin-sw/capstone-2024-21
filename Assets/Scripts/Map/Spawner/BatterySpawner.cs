using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BatterySpawner : Spawner
{
    //네트워크
    //public PhotonView pv;

    [SerializeField] Item battery;

    void Start()
    {
        //pv = gameObject.AddComponent<PhotonView>();
        //pv.ViewID = PhotonNetwork.AllocateViewID(0);
        //Debug.Log(transform.gameObject.name + " 의 ViewID : ")

        pv = gameObject.GetComponent<PhotonView>();

        battery = (Item)Resources.Load("Item/Battery"); 
        items = new List<Item>();
        items.Add(battery);


    }
}
