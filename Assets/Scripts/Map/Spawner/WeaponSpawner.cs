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

        Axe = (Item)Resources.Load("Assets/Resources/Item/Axe.asset");
        BaseballBat = (Item)Resources.Load("Assets/Resources/Item/BaseballBat.asset");
        ButcherKnife = (Item)Resources.Load("Assets/Resources/Item/Butcher Knife.asset");
        Crowbar = (Item)Resources.Load("Assets/Resources/Item/Crowbar.asset");
        Hammer = (Item)Resources.Load("Assets/Resources/Item/Hammer.asset");
        HeavyWrench = (Item)Resources.Load("Assets/Resources/Item/HeavyWrench.asset");
        Machete = (Item)Resources.Load("Assets/Resources/Item/Machete.asset");
        Shovel = (Item)Resources.Load("Assets/Resources/Item/Shovel.asset");
        TacticalKnife = (Item)Resources.Load("Assets/Resources/Item/TacticalKnife.asset");

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
