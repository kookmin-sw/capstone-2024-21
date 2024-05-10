using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Virtual Camera에 들어가 있음 
public class Interact : MonoBehaviour
{
    public Transform canvas;

    public GameObject image_F;//껐다 켰다 할 F UI. 직접 할당해줘야함 findObject로 바꿀까 고민중 
    public GameObject circleGaugeControler; //껐다 켰다 할 게이지  UI. 직접 할당해줘야함
    public Inventory quicSlot; //아이템먹으면 나타나는 퀵슬롯 UI. 직접 할당해줘야함
    public WeaponInventory WeaponQuickslot;

    public bool isInvetigating = false; //수색중인가? -> update문에서 상태를 체크하여 게이지 UI 뜨고 지우고 함 
    public bool isExiting = false;
    public string playerId;
    Vector3 PlayerMoveDir;

    //[HideInInspector] public Animator anim;


    RaycastHit hit;
    float interactDiastance = 4.0f;
    Transform selectedTarget;

    Vector3 raycastOffset = new Vector3(0f, -0.1f, 1.2f);

    //CheckBattery 함수를 위한 변수
    [SerializeField] int cntBattery;
    [SerializeField] int needBattery = 0;

    private void Start()
    {
        canvas = GameObject.Find("Canvas").transform;

        //Find 함수는 해당 이름의 자식 오브젝트를 검색하고 트랜스폼을 반
        image_F = canvas.Find("image_F").gameObject;
        circleGaugeControler = canvas.Find("GaugeController").gameObject; 
        quicSlot = canvas.Find("ItemQuickSlots").GetComponent<Inventory>();
        WeaponQuickslot = canvas.Find("WeaponSlot").GetComponent<WeaponInventory>();
        //anim = GetComponent<Animator>();
}

