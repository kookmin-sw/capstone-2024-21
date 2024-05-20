using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
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

    //우선 아이템 스포너랑 무기 스포너 합쳐보려 함 
    Item painkiller;

    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();

        //무기 
        Axe = (Item)Resources.Load("Item/Axe");
        BaseballBat = (Item)Resources.Load("Item/BaseballBat");
        ButcherKnife = (Item)Resources.Load("Item/Butcher Knife");
        Crowbar = (Item)Resources.Load("Item/Crowbar");
        Hammer = (Item)Resources.Load("Item/Hammer");
        HeavyWrench = (Item)Resources.Load("Item/HeavyWrench");
        Machete = (Item)Resources.Load("Item/Machete");
        Shovel = (Item)Resources.Load("Item/Shovel");
        TacticalKnife = (Item)Resources.Load("Item/TacticalKnife");

        //아이템 
        painkiller = (Item)Resources.Load("Item/Painkiller");

        //리스트 객체 할당 
        items = new List<Item>();

        //리스트에 무기 추가 
        items.Add(Axe);
        items.Add(BaseballBat);
        items.Add(ButcherKnife);
        items.Add(Crowbar);
        items.Add(Hammer);
        items.Add(HeavyWrench);
        items.Add(Machete);
        items.Add(Shovel);
        items.Add(TacticalKnife);

        //리스트에 아이템 추가
        items.Add(painkiller);
    }
}
