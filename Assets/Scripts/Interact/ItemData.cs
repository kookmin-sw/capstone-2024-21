using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//게임 오브젝트에서 아이템에 대한 데이터를 불러 올 수 있도록 하기위한 스크립트. 게임오브젝트 프리팹에 넣어주면됨
public class ItemData : MonoBehaviour
{

    public Item itemData;

    Item getItemData()
    {
        return itemData;
    }
}
