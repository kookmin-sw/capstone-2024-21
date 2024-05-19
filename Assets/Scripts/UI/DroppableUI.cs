using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data.SqlTypes;
using ExitGames.Client.Photon;
// using static UnityEditor.Progress;


public class DroppableUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image slotImage;
    private Color preColor;
    private Color hoverColor;
    private RectTransform slotRect;

    [SerializeField] private Transform batterySlot;
    [SerializeField] private Inventory itemSlots;
    void Awake()
    {
        slotImage = GetComponent<Image>();
        slotRect = GetComponent<RectTransform>();
        preColor = slotImage.color;
    }

    //마우스 포인터가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverColor = Color.white;
        hoverColor.a = 0.6f;
        slotImage.color = hoverColor;
    }

    //마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        slotImage.color = preColor;
    }

    // 현재 아이템 슬롯 영역 내부에서 드롭을 했을 때 1회 호출
    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag = 드래그중인 아이콘 / 드래그하고있는 아이콘이 있으면
        if(eventData.pointerDrag.GetComponent<Slot>().item != null)
        {
            DraggableUI draggedUI = eventData.pointerDrag.GetComponent<DraggableUI>();
            if (transform.childCount > 0) //드롭한 슬롯이 아이템을 가지고 있으면
            {
                Transform existingIcon = transform.GetChild(0);
                existingIcon.SetParent(draggedUI.preSlot);
                if (existingIcon.transform.parent == batterySlot)
                {
                    itemSlots.FreshSlot();                    //옮겨지는 곳이 배터리 슬롯이면 슬롯리스트에서 삭제
                    existingIcon.position = draggedUI.preSlot.position; //옮겨진 슬롯의 아이템이 옴기는 슬롯의 위치로
                    itemSlots.isSlotChanged = true;
                }
                else
                {
                    existingIcon.position = draggedUI.preSlot.position;
                    itemSlots.FreshSlot();
                    itemSlots.isSlotChanged = true;
                }
            }
            eventData.pointerDrag.transform.SetParent(transform);

            if (eventData.pointerDrag.transform.parent == batterySlot) //드롭한 슬롯으로 드래그한 아이템 위치 변경
            {
                eventData.pointerDrag.transform.SetParent(batterySlot);
                itemSlots.FreshSlot();
                eventData.pointerDrag.GetComponent<RectTransform>().position = batterySlot.GetComponent<RectTransform>().position;
                itemSlots.isSlotChanged = true;
            }
            else
            {
                eventData.pointerDrag.GetComponent<RectTransform>().position = slotRect.position;
                itemSlots.FreshSlot();
                itemSlots.isSlotChanged = true;
            }

        }
    }


}
