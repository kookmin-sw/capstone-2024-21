using Photon.Pun;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [Header("Refs")]
    public EquipmentController equipment;
    public MovementStateManager movementStateManager;

    [Header("Attack")]
    [SerializeField] float fireDelay;
    [SerializeField] public bool isAttack;   // 공격 중(애니 이벤트로 In/Out)
    [SerializeField] bool isFireReady;

    PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (!equipment) equipment = GetComponent<EquipmentController>();
        if (!movementStateManager) movementStateManager = GetComponent<MovementStateManager>();
    }

    void Update()
    {
        if (!pv.IsMine) return;
        Attack();
    }

    public void Attack()
    {
        fireDelay += Time.deltaTime;

        var wm = equipment?.equipWeaponGameobject;
        if (!wm) return;

        isFireReady = wm.rate < fireDelay;

        // LMB 눌림 + 공격 중 아님 + 쿨다운 OK
        if (Input.GetMouseButton(0) && !isAttack && isFireReady)
        {
            wm.Use(); // 무기 사용(데미지 처리/이펙트 등은 무기 쪽에서)
            pv.RPC(nameof(AttackTrig), RpcTarget.All);
            fireDelay = 0;
        }
    }

    [PunRPC]
    void AttackTrig()
    {
        movementStateManager.anim.SetTrigger("AttackTrig");
    }

    // 애니메이션 이벤트에서 호출
    public void AttackIn() { isAttack = true; }
    public void AttackOut() { isAttack = false; }

    // 피격 반응(외부에서 호출)
    public void OnDamaged()
    {
        if (pv.IsMine)
            pv.RPC(nameof(RpcHit), RpcTarget.All);
    }

    [PunRPC]
    void RpcHit()
    {
        movementStateManager.anim.SetTrigger("Hit");
    }
}
