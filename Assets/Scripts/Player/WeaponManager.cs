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
    [SerializeField] public BoxCollider meleeArea;
    [HideInInspector] public AttackManager attackManager;

    public KillManager killManager;

    public void Use(){
        if(type == Type.Melee){
            StartCoroutine("Swing");
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            meleeArea.enabled = false;
            HpManager hpManager = other.GetComponent<HpManager>();
            PhotonView pv = other.GetComponent<PhotonView>();
            // AttackManager Enemy = other.GetComponent<AttackManager>();
            attackManager = GetComponent<AttackManager>();

            Debug.Log("공격");
            Debug.Log("내 이름: " + GameManager.Instance.UserId);
            Debug.Log("상대 이름: " + pv.Owner.NickName);

            if (hpManager != null && pv.Owner.NickName != GameManager.Instance.UserId) {
                // Enemy.OnDamaged();
                
                Debug.Log("Hit : " + damage);
                hpManager.OnDamage(damage, killManager.playerId);
            }    
        }
    }

    IEnumerator Swing(){
        while(meleeArea.enabled)
            yield return null;

        yield return new WaitForSeconds(0.5f); //  15프레임
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.83f); //  25프레임
        meleeArea.enabled = false;
    }
}

