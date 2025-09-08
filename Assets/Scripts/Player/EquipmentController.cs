using Photon.Pun;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    [Header("Inventory/Weapon Setup")]
    public GameObject[] weapons;      // 아이템/무기 프리팹 배열 (index 기반)
    [SerializeField] GameObject equipWeapon; // 현재 장착된 무기 GO
    public WeaponInventory weaponInventory;   // 무기 인벤토리
    public QuickslotManager itemInventory;    // 아이템 인벤토리(퀵슬롯)
    public Transform RightHand;

    [Header("State")]
    public int equipWeaponIndex; // 무기 인벤토리에서 장착할 인덱스
    public int equipItemIndex;   // 아이템 인벤토리에서 장착할 인덱스
    public bool isSwap;          // 스왑 중 여부
    public string Armed = "";    // Animator bool 이름 캐시 (OHW/THW)

    [Header("Refs")]
    public MovementStateManager movementStateManager;
    public HpManager hpManager;
    public AttackManager attackManager;     // 공격 중에는 스왑 막기 위해 참조
    public WeaponManager equipWeaponGameobject; // 현재 장착 무기(스크립트)
    public BoxCollider colliderWeapon;      // 현재 무기 콜라이더
    BoxCollider colliderHand;               // 주먹 콜라이더

    // 입력
    bool sDown1, sDown2, sDown3, sDown4, sDown5, gDown, eDown;

    // 내부
    PhotonView pv;
    GameObject weaponQuickSlot;
    GameObject itemQuickSlots;

    void Awake()
    {
        pv = GetComponent<PhotonView>();

        itemQuickSlots = GameObject.Find("ItemQuickSlots");
        weaponQuickSlot = GameObject.Find("WeaponSlot");

        itemInventory = itemQuickSlots.GetComponent<QuickslotManager>();
        weaponInventory = weaponQuickSlot.GetComponent<WeaponInventory>();

        if (!movementStateManager) movementStateManager = GetComponent<MovementStateManager>();
        if (!hpManager) hpManager = GetComponent<HpManager>();
        if (!attackManager) attackManager = GetComponent<AttackManager>();

        // 시작 시 주먹(RightHand 1번째 자식) 장착 가정
        equipWeaponGameobject = RightHand.GetChild(1).GetComponent<WeaponManager>();
        equipWeapon = weapons[9];
        colliderHand = RightHand.GetChild(1).GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (!pv.IsMine) return;

        // 입력
        sDown1 = Input.GetButtonDown("sDown1");
        sDown2 = Input.GetButtonDown("sDown2");
        sDown3 = Input.GetButtonDown("sDown3");
        sDown4 = Input.GetButtonDown("sDown4");
        sDown5 = Input.GetButtonDown("sDown5");
        gDown = Input.GetButtonDown("gDown"); // 버리기
        eDown = Input.GetButtonDown("eDown"); // 사용(알약)

        // 스왑/인벤토리 변화 감지 → Swap RPC 호출
        if ((sDown1 || weaponInventory.isWeaponAdded || weaponInventory.isCrafted) ||
            (sDown2 || sDown3 || sDown4 || sDown5 ||
             ((itemInventory.isItemAdded || itemInventory.isSlotChanged)
              && !weaponQuickSlot.GetComponentInChildren<SelectedSlot>().slotOutline.enabled)) ||
            gDown || eDown)
        {
            RpcSwap();
        }
    }

    // ----------------- Swap 메인 -----------------
    public void RpcSwap()
    {
        pv.RPC(nameof(Swap), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void Swap()
    {
        // 로컬 오너만 로직 실행(상태계산) / 결과는 별도 RPC로 동기화
        if (!pv.IsMine) return;

        // 공격 중에는 스왑 금지
        if (attackManager != null && attackManager.isAttack) return;

        // eDown: 알약 사용
        if (eDown)
        {
            if (equipWeapon != null &&
                equipWeapon.TryGetComponent<ItemData>(out var id) &&
                id.itemData.ItemType == 12)
            {
                RpcPill();
            }
        }

        // gDown: 아이템/무기 버리기
        if (gDown)
        {
            AbandonedItem();
            return;
        }

        // 무기 슬롯 스왑/습득
        if (sDown1 || weaponInventory.isWeaponAdded)
        {
            equipWeaponIndex = -1;

            if (weaponInventory.weaponSlot.item != null &&
                weaponQuickSlot.GetComponentInChildren<SelectedSlot>().slotOutline.enabled)
            {
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weaponInventory.weaponSlot.item.ItemType ==
                        weapons[i].GetComponent<ItemData>().itemData.ItemType)
                    {
                        equipWeaponIndex = i;
                        weaponInventory.isWeaponAdded = false;
                        break;
                    }
                }
            }

            if (equipWeaponIndex != -1 && itemInventory.isSlotChanged == false)
            {
                // 타입 판정 및 레이어/Bool 정리
                equipWeaponGameobject = weapons[equipWeaponIndex].GetComponent<WeaponManager>();

                if (equipWeaponGameobject.GetComponent<ItemData>().itemData.ItemType <= 10)
                {
                    colliderWeapon = equipWeaponGameobject.GetComponent<BoxCollider>();
                    if (equipWeaponGameobject.GetComponent<ItemData>().itemData.ItemType <= 3)
                    {
                        // 양손
                        if (movementStateManager.anim.GetLayerWeight(2) != 1)
                        {
                            if (!string.IsNullOrEmpty(Armed)) movementStateManager.anim.SetBool(Armed, false);
                            movementStateManager.anim.SetLayerWeight(1, 0);
                            Armed = "THW";
                            movementStateManager.anim.SetBool(Armed, true);
                            movementStateManager.anim.SetLayerWeight(2, 1);
                        }
                    }
                    else
                    {
                        // 한손
                        if (movementStateManager.anim.GetLayerWeight(1) != 1)
                        {
                            if (!string.IsNullOrEmpty(Armed)) movementStateManager.anim.SetBool(Armed, false);
                            movementStateManager.anim.SetLayerWeight(2, 0);
                            Armed = "OHW";
                            movementStateManager.anim.SetBool(Armed, true);
                            movementStateManager.anim.SetLayerWeight(1, 1);
                        }
                    }
                }

                // 네트워크로 실제 장착 반영
                RpcEquip(equipWeaponIndex);

                movementStateManager.anim.SetTrigger("doSwap");
                isSwap = true;
                Invoke(nameof(SwapOut), 0.3f);
            }
            else if (equipWeaponIndex == -1 && weaponInventory.isWeaponAdded == false)
            {
                // 빈 칸 → 주먹
                if (equipWeapon != weapons[9])
                {
                    RpcEquip(-1);
                    movementStateManager.anim.SetTrigger("doSwap");
                    isSwap = true;
                    Invoke(nameof(SwapOut), 0.3f);
                }

                // 무장 해제
                movementStateManager.anim.SetLayerWeight(1, 0);
                movementStateManager.anim.SetLayerWeight(2, 0);
                movementStateManager.anim.SetBool("OHW", false);
                movementStateManager.anim.SetBool("THW", false);
                Armed = "";
            }
        }
        // 아이템 슬롯 스왑/습득
        else if (sDown2 || sDown3 || sDown4 || sDown5 ||
                 ((itemInventory.isItemAdded || itemInventory.isSlotChanged)
                  && !weaponQuickSlot.GetComponentInChildren<SelectedSlot>().slotOutline.enabled))
        {
            equipItemIndex = -1;
            bool isFound = false;

            for (int i = 0; i < 4; i++)
            {
                if (itemQuickSlots.GetComponentsInChildren<SelectedSlot>()[i].slotOutline.enabled)
                {
                    if (itemQuickSlots.GetComponent<QuickslotManager>().slots[i].item != null)
                    {
                        for (int j = weapons.Length - 1; j >= 0; j--)
                        {
                            if (itemQuickSlots.GetComponent<QuickslotManager>().slots[i].item.ItemType ==
                                weapons[j].GetComponent<ItemData>().itemData.ItemType)
                            {
                                equipItemIndex = j;
                                itemInventory.isItemAdded = false;
                                isFound = true;
                                break;
                            }
                        }
                        if (isFound) break;
                    }
                }
            }
            itemInventory.isSlotChanged = false;

            if (equipItemIndex != -1)
            {
                equipWeaponGameobject = weapons[equipItemIndex].GetComponent<WeaponManager>();

                if (equipWeaponGameobject.GetComponent<ItemData>().itemData.ItemType > 10)
                {
                    colliderWeapon = equipWeaponGameobject.GetComponent<BoxCollider>();
                    if (movementStateManager.anim.GetLayerWeight(1) != 1)
                    {
                        if (!string.IsNullOrEmpty(Armed)) movementStateManager.anim.SetBool(Armed, false);
                        movementStateManager.anim.SetLayerWeight(2, 0);
                        Armed = "OHW";
                        movementStateManager.anim.SetBool(Armed, true);
                        movementStateManager.anim.SetLayerWeight(1, 1);
                    }
                }

                RpcEquip(equipItemIndex);

                movementStateManager.anim.SetTrigger("doSwap");
                isSwap = true;
                Invoke(nameof(SwapOut), 0.3f);
            }
            else if (equipItemIndex == -1)
            {
                // 빈 칸 → 주먹
                equipWeaponGameobject = weapons[9].GetComponent<WeaponManager>();
                if (equipWeapon != weapons[9])
                {
                    colliderWeapon = equipWeaponGameobject.GetComponent<BoxCollider>();
                    RpcEquip(9);
                    movementStateManager.anim.SetTrigger("doSwap");
                    isSwap = true;
                    equipWeapon = weapons[9];
                    Invoke(nameof(SwapOut), 0.3f);
                }
                movementStateManager.anim.SetLayerWeight(1, 0);
                movementStateManager.anim.SetLayerWeight(2, 0);
                movementStateManager.anim.SetBool("OHW", false);
                movementStateManager.anim.SetBool("THW", false);
                Armed = "";
            }
        }

        // 크래프팅 반영(번개 + 데미지 배율)
        if (weaponInventory.isWeaponAdded || weaponInventory.isCrafted)
        {
            if (equipWeapon != weapons[9])
            {
                if (equipWeapon.transform.childCount > 0 &&
                    equipWeapon.GetComponent<ItemData>().itemData.ItemType != 12)
                {
                    if (weaponInventory.weaponSlot.item != null &&
                        weaponInventory.weaponSlot.item.craftCompleted)
                    {
                        OnLightening(true);
                        equipWeaponGameobject.damage *= 2;
                    }
                    else
                    {
                        OnLightening(false);
                    }
                    weaponInventory.isCrafted = false;
                }
            }
        }
    }

    void SwapOut()
    {
        isSwap = false;
        movementStateManager.anim.SetTrigger("SwapOut");
    }

    // ----------------- 버리기 -----------------
    void AbandonedItem()
    {
        pv.RPC(nameof(RpcAbandonedItem), RpcTarget.All);
    }

    [PunRPC]
    void RpcAbandonedItem()
    {
        if (!pv.IsMine) return;

        if (weaponQuickSlot.GetComponentInChildren<SelectedSlot>().slotOutline.enabled)
        {
            // 무기 버리기
            if (equipWeapon != weapons[9])
            {
                equipWeapon.transform.GetChild(0).gameObject.SetActive(false);
                weaponInventory.abandonedItem = weaponInventory.weaponSlot.item;

                if (weaponInventory.abandonedItem != null &&
                    weaponInventory.abandonedItem.craftCompleted)
                {
                    equipWeaponGameobject.damage /= 2;
                }

                weaponInventory.weaponSlot.item = null;
                weaponInventory.craftCompletedMark.SetActive(false);

                RpcEquip(9);
                equipWeapon = weapons[9];

                movementStateManager.anim.SetTrigger("doSwap");
                isSwap = true;
                Invoke(nameof(SwapOut), 0.3f);

                if (!string.IsNullOrEmpty(Armed)) movementStateManager.anim.SetBool(Armed, false);
                movementStateManager.anim.SetLayerWeight(1, 0);
                movementStateManager.anim.SetLayerWeight(2, 0);
                Armed = "";
            }
        }
        else
        {
            // 아이템 버리기
            for (int i = 0; i < 4; i++)
            {
                if (itemQuickSlots.GetComponentsInChildren<SelectedSlot>()[i].slotOutline.enabled)
                {
                    if (equipWeapon != weapons[9])
                    {
                        weaponInventory.abandonedItem =
                            itemQuickSlots.GetComponent<QuickslotManager>().slots[i].item;

                        itemQuickSlots.GetComponent<QuickslotManager>().slots[i].item = null;
                        itemQuickSlots.GetComponent<QuickslotManager>().FreshSlot();

                        RpcEquip(9);
                        equipWeapon = weapons[9];

                        movementStateManager.anim.SetTrigger("doSwap");
                        isSwap = true;
                        Invoke(nameof(SwapOut), 0.3f);

                        if (!string.IsNullOrEmpty(Armed)) movementStateManager.anim.SetBool(Armed, false);
                        movementStateManager.anim.SetLayerWeight(1, 0);
                        movementStateManager.anim.SetLayerWeight(2, 0);
                        Armed = "";
                        break;
                    }
                }
            }
        }
    }

    // ----------------- 알약 사용 -----------------
    public void RpcPill()
    {
        pv.RPC(nameof(RpcTakingPill), RpcTarget.All);
    }

    [PunRPC]
    void RpcTakingPill()
    {
        movementStateManager.anim.SetLayerWeight(8, 1);
        movementStateManager.anim.SetTrigger("Pill");
        // 애니 이벤트에서 movementStateManager.pillTaked = true 로 전환되었다고 가정
        Invoke(nameof(PillTaked), 0.35f); // 필요 시 타이밍 조정
    }

    void PillTaked()
    {
        if (!movementStateManager.pillTaked) return;

        for (int i = 0; i < 4; i++)
        {
            if (itemQuickSlots.GetComponentsInChildren<SelectedSlot>()[i].slotOutline.enabled)
            {
                var slot = itemQuickSlots.GetComponent<QuickslotManager>().slots[i];
                if (slot.item == null) break;

                hpManager.OnRecovery(slot.item.ItemRecovery);
                slot.item = null;
                itemQuickSlots.GetComponent<QuickslotManager>().FreshSlot();

                RpcEquip(9);
                equipWeapon = weapons[9];
                isSwap = true;

                if (!string.IsNullOrEmpty(Armed)) movementStateManager.anim.SetBool(Armed, false);
                movementStateManager.anim.SetLayerWeight(1, 0);
                movementStateManager.anim.SetLayerWeight(2, 0);
                Armed = "";
                movementStateManager.pillTaked = false;
                break;
            }
        }
    }

    // ----------------- 번개 이펙트 -----------------
    public void OnLightening(bool isLightening)
    {
        pv.RPC(nameof(RPCOnLightening), RpcTarget.All, isLightening);
    }

    [PunRPC]
    void RPCOnLightening(bool isLightening)
    {
        if (equipWeapon != null)
        {
            if (isLightening)
                equipWeapon.transform.GetChild(0).gameObject.SetActive(true);
            else
                equipWeapon.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // ----------------- 장착 적용 -----------------
    public void RpcEquip(int index)
    {
        pv.RPC(nameof(RPCWeaponEquip), RpcTarget.All, index);
    }

    [PunRPC]
    void RPCWeaponEquip(int RpcEquipWeaponIndex)
    {
        if (RpcEquipWeaponIndex == -1)
        {
            if (equipWeapon != weapons[9])
                equipWeapon.SetActive(false);
            equipWeapon = weapons[9];
            equipWeapon.SetActive(true);
            equipWeaponGameobject = equipWeapon.GetComponent<WeaponManager>();
            colliderWeapon = equipWeapon.GetComponent<BoxCollider>();
            return;
        }

        if (equipWeapon != weapons[9]) equipWeapon.SetActive(false);

        equipWeapon = weapons[RpcEquipWeaponIndex];
        equipWeapon.SetActive(true);
        equipWeaponGameobject = equipWeapon.GetComponent<WeaponManager>();
        colliderWeapon = equipWeapon.GetComponent<BoxCollider>();
    }
}
