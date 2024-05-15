using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ItemSpawner : Spawner
{
    Item painkiller;

    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();

        painkiller = (Item)Resources.Load("Item/Painkiller");

        items = new List<Item>();

        items.Add(painkiller);
    }
}
