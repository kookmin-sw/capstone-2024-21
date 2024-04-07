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
    public TrailRenderer trailEffect;
    [HideInInspector] public AttackManager attackManager;

    // public void Use(){
    //     if(type == Type.Melee){
    //         StopCoroutine("Swing");
    //         StartCoroutine("Swing");
    //     }
    // }

    void OnTriggerEnter(Collider other){
        Debug.Log("collision");
        Debug.Log(other.gameObject.name);
        if(other.gameObject.tag == "Player"){
            HpManager hpManager = other.GetComponent<HpManager>();
            AttackManager Enemy = other.GetComponent<AttackManager>();
            attackManager = GetComponent<AttackManager>();

            if (Enemy != null) {
                attackManager.AttackEnd();
                Enemy.OnDamaged();
            }

            Debug.Log("Hit : " + damage);
            hpManager.OnDamage(damage);
        }
    }

    IEnumerator Swing(){
        yield return null;
        yield return new WaitForSeconds(0.1f);
        
    }
}
