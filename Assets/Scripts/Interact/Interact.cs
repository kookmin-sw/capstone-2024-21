using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Virtual Camera에 들어가 있음 
public class Interact : MonoBehaviour
{

    public GameObject image_F;//껐다 켰다 할 F UI. 직접 할당해줘야함 findObject로 바꿀까 고민중 
    public GameObject circleGaugeControler; //껐다 켰다 할 게이지  UI. 직접 할당해줘야함
    public Inventory quicSlot; //아이템먹으면 나타나는 퀵슬롯 UI. 직접 할당해줘야함
    public WeaponInventory WeaponQuickslot;

    public bool isInvetigating = false; //수색중인가? -> update문에서 상태를 체크하여 게이지 UI 뜨고 지우고 함 


    RaycastHit hit;
    float interactDiastance = 4.0f;
    Transform selectedTarget;

    Vector3 raycastOffset = new Vector3(0f, 0f, 1.4f);

    void Update()
    {
        Vector3 raycastStartingPoint = transform.position + transform.TransformDirection(raycastOffset); 
        Debug.DrawRay(raycastStartingPoint, transform.forward * interactDiastance, Color.blue, interactDiastance);//레이캐스트 디버그 

        //LayerMask.GetMask("Interact") : raycast가 Interact 레이어와만 상호작용

        //레이캐스트 발사 
        if (Physics.Raycast(raycastStartingPoint, transform.TransformDirection(Vector3.forward), out hit, interactDiastance, LayerMask.GetMask("Interact")))
        {
            //셀렉된 타겟이 없거나 새로운 오브젝트라면 새로 셀렉 
            if (selectedTarget == null || selectedTarget != hit.transform)
            {
                Transform obj = hit.transform; 
                selectTarget(obj); //레이캐스트가 충돌한 물체를 셀렉하고 
                addOutline(obj); // 아웃라인 추가 
                image_F.GetComponent<UIpressF>().show_image(); //UI에서 F 이미지 활성화 
            }

            //F를 누르면 상호작용 
            if (selectedTarget != null && Input.GetKeyDown(KeyCode.F))
            {
                if (selectedTarget.CompareTag("door"))
                {
                    Debug.Log("문 상호작용 ");
                    if (selectedTarget.GetComponent<DoorRight>())
                    {
                        selectedTarget.GetComponent<DoorRight>().ChangeDoorState();
                    }
                    else
                    {
                        selectedTarget.GetComponent<DoorLeft>().ChangeDoorState();
                    }

                }

                if (selectedTarget.CompareTag("ItemSpawner"))
                {
                    Debug.Log("betterySpawner 와 상호작용");

                    circleGaugeControler.GetComponent<InteractGaugeControler>().SetGuageZero();//수색 게이지 초기화하고
                    circleGaugeControler.GetComponent<InteractGaugeControler>().AbleInvestinGaugeUI(); //게이지UI켜고 
                    isInvetigating = true;//수색시작
                }

                if (selectedTarget.CompareTag("Item"))
                {
                    Debug.Log(hit.collider.gameObject.name + " item과 상호작용");
                    ItemData itemdata = hit.collider.gameObject.GetComponent<ItemData>();
                    Item item = itemdata.itemData;
                    if (item.ItemType > 10)
                    {
                        if (quicSlot.AddItem(item) == 1)
                        {
                            //아이템 넣기에 성공할때만 디스트로이
                            Destroy(hit.collider.gameObject);
                            image_F.GetComponent<UIpressF>().remove_image();
                        }
                    }
                    else
                    {
                        WeaponQuickslot.AddItem(item);
                        Destroy(hit.collider.gameObject);
                        image_F.GetComponent<UIpressF>().remove_image();
                    }
                }
            }

        }
        else
        {
            //레이캐스트가 Interactable 오브젝트와 충돌하지 않고 있다면


            //셀렉된 타겟이 있다면
            if (selectedTarget)
            {
                removeOutline(selectedTarget);
                clearTarget(selectedTarget);
                image_F.GetComponent<UIpressF>().remove_image();
            }
        }




        //수색여부(isInvetigating)에 따라 실행됨. 수색중이면 게이지 증가 
        if (isInvetigating)
        {
            if (circleGaugeControler.GetComponent<InteractGaugeControler>().FillCircle())
            {
                //수색을 성공적으로 마쳤다면 아이템 스폰 
                selectedTarget.GetComponent<ItemSpawner>().SpawnItem();

                //수색종료
                isInvetigating = false; 
                circleGaugeControler.GetComponent<InteractGaugeControler>().EnableInvestinGaugeUI();
            }
        }

    }



    void clearTarget(Transform obj)
    {
        if (selectedTarget == null) return;

        removeOutline(obj);
        selectedTarget = null;
        Debug.Log(obj.name + " is unselected");


        isInvetigating = false; //수색중이라면 취소하고
        circleGaugeControler.GetComponent<InteractGaugeControler>().EnableInvestinGaugeUI(); //게이지UI끄기 

    }

    void selectTarget(Transform obj)
    {
        if (obj == null) return;

        clearTarget(selectedTarget); //이전에 이미 선택했던 오브젝트는 원래대로

        selectedTarget = obj;
        Debug.Log("selectTarget is " + obj.name);
        addOutline(obj);
    }

    void addOutline(Transform obj)
    {
        //Outline 컴포넌트가 있으면 그냥 활성화
        if (obj.gameObject.GetComponent<Outline>() != null)
        {
            obj.gameObject.GetComponent<Outline>().enabled = true;
        }
        else
        {
            //없으면 찾아서 넣어줌
            Outline outline = obj.gameObject.AddComponent<Outline>();
            outline.enabled = true;
            obj.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
            obj.gameObject.GetComponent<Outline>().OutlineWidth = 5.0f;
        }
    }

    void removeOutline(Transform obj)
    {
        if (obj != null)
        {
            obj.gameObject.GetComponent<Outline>().enabled = false;
        }
    }


}

