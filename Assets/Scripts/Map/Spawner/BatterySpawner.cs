using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BatterySpawner : Spawner
{
    [SerializeField] Item battery;

    void Start()
    {
        battery = (Item)Resources.Load("Item/Battery");
        items = new List<Item>();
        items.Add(battery);

        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);
    }
}
