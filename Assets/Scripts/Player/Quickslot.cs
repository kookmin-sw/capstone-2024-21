using System.Collections.Generic;
using UnityEngine;

public enum Slot
{
    WeaponSlot,
    ItemSlot1,
    ItemSlot2,
    ItemSlot3,
    ItemSlot4,
    BatterySlot,
    Max
}

public class Quickslot : MonoBehaviour
{
    public SlotData[] slots = new SlotData[(int)Slot.Max];

    [SerializeField] private int maxitem = 4;

    public delegate void OnQuickslotChangedEvent(SlotData[] slots);
    public event OnQuickslotChangedEvent OnQuickslotChanged;

    /// <summary>
    /// 아이템 습득 시 실행할 함수
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(ItemData item)
    {
        if (item == null) return;

        SlotData slot = new SlotData();
        if (item.ItemType <= 10) //weapon
        {
            slot.item = item;
            slots[(int)Slot.WeaponSlot] = slot;
        }
        else //item
        {
            int idx = CanAdd();
            if (idx != -1) //빈 슬롯이 있으면
            {
                slot.item = item;
                slots[idx] = slot;
            }
            else return;
        }
        OnQuickslotChanged?.Invoke(slots);
    }

    // 잔여 퀵슬롯 판단
    private int CanAdd()
    {
        for(int i = 1; i < 5; ++i) if (slots[i] == null) return i;
        return -1;
    }

    /// <summary>
    /// 아이템 버리거나 사망 시 실행될 함수
    /// </summary>
    /// <param name="idx"></param>
    public void RemoveItem(int idx)
    {
        slots[idx] = null;
        OnQuickslotChanged?.Invoke(slots);
    }

    /// <summary>
    /// DroppableUI OnSwap구독 함수
    /// </summary>
    /// <param name="idx1"></param>
    /// <param name="idx2"></param>
    public void SwapItem(int idx1, int idx2)
    {
        ( slots[idx1], slots[idx2] ) = ( slots[idx2], slots[idx1] );
        OnQuickslotChanged?.Invoke(slots);
    }
}
