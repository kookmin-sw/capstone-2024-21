using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class WeaponSpawner : Spawner
{
    ItemData Axe;
    ItemData BaseballBat;
    ItemData ButcherKnife;
    ItemData Crowbar;
    ItemData Hammer;
    ItemData HeavyWrench;
    ItemData Machete;
    ItemData Shovel;
    ItemData TacticalKnife;

    //우선 아이템 스포너랑 무기 스포너 합쳐보려 함 
    ItemData painkiller;

    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();

        //무기 
        Axe = (ItemData)Resources.Load("Item/Axe");
        BaseballBat = (ItemData)Resources.Load("Item/BaseballBat");
        ButcherKnife = (ItemData)Resources.Load("Item/Butcher Knife");
        Crowbar = (ItemData)Resources.Load("Item/Crowbar");
        Hammer = (ItemData)Resources.Load("Item/Hammer");
        HeavyWrench = (ItemData)Resources.Load("Item/HeavyWrench");
        Machete = (ItemData)Resources.Load("Item/Machete");
        Shovel = (ItemData)Resources.Load("Item/Shovel");
        TacticalKnife = (ItemData)Resources.Load("Item/TacticalKnife");

        //아이템 
        painkiller = (ItemData)Resources.Load("Item/Painkiller");

        //리스트 객체 할당 
        items = new List<ItemData>();

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
