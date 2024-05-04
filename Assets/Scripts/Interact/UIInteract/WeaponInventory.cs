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
    public bool isCrafted;
    public Item abandonedItem;

    [SerializeField] private GameObject objWeaponSlot;
    [SerializeField] private CraftGaugeController craftGauge;
    [SerializeField] private GameObject batterySlot;
    

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
        if (weaponSlot.item != null && batterySlot.transform.childCount > 0 && weaponSlot.item.craftCompleted == false)
        {
            if(batterySlot.GetComponentInChildren<Slot>().item != null)
            {
                if (batterySlot.GetComponentInChildren<Slot>().item.ItemType == 11)
                {
                    if (craftGauge.FillBolt())
                    {
                        batterySlot.GetComponentInChildren<Slot>().item = null;
                        weaponSlot.item.craftCompleted = true;
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
    public int AddWeapon(Item _item)
    {
        if(weaponSlot.item == null)
        {
            weaponSlot.item = _item;
            isWeaponAdded = true;
            return 1;
        }
        else
        {
            return 0;
        }

    }
}
