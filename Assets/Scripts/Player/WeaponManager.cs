using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponManager : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    [SerializeField] public float rate = 1.0f;
    public float colliderOn;
    public float colliderOff;
    [SerializeField] public BoxCollider meleeArea;
    [HideInInspector] public AttackManager attackManager;

    public KillManager killManager;

    [SerializeField] private bool chk = true;
    private Coroutine swingCoroutine; // 코루틴 참조를 저장하기 위한 변수

    private void Start()
    {
        attackManager = transform.GetComponentInParent<AttackManager>(); 
    }

    public void callAttack(){
        if(swingCoroutine != null)
        {
            StopCoroutine(swingCoroutine);
        }
        if(type == Type.Melee)
        {
            attackManager = transform.GetComponentInParent<AttackManager>();
        }
    }

    public void Use(){
        if(type == Type.Melee){
            StartSwing();
        }
    }

    void StartSwing(){
        if (chk == true){ // 이미 코루틴이 실행 중인지 확인
            chk = false;
            swingCoroutine = StartCoroutine(Swing());
        }
    }

    void OnTriggerEnter(Collider other){
        if((other.gameObject.tag == "Monster" || other.gameObject.tag == "Player") && meleeArea != null && meleeArea.enabled){
            meleeArea.enabled = false;
            HpManager hpManager = other.GetComponent<HpManager>();
            PhotonView pv = other.GetComponent<PhotonView>();

            if (hpManager != null && GameManager.Instance.isPlaying) 
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.SFX_temphit);

                if (other.gameObject.tag == "Monster")
                {
                    Debug.Log("Hit : " + damage);
                    hpManager.OnDamage(damage, killManager.playerId);
                }
                else
                {
                    if (pv.Owner.NickName != GameManager.Instance.UserId)
                    {
                        Debug.Log("Hit : " + damage);
                        hpManager.OnDamage(damage, killManager.playerId);
                    }
                }
            }
        }
    }

    IEnumerator Swing(){
        attackManager.AttackIn();
        yield return new WaitForSeconds(colliderOn);
        if (meleeArea != null)
        {
            meleeArea.enabled = true;
        }
        yield return new WaitForSeconds(colliderOff);
        if (meleeArea != null)
        {
            meleeArea.enabled = false;
        }
        yield return new WaitForSeconds(rate - colliderOn - colliderOff);
        chk = true; // 코루틴 종료 후 변수 초기화
        attackManager.AttackOut();
    }
}
