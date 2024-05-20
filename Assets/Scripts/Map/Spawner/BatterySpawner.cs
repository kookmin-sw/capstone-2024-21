using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BatterySpawner : Spawner
{

    [SerializeField] Item battery;

    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();

        battery = (Item)Resources.Load("Item/Battery"); 
        items = new List<Item>();
        items.Add(battery);


    }
}
