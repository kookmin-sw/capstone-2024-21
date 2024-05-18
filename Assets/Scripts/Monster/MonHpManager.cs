using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;



public class MonHpManager : MonoBehaviour
{
    public float maxHp { get; set; } = 50;
    public float hp;
    public bool isDead { get; set; } // 죽었는지 확인
    public GameObject DroppedItem;


    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    // 캐릭터 생성, 부활 등등 활성화 될 때 실행되는 코드
    void OnEnable()
    {   
        hp = maxHp;
    }

    private void Update()
    {
        if(pv.IsMine)
        {
            if (hp <= 0)
            {
                MonsterDie();
            }
        }
    }


    // 데미지 처리하는 함수
    [PunRPC]
    public void RpcOnDamage(float damage, string playerId)
    {
        if (pv.IsMine){

            Debug.Log("받은 데미지: " + damage);
            hp -= damage;
            Debug.Log("남은 hp: " + hp);

            // 체력이 0 이하이고 살아있으면 사망
            if (hp <= 0)
            {
                Debug.Log("나를 죽인 사람: " + playerId);
                MonsterDie();
            }
        }
    }
    public void OnDamage(float damage, string playerId)
    {
        Debug.Log("OnDamage는 실행됨");
        pv.RPC("RpcOnDamage", RpcTarget.Others, damage, playerId);
    }



    [PunRPC]
    public void RpcMonsterDie()
    {
        if (pv.IsMine)
        {
            DroppedItem = Instantiate(Resources.Load<GameObject>("Prefabs/battery")); //프리펩 생성
            DroppedItem.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            Destroy(gameObject);
        }
    }

    // 사망 함수
    public void MonsterDie()
    {
        pv.RPC("RpcDie", RpcTarget.All);
    }
}