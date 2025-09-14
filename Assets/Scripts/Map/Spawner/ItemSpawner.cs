using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ItemSpawner : Spawner
{
    ItemData painkiller;

    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();

        painkiller = (ItemData)Resources.Load("Item/Painkiller");

        items = new List<ItemData>();

        items.Add(painkiller);
    }
}
