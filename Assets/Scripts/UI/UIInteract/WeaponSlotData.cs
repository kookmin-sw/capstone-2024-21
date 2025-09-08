using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotData : SlotData
{
    protected override bool CanAccept(Item curItem)
    {
        if(curItem.ItemType <= 10)
        {
            return true;
        }
        return false;
    }
}
