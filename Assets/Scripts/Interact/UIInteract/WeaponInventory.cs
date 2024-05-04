using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class WeaponInventory : MonoBehaviour
{

    public WeaponSlot weaponSlot;
    public bool isWeaponAdded;
    public Item abandonedItem;

    [SerializeField] private CraftGaugeController craftGauge;
    [SerializeField] private GameObject batterySlot;
    [SerializeField] private bool isCrafted;

    void Awake()
    { 
        isCrafted = false;
        batterySlot.GetComponentInChildren<Slot>().item = null;
        weaponSlot.item = null;
        isWeaponAdded = false;
        abandonedItem = null;
    }

    private void Update()
    {
        if (weaponSlot.item != null && batterySlot.transform.childCount > 0)
        {
            if(batterySlot.GetComponentInChildren<Slot>().item != null)
            {
                if (batterySlot.GetComponentInChildren<Slot>().item.ItemType == 11)
                {
                    if (craftGauge.FillBolt())
                    {
                        batterySlot.GetComponentInChildren<Slot>().item = null;
                        craftGauge.SetGaugeZero();
                    }
                }
                else
                {
                    craftGauge.SetGaugeZero();
                }
            }
        }
        else
        {
            craftGauge.SetGaugeZero();
        }
    }
    public void AddItem(Item _item)
    {
        abandonedItem = weaponSlot.item;
        weaponSlot.item = _item;
        isWeaponAdded = true;
    }
}
