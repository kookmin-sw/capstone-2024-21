using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class MovementStateManager : MonoBehaviour
{
    [SerializeField] Interact interact;
    [SerializeField] bool isExiting = false;
    [SerializeField] bool wasExiting = false;

    [HideInInspector] public float xAxis; // 좌, 우
    [HideInInspector] public float zAxis; // 앞, 뒤

    [HideInInspector] public Vector3 moveDir; 
    public float currentMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2; 
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 1, crouchBackSpeed = 1;
    public float crouchFastSpeed = 2, crouchFastBackSpeed = 2;
    public float jumpPower = 0;
    CharacterController controller;

    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;
    Vector3 itemPos;


    [SerializeField] float jumpForce = 3;
    [SerializeField] float gravity = -9.81f;
    public bool jumped;
    public bool pillTaked;
    public bool isJumpStart;
    Vector3 velocity;

    //
    public GameObject DroppedItem;
    
    ///////Attack
    float fireDelay;
    bool fDown;
    bool isFireReady;
    ////// Attack

    // FlashLight
    public GameObject spotLightObject;
    public Light lightComponent;

    public MovementBaseState previousState;
    public MovementBaseState currentState;
    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();
    public JumpState Jump = new JumpState();

    [HideInInspector] public Animator anim;
    [HideInInspector] public AttackManager attackManager;
    [HideInInspector] public StaminaManager staminaManager;
    public KillManager killManager;

    private UIManager uiManager;

    private PhotonView pv;

    void Start()
    {
        interact = GameObject.Find("Virtual Camera").GetComponent<Interact>();

        pv = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        attackManager = GetComponent<AttackManager>();
        staminaManager = GetComponent<StaminaManager>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        // FlashLight
        spotLightObject = transform.Find("CameraFollowPos").gameObject;
        lightComponent = spotLightObject.GetComponentInChildren<Light>();

        SwitchState(Idle);
    }

    void Update()
    {
        if (pv.IsMine)
        {
            if (attackManager.weaponInventory.abandonedItem != null) //버릴 무기가 있으면
            {
                DroppedItem = Instantiate(attackManager.weaponInventory.abandonedItem.itemPrefab); //프리펩 생성
                if(attackManager.weaponInventory.abandonedItem.ItemType < 11)
                {
                    if (attackManager.weaponInventory.abandonedItem.craftCompleted == true)
                    {
                        DroppedItem.GetComponent<Weapon>().settedLightning = true;
                    }
                    else if (attackManager.weaponInventory.abandonedItem.craftCompleted == false)
                    {
                        DroppedItem.GetComponent<Weapon>().settedLightning = false;
                    }
                }

                DroppedItem.transform.position = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
                attackManager.weaponInventory.abandonedItem = null;
            }
            if (uiManager.isGameOver == false && attackManager.isAttack == false)
            {
                uiManager.SelectQuickSlot();
            }
            GetDirectionAndMove();
            Gravity();

            anim.SetFloat("xAxis", xAxis);
            anim.SetFloat("zAxis", zAxis);

            currentState.UpdateState(this);
            if(uiManager.isUIActivate == false && uiManager.isComActivate == false)
            {
                attackManager.Attack();
            }

            ExitState();
        }
    }

    void ExitState(){   
        if (!wasExiting && interact.isExiting && moveDir.magnitude < 0.1f) 
            {   
                anim.SetLayerWeight(7,1);
                wasExiting = true; // 한 번 실행된 후 다시 false로 설정될 때까지 실행되지 않도록 설정
                Debug.Log(wasExiting);
                anim.SetTrigger("Exiting");
                
            }
        else if(wasExiting && (moveDir.magnitude > 0.1f || !interact.isExiting)){
                anim.SetTrigger("Cancel");
                anim.SetLayerWeight(7,0);
                wasExiting = false;
                Debug.Log(wasExiting);
            }
        isExiting = interact.isExiting;
    }


    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    void GetDirectionAndMove()
    {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");

        moveDir = (transform.forward * zAxis + transform.right * xAxis).normalized;

        controller.Move(moveDir * currentMoveSpeed * Time.deltaTime);
    }

    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePos, controller.radius + 0.05f, groundMask)) {
            return true;
        }
        return false;
    }

    
    void Gravity()
    {
        if (!IsGrounded()) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2;

        controller.Move(velocity * Time.deltaTime);
    }

    public void JumpForce(){
        velocity.y += jumpForce;
        isJumpStart = true;
    } 

    public void OpenExitDoor(){
        anim.SetLayerWeight(7,1);
        anim.SetTrigger("Working");
    }
    public void EndExitDoor(){
        anim.SetLayerWeight(7,1);
    }

    public void Jumped() => jumped = true;

    public void PillTaked() => pillTaked = true;

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
    // }

}