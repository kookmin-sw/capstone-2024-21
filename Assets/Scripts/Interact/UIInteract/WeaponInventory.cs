using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class WeaponInventory : MonoBehaviour
{

    [SerializeField] private GameObject batterySlot;
    public WeaponSlot weaponSlot;
    public bool isWeaponAdded;
    [SerializeField] private CraftGaugeController craftGauge;
    

    [SerializeField] private bool isCrafted;

    void Awake()
    { 
        isCrafted = false;
        weaponSlot.item = null;
        isWeaponAdded = false;
    }

    private void Update()
    {
        if (weaponSlot.item != null && batterySlot.GetComponentInChildren<Slot>().item != null)
        {
            if(batterySlot.GetComponentInChildren<Slot>().item.ItemType == 11)
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
        else
        {
            craftGauge.SetGaugeZero();
        }
    }
    public void AddItem(Item _item)
    {
        weaponSlot.item = _item;
        isWeaponAdded = true;
    }
}
