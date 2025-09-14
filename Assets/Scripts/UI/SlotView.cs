using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotView : MonoBehaviour
{
    public Slot slot;
    private Image slotImage;

    void Awake()
    {
        slotImage = transform.GetChild(0).GetComponent<Image>();
    }

    public void InitSlotItem(SlotData slotData)
    {
        if (slotData == null) slotImage.sprite = null;
        else slotImage.sprite = slotData.item.itemImage;
    }
}