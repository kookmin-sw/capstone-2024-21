using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponManager : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public float Oncollider;
    public float Offcollider;
    [SerializeField] public BoxCollider meleeArea;
    [HideInInspector] public AttackManager attackManager;

    public KillManager killManager;

    private Coroutine swingCoroutine; // 코루틴 참조를 저장하기 위한 변수

    public void Use(){
        if(type == Type.Melee){
            StartSwing();
        }
    }

    void StartSwing(){
        if (swingCoroutine == null) // 이미 코루틴이 실행 중인지 확인
            swingCoroutine = StartCoroutine(Swing());
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
        yield return new WaitForSeconds(Oncollider);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(Offcollider); // 15프레임
        meleeArea.enabled = false;
        swingCoroutine = null; // 코루틴 종료 후 변수 초기화
    }
}
