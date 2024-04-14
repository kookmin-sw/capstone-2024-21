using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data.SqlTypes;
using ExitGames.Client.Photon;
using static UnityEditor.Progress;

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
        if(eventData.pointerDrag != null)
        {
            // 슬롯에 아이콘이 있으면 아이콘 교체
            DraggableUI draggedUI = eventData.pointerDrag.GetComponent<DraggableUI>();
            if (transform.childCount > 0)
            {
                Transform existingIcon = transform.GetChild(0);
                existingIcon.SetParent(draggedUI.preSlot);
                if (existingIcon.transform.parent == batterySlot)
                {
                    itemSlots.DeleteItem(existingIcon.GetComponent<Slot>().item);
                    existingIcon.position = draggedUI.preSlot.position;
                }
                //else if(existingIcon.GetComponent<DraggableUI>().preSlot == batterySlot) //교체로 옮겨가는 아이템의 전 슬롯이 배터리슬롯이고 아이템슬롯으로 옮겨가면 아이템리스트에 더하기
                //{
                //    //itemSlots.AddItem(existingIcon.GetComponent<Slot>().item);
                //    existingIcon.position = draggedUI.preSlot.position;
                //    itemSlots.FreshSlot();

                //}
                else
                {
                    existingIcon.position = draggedUI.preSlot.position;
                    itemSlots.FreshSlot();
                }

            }
            eventData.pointerDrag.transform.SetParent(transform);
            if(eventData.pointerDrag.transform.parent == batterySlot)
            {
                itemSlots.DeleteItem(eventData.pointerDrag.GetComponent<Slot>().item);
                eventData.pointerDrag.GetComponent<RectTransform>().position = slotRect.position;
            }
            //else if(eventData.pointerDrag.GetComponent<DraggableUI>().preSlot == batterySlot) // 드롭으로 옮겨 지는 아이템의 전 슬롯이 배터리 슬롯이고 아이템 슬롯으로 옮겨가면 아이템리스트에 더하기
            //{
            //    //itemSlots.AddItem(eventData.pointerDrag.GetComponent<Slot>().item);
            //    eventData.pointerDrag.GetComponent<RectTransform>().position = slotRect.position;
            //    itemSlots.FreshSlot();

            //}
            else
            {
                eventData.pointerDrag.GetComponent<RectTransform>().position = slotRect.position;
                itemSlots.FreshSlot();
            }

            //if (eventData.pointerDrag.GetComponent<RectTransform>().position == firstComSlotRect.position)
            //{
            //    craftSlot.items.Add() firstComSlot.item;
            //}
            //else if (eventData.pointerDrag.GetComponent<RectTransform>().position == secondComSlotRect.position)
            //{
            //    craftSlot.items[1] = secondComSlot.item;
            //}

        }
    }


}
