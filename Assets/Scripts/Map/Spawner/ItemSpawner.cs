using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : Spawner
{
    Item painkiller;

    void Start()
    {

        painkiller = (Item)Resources.Load("Item/Painkiller");

        items = new List<Item>();

        items.Add(painkiller);
    }
}
