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

    GameObject equipWeapon; // 장착 중인 무기
    GameObject weaponQuickSlot;
    GameObject itemQuickSlots;
    public int equipWeaponIndex; // 장착 중인 무기의 weapons에서의 위치
    public int equipItemIndex;
    public bool isSwap; // 스왑 상태
    public bool isAttack; // 공격 상태
    public WeaponInventory weaponInventory; // 가지고 있는 무기
    public Inventory itemInventory; //가지고 있는 아이템
    public Transform RightHand;

    public BoxCollider colliderWeapon; // 무기들의 collider
    BoxCollider colliderHand; // 주먹 collider
    GameObject objWeapon; // 장착중인 무기 gameobject(무기 및 주먹)
    public string Armed; // 현재 장착중인 무기 타입

    public MovementStateManager movementStateManager;    
    private PhotonView pv;
    
    void Start(){
        pv = GetComponent<PhotonView>();
        itemQuickSlots = GameObject.Find("ItemQuickSlots");
        weaponQuickSlot = GameObject.Find("WeaponSlot");
        itemInventory = itemQuickSlots.GetComponent<Inventory>();
        weaponInventory = weaponQuickSlot.GetComponent<WeaponInventory>();
        objWeapon = RightHand.GetChild(1).gameObject; // 처음 시작할 때 주먹의 sphereCollider 받아옴
        colliderHand = RightHand.GetChild(1).GetComponent<BoxCollider>();
        movementStateManager = GetComponent<MovementStateManager>();
    }

    // 공격
    public void Attack(){ 
        if (Input.GetMouseButton(0))
            {   
                movementStateManager.anim.SetBool("Attack", true);
            }
        else movementStateManager.anim.SetBool("Attack", false);
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
            if (Input.GetButtonDown("Swap1") || weaponInventory.isWeaponAdded == true)
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
                    objWeapon = weapons[equipWeaponIndex];

                    if (objWeapon.GetComponent<ItemData>().itemData.ItemType <= 10)
                    {
                        colliderWeapon = objWeapon.GetComponent<BoxCollider>();
                        if (objWeapon.GetComponent<ItemData>().itemData.ItemType <= 3)
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
                // 무기 들었을 때 애니메이션 변경
                // swap 애니메이션 넣으면 사용
                //int weaponIndex = -1;
                //if (Input.GetButtonDown("Swap1")) weaponIndex = 0;
                //if (Input.GetButtonDown("Swap2")) weaponIndex = 1;
                //if (Input.GetButtonDown("Swap3")) weaponIndex = 2;
                //if (Input.GetButtonDown("Swap4")) weaponIndex = 3;
                //if (Input.GetButtonDown("Swap5")) weaponIndex = 4;
            }
            else if(Input.GetButtonDown("Swap2") || Input.GetButtonDown("Swap3") || Input.GetButtonDown("Swap4") || Input.GetButtonDown("Swap5") || itemInventory.isItemAdded == true)
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
                    objWeapon = weapons[equipItemIndex];
                    if (objWeapon.GetComponent<ItemData>().itemData.ItemType > 10)
                    {
                        colliderWeapon = objWeapon.GetComponent<BoxCollider>();

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
