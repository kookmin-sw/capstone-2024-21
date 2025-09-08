using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickslotManager : MonoBehaviour
{
    [SerializeField] private List<Item> items; 
    [SerializeField] private SlotData[] slots;
    [SerializeField] private Shadow[] slotOutlines;
    public bool isItemAdded;
    public bool isSlotChanged;

    public void FreshSlot()
    {
        slots = GetComponentsInChildren<SlotData>();
        
        for (int i = 0; i < slots.Length; i++)
        {
            items[i] = slots[i].item;
        }
    }

    void ControlCraftSlot()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            gameObject.SetActive(true);
            CursorX.Free(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            gameObject.SetActive(false);
            CursorX.Free(false);
        }
    }
    //퀵슬롯 1,2,3,4,5로 선택
    public void SelectQuickSlot()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeSlot(4);
    }
    //이전 선택 슬롯 비활성화, 현재 선택 슬롯 활성화
    void ChangeSlot(int pressValue)
    {
        slots[selectSlot].Deselected();
        slots[pressValue].Selected();
        selectSlot = pressValue;
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

    void AbandonItem()
    {

    }

    void DieRoot()
    {
        //아이템 버리기
        if (uiManager.weaponInventory.weaponSlot.item != null)
        {
            DroppedItem = PhotonNetwork.Instantiate("Prefabs/" + uiManager.weaponInventory.weaponSlot.item.itemName, SpawnPos, transform.rotation);
            uiManager.weaponInventory.weaponSlot.item = null;
        }
        for (int i = 0; i < 4; i++)
        {
            if (uiManager.inventory.slots[i].item != null)
            {
                DroppedItem = PhotonNetwork.Instantiate("Prefabs/" + uiManager.inventory.slots[i].item.itemName, SpawnPos, transform.rotation);
                uiManager.inventory.slots[i].item = null;
                uiManager.inventory.FreshSlot();
            }
        }
        GameManager.Instance.GameOver();
        uiManager.isUIActivate = true;

        // 죽었을때 아이템 루팅
        if (attackManager.weaponInventory.abandonedItem != null) //버릴 무기가 있으면
        {
            // DroppedItem = Instantiate(attackManager.weaponInventory.abandonedItem.itemPrefab); //프리펩 생성
            Debug.Log("아이템 버림");
            Vector3 SpawnPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z + 1);

            DroppedItem = PhotonNetwork.Instantiate("Prefabs/" + attackManager.weaponInventory.abandonedItem.itemName, SpawnPos, transform.rotation);
            if (attackManager.weaponInventory.abandonedItem.ItemType < 11)
            {
                if (attackManager.weaponInventory.abandonedItem.craftCompleted == true)
                {
                    DroppedItem.GetComponent<Weapon>().settedLightning = true;
                    DroppedItem.GetComponent<ItemData>().itemData.ItemDamage *= 2;
                }
                else if (attackManager.weaponInventory.abandonedItem.craftCompleted == false)
                {
                    DroppedItem.GetComponent<Weapon>().settedLightning = false;
                }
                attackManager.weaponInventory.abandonedItem.craftCompleted = false;
            }
            attackManager.weaponInventory.abandonedItem = null;
        }
    }
}