    void Update()
    {
        Vector3 raycastStartingPoint = transform.position + transform.TransformDirection(raycastOffset); 
        Debug.DrawRay(raycastStartingPoint, transform.forward * interactDiastance, Color.blue, interactDiastance);//레이캐스트 디버그 

        //레이캐스트 발사 
        if (Physics.Raycast(raycastStartingPoint, transform.TransformDirection(Vector3.forward), out hit, interactDiastance))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interact"))
            {
                //충돌한 물체의 레이어가 Interact고, 셀렉된 타겟이 없거나 새로운 오브젝트라면 새로 셀렉 
                if (selectedTarget == null || selectedTarget != hit.transform)
                {
                    Transform obj = hit.transform;
                    selectTarget(obj); //레이캐스트가 충돌한 물체를 셀렉하고 
                    addOutline(obj); // 아웃라인 추가 
                    image_F.GetComponent<UIpressF>().show_image(); //UI에서 F 이미지 활성화 
                }
            }
            else
            {
                if (selectedTarget)
                {
                    isExiting = false;
                    removeOutline(selectedTarget);
                    clearTarget(selectedTarget);
                    image_F.GetComponent<UIpressF>().remove_image();
                }
            }


            //F를 누르면 상호작용 
            if (selectedTarget != null && Input.GetKeyDown(KeyCode.F))
            {
                if (selectedTarget.CompareTag("door"))
                {
                    //Debug.Log("문 상호작용 ");
                    if (selectedTarget.GetComponent<DoorRight>())
                    {   
                        selectedTarget.GetComponent<DoorRight>().ChangeDoorStateRPC();
                    }
                    else
                    {
                        selectedTarget.GetComponent<DoorLeft>().ChangeDoorStateRPC();
                    }

                }

                if (selectedTarget.CompareTag("Exit"))
                {   
                    Debug.Log("Exit 문 상호작용 ");
                    FindMovedir();
                    if(PlayerMoveDir.magnitude < 0.1f && CheckBattery())
                        isExiting = true;
                }

                if (selectedTarget.CompareTag("ItemSpawner"))
                {
                    //Debug.Log("betterySpawner 와 상호작용");

                    circleGaugeControler.GetComponent<InteractGaugeControler>().SetGuageZero();//수색 게이지 초기화하고
                    circleGaugeControler.GetComponent<InteractGaugeControler>().AbleInvestinGaugeUI(); //게이지UI켜고 
                    isInvetigating = true;//수색시작
                }

                if (selectedTarget.CompareTag("Item"))
                {
                    //Debug.Log(hit.collider.gameObject.name + " item과 상호작용");
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
                        if(WeaponQuickslot.AddWeapon(item) == 1)
                        {
                            if (hit.collider.gameObject.GetComponent<Weapon>().settedLightning == true) //습득하는 무기가 조합무기면 
                            {
                                item.craftCompleted = true; //아이템 정보도 조합무기로
                                item.ItemDamage *= 2; //아이템 데미지도 조합무기 데미지로
                                WeaponQuickslot.craftCompletedMark.SetActive(true);
                            }
                            else if (hit.collider.gameObject.GetComponent<Weapon>().settedLightning == false) //습득하는 무기가 일반무기면 
                            {
                                item.craftCompleted = false;  //아이템 정보도 일반무기로
                            }
                            //무기 넣기에 성공할때만 디스트로이
                            Destroy(hit.collider.gameObject);
                            image_F.GetComponent<UIpressF>().remove_image();
                        }
                    }
                }
            }

        }
        else  //레이캐스트가 Interactable 오브젝트와 충돌하지 않고 있다면
        {
            if (selectedTarget)            //셀렉된 타겟이 있다면
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
                //수색을 성공적으로 마쳤다면 스폰
                if(selectedTarget.gameObject.GetComponent<BatterySpawner>() != null)
                {
                    selectedTarget.GetComponent<BatterySpawner>().SpawnItem();
                }
                else if (selectedTarget.gameObject.GetComponent<WeaponSpawner>() != null)
                {
                    selectedTarget.GetComponent<WeaponSpawner>().SpawnItem();
                }


                //수색종료
                isInvetigating = false; 
                circleGaugeControler.GetComponent<InteractGaugeControler>().DisableInvestinGaugeUI();
            }
        }
        else if (isExiting)
        {
            if (circleGaugeControler.GetComponent<InteractGaugeControler>().ExitFillCircle())
            {
                //수색종료
                Tmp();
            }
        }

    }

    public void FindMovedir()
    {
        playerId = GameManager.Instance.UserId;
        GameObject obj = GameObject.Find(playerId);
        MovementStateManager movement = obj.GetComponent<MovementStateManager>();
        PlayerMoveDir = movement.moveDir;
    }

    bool CheckBattery()
    {
        //현재 가지고 있는 배터리 갯수 확인
        cntBattery = 0;
        for (int i = 0; i < quicSlot.items.Count; i++)
        {
            Debug.Log("quicSlot.items[i] : "+quicSlot.items[i]);
            if (quicSlot.items[i] && quicSlot.items[i].itemName == "battery")
            {
                cntBattery++;
            }
        }
        Debug.Log("현재 소지한 배터리 갯수 : " + cntBattery);

        if (cntBattery >= needBattery)
        {
            return true;
        }
        else
        {   
            Debug.Log("배터리가 부족합니다.");
            return false;
        }
    }

    void Tmp()
    {
        //needBattery개 이상이면 문을 열고 가지고 있는 배터리 3개 삭제 

        Debug.Log("배터리 충분. ");
        if (selectedTarget.GetComponent<Exit>())
        {
            selectedTarget.GetComponent<Exit>().ChangeExitDoorStateRPC();
        }

        //사용한 배터리 삭제
        cntBattery = 0;
        int idx = 0;

        while (cntBattery < needBattery)
        {
            Debug.Log("idx : " + idx);
            if (quicSlot.items[idx] && quicSlot.items[idx].itemName == "battery")
            {
                Debug.Log("hit");
                quicSlot.slots[idx].item = null;

                cntBattery++;
            }
            idx++;
        }
        quicSlot.FreshSlot();
        isExiting = false; 
        Debug.Log("배터리를 사용해서 문을 열였습니다.");
    }


    void clearTarget(Transform obj)
    {
        if (selectedTarget == null) return;

        removeOutline(obj);
        selectedTarget = null;
        Debug.Log(obj.name + " is unselected");


        isInvetigating = false; //수색중이라면 취소하고
        circleGaugeControler.GetComponent<InteractGaugeControler>().DisableInvestinGaugeUI(); //게이지UI끄기 

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

