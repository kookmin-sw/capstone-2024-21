using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    [HideInInspector] public AttackManager attackManager;

    public KillManager killManager;

    public void Use(){
        if(type == Type.Melee){
            StartCoroutine("Swing");
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            HpManager hpManager = other.GetComponent<HpManager>();
            AttackManager Enemy = other.GetComponent<AttackManager>();
            attackManager = GetComponent<AttackManager>();
            if (Enemy != null) {
                Enemy.OnDamaged();

                Debug.Log("Hit : " + damage);
                hpManager.OnDamage(damage);
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

