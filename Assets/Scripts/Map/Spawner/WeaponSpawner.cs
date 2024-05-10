using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : Spawner
{
    Item Axe;
    Item BaseballBat;
    Item ButcherKnife;
    Item Crowbar;
    Item Hammer;
    Item HeavyWrench;
    Item Machete;
    Item Shovel;
    Item TacticalKnife;

    void Start()
    {

        Axe = (Item)Resources.Load("Item/Axe");
        BaseballBat = (Item)Resources.Load("Item/BaseballBat");
        ButcherKnife = (Item)Resources.Load("Item/Butcher Knife");
        Crowbar = (Item)Resources.Load("Item/Crowbar");
        Hammer = (Item)Resources.Load("Item/Hammer");
        HeavyWrench = (Item)Resources.Load("Item/HeavyWrench");
        Machete = (Item)Resources.Load("Item/Machete");
        Shovel = (Item)Resources.Load("Item/Shovel");
        TacticalKnife = (Item)Resources.Load("Item/TacticalKnife");

        items = new List<Item>();

        items.Add(Axe);
        items.Add(BaseballBat);
        items.Add(ButcherKnife);
        items.Add(Crowbar);
        items.Add(Hammer);
        items.Add(HeavyWrench);
        items.Add(Machete);
        items.Add(Shovel);
        items.Add(TacticalKnife);
    }
}
