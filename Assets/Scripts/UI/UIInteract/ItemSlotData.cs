using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotData : SlotData
{
    protected override bool CanAccept(Item curItem)
    {
        if (curItem.ItemType > 10)
        {
            return true;
        }
        return false;
    }
}
