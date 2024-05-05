using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

//ItemQuickSlots에 들어가 있음
public class Inventory : MonoBehaviour
{
    public List<Item> items ; //아이템을 담을 리스트

    //[SerializeField] private Transform slotParent; // Slot의 부모를 담을 곳
    public Slot[] slots;  //Iten Quick Slot의 하위에 있는 Slot을 담을 곳
    public bool isItemAdded;
    public bool isSlotChanged;

    public void FreshSlot()
    {
        slots = GetComponentsInChildren<Slot>();
        int i = 0; //두 개의 For 문에 같은 i의 값을 사용하기 위해서 외부에 선언

        for (; i < items.Count && i < slots.Length; i++)
        {
            items[i] = slots[i].item;
        }
    }


    //게임이 시작되면 items에 들어 있는 아이템을 인벤토리에 넣어줌 
    void Awake()
    {
        items = new List<Item> { null, null, null, null };
        isItemAdded = false;
        isSlotChanged = false;
        FreshSlot();
    }

    //아이템을 획득할 경우 AddItem을 불러와 넣어 주면 됨  성공하면 1 실패하면 0반환 
    public int AddItem(Item _item)
    {
        if(items.FindIndex(x => x == null) != -1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = _item;
                    slots[i].item = items[i];
                    isItemAdded = true;
                    FreshSlot();
                    break;
                }
            }
            return 1;
        }
        else
        {
            print("슬롯이 가득 차 있습니다.");
            return 0;
        }
    }


}