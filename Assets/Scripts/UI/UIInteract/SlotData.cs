using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotData : MonoBehaviour
{
    [SerializeField] protected Image image;
    [SerializeField] protected Item _item;

    public Item item
    {
        get { return _item; }

        set
        {
            //item set은  Inventory 스크립트에서 addItem()과 FreshSlot() 함수에서 이루어짐
            if (CanAccept(value))
            {
                _item = value;
                Refresh();
            }
        }
    }
    protected virtual bool CanAccept(Item curItem) { return true; }

    protected void Refresh()
    {
        if(_item != null)
        {
            image.sprite = item.itemImage;
            image.enabled = true;
        }
        else
        {
            image.sprite = null;
            image.enabled = false;
        }
    }
}