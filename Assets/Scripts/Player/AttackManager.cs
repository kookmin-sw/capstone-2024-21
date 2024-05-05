using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEditor.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AttackManager : MonoBehaviour
{
    public GameObject[] weapons; // 모든 무기 배열

    [SerializeField]GameObject equipWeapon; // 장착 중인 무기
    GameObject weaponQuickSlot;
    GameObject itemQuickSlots;
    public int equipWeaponIndex; // 장착 중인 무기의 weapons에서의 위치
    public int equipItemIndex;
    public bool isSwap; // 스왑 상태
    public bool isAttack; // 공격 상태
    public WeaponInventory weaponInventory; // 가지고 있는 무기
    public Inventory itemInventory; //가지고 있는 아이템
    public Transform RightHand;
    ///////Attack
    float fireDelay;
    bool fDown;
    bool isFireReady;
    ////// Attack
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool sDown4;
    bool sDown5;
    bool gDown;
    bool eDown;

    public BoxCollider colliderWeapon; // 무기들의 collider
    BoxCollider colliderHand; // 주먹 collider
    public WeaponManager equipWeaponGameobject; // 장착중인 무기 gameobject(무기 및 주먹)
    
    public string Armed; // 현재 장착중인 무기 타입

    public MovementStateManager movementStateManager;   
    [HideInInspector] public AttackManager attackManager;
    [HideInInspector] public WeaponManager weaponManager; 
    private PhotonView pv;
    
    void Start(){
        pv = GetComponent<PhotonView>();
        itemQuickSlots = GameObject.Find("ItemQuickSlots");
        weaponQuickSlot = GameObject.Find("WeaponSlot");
        itemInventory = itemQuickSlots.GetComponent<Inventory>();
        weaponInventory = weaponQuickSlot.GetComponent<WeaponInventory>();
        equipWeaponGameobject = RightHand.GetChild(1).gameObject.GetComponent<WeaponManager>(); // 처음 시작할 때 주먹의 sphereCollider 받아옴
        equipWeapon = weapons[9];
        colliderHand = RightHand.GetChild(1).GetComponent<BoxCollider>();
        movementStateManager = GetComponent<MovementStateManager>();
    }
    
    void Update(){
        getInput();
        Attack();  
    }

    void getInput(){
        sDown1 = Input.GetButtonDown("sDown1"); // 스왑 1 ~ 5
        sDown2 = Input.GetButtonDown("sDown2");
        sDown3 = Input.GetButtonDown("sDown3");
        sDown4 = Input.GetButtonDown("sDown4");
        sDown5 = Input.GetButtonDown("sDown5");
        gDown = Input.GetButtonDown("gDown"); // 아이템 버리기
        eDown = Input.GetButtonDown("eDown"); // 아이템 장착
    }

    // 공격
    void Attack()
    {
        fireDelay += Time.deltaTime;
        isFireReady = equipWeaponGameobject.rate < fireDelay; 

        // 마우스 왼쪽 버튼을 누르고 공격이 가능한 상태이면 실행
        if (Input.GetMouseButton(0) && isFireReady)
        {
            // 무기를 사용하고 애니메이션을 트리거합니다.
            equipWeaponGameobject.Use(); // 무기 사용
            movementStateManager.anim.SetBool("Attack", true); // 애니메이션 트리거
            fireDelay = 0; // 공격 딜레이 초기화
        }
        else movementStateManager.anim.SetBool("Attack", false); // 애니메이션 트리거
    }

    // 무기 장착 시와 비 장착시 공격 시작(collider on)
    public void AttackStart(){
        
        if(Armed != "") {
            Debug.Log("WeapColl On");
            colliderWeapon.enabled = true;
        }
        else {
            Debug.Log("HandColl On");
            colliderHand.enabled = true;
        }
    }

    // 무기 장착 시와 비 장착시 공격 끝(collider off)
    public void AttackEnd(){
        Debug.Log("coll Off");
        if(Armed != "") {
            colliderWeapon.enabled = false;
        }
        else {
            colliderHand.enabled = false;
        }
    }

    // 공격 시작
    public void AttackIn(){
        isAttack = true;
    }

    // 공격 끝
    public void AttackOut(){
        isAttack = false;   
    }   

    // 스왑 끝
    void SwapOut(){
        isSwap = false;
        movementStateManager.anim.SetTrigger("SwapOut");
    }

    [PunRPC]
    void Swap(){
        
        if(pv.IsMine)
        {
            if (isAttack) return; // 공격 중에는 스왑 불가


            if (Input.GetKeyDown(KeyCode.G)) 
            { // G는 버리는 키라서 인벤토리에서도 빼기
                if(weaponQuickSlot.GetComponentInChildren<SelectedSlot>().slotOutline.enabled) //무기 버릴 때
                {
                    if (equipWeapon.activeSelf == true)
                    {
                        weaponInventory.abandonedItem = weaponInventory.weaponSlot.item;
                        weaponInventory.weaponSlot.item = null;

                        RpcEquip(-1);
                        movementStateManager.anim.SetTrigger("doSwap");
                        isSwap = true;
                        Invoke("SwapOut", 0.3f);

                        movementStateManager.anim.SetBool(Armed, false);
                        movementStateManager.anim.SetLayerWeight(1, 0);
                        movementStateManager.anim.SetLayerWeight(2, 0);
                        Armed = "";
                    }
                }
                else //아이템 버릴 때
                {
                    for(int i = 0; i < 4; i++)
                    {
                        if(itemQuickSlots.GetComponentsInChildren<SelectedSlot>()[i].slotOutline.enabled)
                        {
                            if (equipWeapon.activeSelf == true)
                            {
                                weaponInventory.abandonedItem = itemQuickSlots.GetComponent<Inventory>().slots[i].item;
                                itemQuickSlots.GetComponent<Inventory>().slots[i].item = null;
                                itemQuickSlots.GetComponent<Inventory>().FreshSlot();
                                RpcEquip(-1);
                                movementStateManager.anim.SetTrigger("doSwap");
                                isSwap = true;
                                Invoke("SwapOut", 0.3f);

                                movementStateManager.anim.SetBool(Armed, false);
                                movementStateManager.anim.SetLayerWeight(1, 0);
                                movementStateManager.anim.SetLayerWeight(2, 0);
                                Armed = "";
                                break;
                            }

                        }
                    }
                }

            }
            // 버튼을 입력 받으면 
            if (sDown1 || weaponInventory.isWeaponAdded == true)
            {
                equipWeaponIndex = -1;
                if (weaponInventory.weaponSlot.item != null && weaponQuickSlot.GetComponentInChildren<SelectedSlot>().slotOutline.enabled)
                {
                    for (int i = 0; i < weapons.Length; i++)
                    {
                        if (weaponInventory.weaponSlot.item.ItemType == weapons[i].GetComponent<ItemData>().itemData.ItemType)
                        {
                            equipWeaponIndex = i;
                            weaponInventory.isWeaponAdded = false;
                            break;
                        }
                    }
                    
                }
                if(equipWeaponIndex != -1)
                {
                    equipWeaponGameobject = weapons[equipWeaponIndex].GetComponent<WeaponManager>();

                    if (equipWeaponGameobject.GetComponent<ItemData>().itemData.ItemType <= 10)
                    {
                        colliderWeapon = equipWeaponGameobject.GetComponent<BoxCollider>();
                        if (equipWeaponGameobject.GetComponent<ItemData>().itemData.ItemType <= 3)
                        {
                            movementStateManager.anim.SetBool(Armed, false);
                            movementStateManager.anim.SetLayerWeight(1, 0);

                            Armed = "THW";
                            movementStateManager.anim.SetBool(Armed, true);
                            movementStateManager.anim.SetLayerWeight(2, 1);

                        }
                        else
                        {
                            movementStateManager.anim.SetBool(Armed, false);
                            movementStateManager.anim.SetLayerWeight(2, 0);
                            Armed = "OHW";
                            movementStateManager.anim.SetBool(Armed, true);
                            movementStateManager.anim.SetLayerWeight(1, 1);

                        }
                    }

                    RpcEquip(equipWeaponIndex);

                    movementStateManager.anim.SetTrigger("doSwap");

                    isSwap = true;

                    Invoke("SwapOut", 0.3f);
                }
                else if (equipWeaponIndex == -1 && weaponInventory.isWeaponAdded == false)
                {
                    Debug.Log("무기 칸 비어있음");
                    if (equipWeapon.activeSelf == true)
                    {
                        RpcEquip(equipWeaponIndex);
                        movementStateManager.anim.SetTrigger("doSwap");
                        isSwap = true;

                        Invoke("SwapOut", 0.3f);
                    }

                    movementStateManager.anim.SetLayerWeight(1, 0);
                    movementStateManager.anim.SetLayerWeight(2, 0);
                    movementStateManager.anim.SetBool("OHW", false);
                    movementStateManager.anim.SetBool("THW", false);
                }
            }
            else if(sDown2 || sDown3 || sDown4 || sDown5 || itemInventory.isItemAdded == true || itemInventory.isSlotChanged == true)
            {
                equipItemIndex = -1;
                bool isFound = false;
                for (int i = 0; i < 4; i++)
                {
                    if (itemQuickSlots.GetComponentsInChildren<SelectedSlot>()[i].slotOutline.enabled)
                    {
                        if(itemQuickSlots.GetComponent<Inventory>().slots[i].item != null)
                        {
                            for (int j = weapons.Length - 1; j >= 0; j--)
                            {
                                if (itemQuickSlots.GetComponent<Inventory>().slots[i].item.ItemType == weapons[j].GetComponent<ItemData>().itemData.ItemType)
                                {
                                    equipItemIndex = j;
                                    itemInventory.isItemAdded = false;
                                    itemInventory.isSlotChanged = false;
                                    isFound = true;
                                    break;
                                }
                            }
                            if(isFound == true)
                            {
                                break;
                            }
                        }
                    }
                }
                if (equipItemIndex != -1)
                {
                    equipWeaponGameobject = weapons[equipItemIndex].GetComponent<WeaponManager>();
                    itemInventory.isSlotChanged = false;
                    if (equipWeaponGameobject.GetComponent<ItemData>().itemData.ItemType > 10)
                    {
                        colliderWeapon = equipWeaponGameobject.GetComponent<BoxCollider>();

                        movementStateManager.anim.SetBool(Armed, false);
                        movementStateManager.anim.SetLayerWeight(2, 0);
                        Armed = "OHW";
                        movementStateManager.anim.SetBool(Armed, true);
                        movementStateManager.anim.SetLayerWeight(1, 1);
                    }
                    RpcEquip(equipItemIndex);

                    movementStateManager.anim.SetTrigger("doSwap");

                    isSwap = true;

                    Invoke("SwapOut", 0.3f);
                }
                else if(equipItemIndex == -1 && itemInventory.isItemAdded == false)
                {
                    itemInventory.isSlotChanged = false;
                    Debug.Log("아이템 칸 비어있음");
                    if(equipWeapon.activeSelf == true)
                    {
                        RpcEquip(equipItemIndex);
                        movementStateManager.anim.SetTrigger("doSwap");
                        isSwap = true;

                        Invoke("SwapOut", 0.3f);
                    }
                    movementStateManager.anim.SetLayerWeight(1, 0);
                    movementStateManager.anim.SetLayerWeight(2, 0);
                    movementStateManager.anim.SetBool("OHW", false);
                    movementStateManager.anim.SetBool("THW", false);
                }

            }
        }
    }

    [PunRPC]
    void RPCWeaponEquip(int RpcEquipWeaponIndex){
        if(RpcEquipWeaponIndex == -1)
        {
            if(equipWeapon != null)
            {
                equipWeapon.SetActive(false);
            }
        }
        else
        {
            if(equipWeapon != null)
               equipWeapon.SetActive(false);

            equipWeapon = weapons[RpcEquipWeaponIndex];
            equipWeapon.SetActive(true);
        }
    }

    public void RpcEquip(int RpcEquipWeaponIndex){
        pv.RPC("RPCWeaponEquip", RpcTarget.All, RpcEquipWeaponIndex);
    }

    public void RpcSwap()
    {
        pv.RPC("Swap", RpcTarget.All);
    }

    void hitOut(){
        movementStateManager.anim.SetTrigger("HitOut");
    }
    public void OnDamaged()
    {   
        Debug.Log("doDamaged");

        //movementStateManager.animation
        movementStateManager.anim.SetTrigger("doDamaged");
        Invoke("hitOut", 0.1f);
    }
}
