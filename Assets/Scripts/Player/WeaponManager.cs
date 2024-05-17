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

    public void callAttack(){
        StopCoroutine("Swing");
        if(type == Type.Melee)
            attackManager = transform.GetParentComponent<AttackManager>(); 
    }

    public void Use(){
        if(type == Type.Melee){
            StartSwing();
        }
    }

    void StartSwing(){
        if (chk == true){ // 이미 코루틴이 실행 중인지 확인
            Debug.Log(chk);
            chk = false;
            Debug.Log("스타트스윙");
            Debug.Log(chk);
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else Debug.Log("실패");
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player" && meleeArea.enabled){
            meleeArea.enabled = false;
            HpManager hpManager = other.GetComponent<HpManager>();
            PhotonView pv = other.GetComponent<PhotonView>();

            if (hpManager != null && pv.Owner.NickName != GameManager.Instance.UserId) {
                Debug.Log("Hit : " + damage);
                hpManager.OnDamage(damage, killManager.playerId);
            }    
        }
    }

    IEnumerator Swing(){
        Debug.Log("스윙");
        Debug.Log(chk);
        attackManager.AttackIn();
        yield return new WaitForSeconds(colliderOn);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(colliderOff);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(1.0f - colliderOn - colliderOff);
        chk = true; // 코루틴 종료 후 변수 초기화
        attackManager.AttackOut();
        Debug.Log("스윙끝");
        Debug.Log(chk);
    }
}
