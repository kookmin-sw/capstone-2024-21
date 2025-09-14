using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BatterySpawner : Spawner
{

    [SerializeField] ItemData battery;

    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();

        battery = (ItemData)Resources.Load("Item/Battery"); 
        items = new List<ItemData>();
        items.Add(battery);


    }
}
