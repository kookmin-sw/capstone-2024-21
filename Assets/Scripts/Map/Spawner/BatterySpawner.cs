using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySpawner : Spawner
{
    Item battery = (Item)Resources.Load("Assets/Resources/Item/battery.asset");
    // Start is called before the first frame update
    void Start()
    {
        items.Add(battery);
    }
}
