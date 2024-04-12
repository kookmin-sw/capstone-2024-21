using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class WeaponInventory : MonoBehaviour
{

    [SerializeField] private WeaponSlot weaponSlot;
    [SerializeField] private Slot batterySlot;
    [SerializeField] private Slider craftGauge;
    [SerializeField] private GameObject connected;
    [SerializeField] private GameObject unconnected;
    [SerializeField] private bool isCrafted = false;

    void Awake()
    {
        weaponSlot.item = null;
    }

    private void Update()
    {
        if(weaponSlot != null && batterySlot.item.ItemType == 11)
        {
            if(isCrafted == false)
            {
                craftGauge.value += 1 * Time.deltaTime;
                unconnected.SetActive(false);
                connected.SetActive(true);
            }
            if(craftGauge.value == 100)
            {
                isCrafted = true;
                craftGauge.value = 0;
            }

        }
        else
        {
            craftGauge.value = 0;
        }
    }
    public void AddItem(Item _item)
    {
        weaponSlot.item = _item;
    }
}
