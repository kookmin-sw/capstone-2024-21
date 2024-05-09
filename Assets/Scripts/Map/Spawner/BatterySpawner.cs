using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySpawner : Spawner
{
    [SerializeField] Item battery;

    //List<Item> BatterySpawner.items = Spawner.items;

    // Start is called before the first frame update
    void Start()
    {
        battery = (Item)Resources.Load("Item/Battery");

        items.Add(battery);
        Debug.Log("items.Count" + items.Count);
    }
}
