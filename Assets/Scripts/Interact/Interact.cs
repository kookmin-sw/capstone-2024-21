using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

//Virtual Camera에 들어가 있음 
public class Interact : MonoBehaviour
{
    PhotonView pv;

    Transform canvas;
    GameObject image_F;//껐다 켰다 할 F UI. 
    GameObject circleGaugeControler; //껐다 켰다 할 게이지 컨트롤러 
    Inventory quicSlot; //아이템먹으면 나타나는 퀵슬롯 UI.  
    WeaponInventory WeaponQuickslot;


    public bool isInvetigating = false; //수색중인가?
    public bool isExiting = false; //탈출구에 배터리를 넣고 있는가? 

    GameObject ExitDoor;
    public string playerId;
    Vector3 PlayerMoveDir;

    //[HideInInspector] public Animator anim;


    RaycastHit hit;
    float interactDiastance = 4.0f;
    Transform selectedTarget;
    Vector3 raycastOffset = new Vector3(0f, -0.1f, 1.2f);

    private void Start()
    {
        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);

        canvas = GameObject.Find("Canvas").transform;

        //Find 함수는 해당 이름의 자식 오브젝트를 검색하고 트랜스폼을 반
        image_F = canvas.Find("image_F").gameObject;
        circleGaugeControler = canvas.Find("GaugeController").gameObject;
        quicSlot = canvas.Find("ItemQuickSlots").GetComponent<Inventory>();
        WeaponQuickslot = canvas.Find("WeaponSlot").GetComponent<WeaponInventory>();

        ExitDoor = GameObject.Find("exit");
}

    void Update()
    {
        Vector3 raycastStartingPoint = transform.position + transform.TransformDirection(raycastOffset); 
        Debug.DrawRay(raycastStartingPoint, transform.forward * interactDiastance, Color.blue, interactDiastance);//레이캐스트 디버그 

        //레이캐스트 발사 
        if (Physics.Raycast(raycastStartingPoint, transform.TransformDirection(Vector3.forward), out hit, interactDiastance))
        {
            //레이캐스트에 충돌한 물체가 Interact레이어라면 
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
            //레이캐스트에 충돌한 물체가 Interact레이어가 아니라면 
            else
            {
                //status 초기화 
                isInvetigating = false;
                isExiting = false;

                //셀렉한 물체가 있다면 unselect 
                if (selectedTarget)
                {
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
                    if (selectedTarget.name == "ExitCup")
                    {
                        selectedTarget.GetComponent<ExitCupOpen>().ChangeDoorStateRPC();
                    }
                    else if(selectedTarget.GetComponent<DoorRight>())
                    {   
                        selectedTarget.GetComponent<DoorRight>().ChangeDoorStateRPC();
                    }
                    else
                    {
                        selectedTarget.GetComponent<DoorLeft>().ChangeDoorStateRPC();
                    }
                }
                else if (selectedTarget.CompareTag("Spawner"))
                {
                    circleGaugeControler.GetComponent<InteractGaugeControler>().SetGuageZero();//수색 게이지 초기화
                    isInvetigating = true;//수색시작
                }
                else if (selectedTarget.CompareTag("Item"))
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
                else if (selectedTarget.CompareTag("Exit"))
                {
                    Debug.Log("Exit 문 상호작용 ");
                    FindMovedir();
                    if (PlayerMoveDir.magnitude < 0.1f && CheckInventoryBattery())
                    {
                        circleGaugeControler.GetComponent<InteractGaugeControler>().SetGuageZero();//수색 게이지 초기화
                        isExiting = true;
                    }
                }
            }

        }
        else  //레이캐스트가 Interactable 오브젝트와 충돌하지 않고 있다면
        {
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
                //수색을 성공적으로 마쳤다면 스폰
                if(selectedTarget.gameObject.GetComponent<BatterySpawner>() != null)
                {
                    selectedTarget.GetComponent<BatterySpawner>().SpawnItem();
                }
                else if (selectedTarget.gameObject.GetComponent<WeaponSpawner>() != null)
                {
                    selectedTarget.GetComponent<WeaponSpawner>().SpawnItem();
                }
                else if (selectedTarget.gameObject.GetComponent<ItemSpawner>() != null)
                {
                    selectedTarget.GetComponent<ItemSpawner>().SpawnItem();
                }

                //수색종료
                isInvetigating=false;
            }
        }
        else if (isExiting)
        {
            if (circleGaugeControler.GetComponent<InteractGaugeControler>().ExitFillCircle())
            {
                // 성공적으로 게이지가 다 찼다면
                EraseInventoryBattery(); //인벤토리에서 배터리 하나 지우고 
                AddExitChargeBatteryRPC(); // 차지한 배터리 하나 증가

                //맵 밝게하기
                MapManager.Instance.BightenLightRPC(15);

                // 문을 여는데 필요하한 갯수만큼 배터리를 차지했다면 탈출구 열기 
                if (MapManager.Instance.ExitNeedBattery == MapManager.Instance.ExitChargedBattery)
                {
                    ExitDoor.GetComponent<Exit>().ChangeExitDoorStateRPC();
                }
                else
                {
                    Debug.Log("탈출구를 열기 위해서는 배터리를 더 차지해야합니다 ");
                }

                //isExiting
                isExiting = false;
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

    bool CheckInventoryBattery()
    {
        bool check = false;
        for (int i = 0; i < quicSlot.items.Count; i++)
        {
            if (quicSlot.items[i] && quicSlot.items[i].itemName == "battery")
            {
                check = true;
                break;
            }
        }
        return check;
    }

    void EraseInventoryBattery()
    {
        //사용한 배터리 삭제
        int idx = 0;
        while (idx < quicSlot.items.Count)
        {
            if (quicSlot.items[idx] && quicSlot.items[idx].itemName == "battery")
            {
                quicSlot.slots[idx].item = null;
            }
            idx++;
        }
        quicSlot.FreshSlot();
        //isExiting = false; 
        Debug.Log("배터리를 사용했습니닫.");
    }

    [PunRPC]
    void AddExitChargeBattery()
    {
        MapManager.Instance.ExitChargedBattery++;
        Debug.Log("MapManager.Instance.ExitChargedBattery : "+ MapManager.Instance.ExitChargedBattery);
    }

    void AddExitChargeBatteryRPC()
    {
        pv.RPC("AddExitChargeBattery", RpcTarget.AllBuffered);
    }

    void clearTarget(Transform obj)
    {
        if (selectedTarget == null) return;

        removeOutline(obj);
        selectedTarget = null;
        //Debug.Log(obj.name + " is unselected");

        isInvetigating = false; //수색중이라면 취소하고
        circleGaugeControler.GetComponent<InteractGaugeControler>().DisableInteractGaugeImage(); //게이지UI끄기 

    }

    void selectTarget(Transform obj)
    {
        if (obj == null) return;

        clearTarget(selectedTarget); //이전에 이미 선택했던 오브젝트는 원래대로

        selectedTarget = obj;
        //Debug.Log("selectTarget is " + obj.name);
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

