using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;



public class HpManager : MonoBehaviour
{
    public float maxHp { get; set; } = 100;
    public float hp { get; set; }
    public bool isDead { get; set; } // 죽었는지 확인

    [SerializeField] private Slider healthPointBar;
    [SerializeField] private TMP_Text healthPointCount;
    [SerializeField] private UIManager uiManager;

    // 죽었을 때 작동할 함수들을 저장하는 변수
    // onDeath += 함수이름; 이렇게 이벤트 등록 가능
    // 함수 이름에 () 안붙여야함
    public event Action onDeath;

    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        uiManager = FindObjectOfType<UIManager>();
        healthPointBar = GameObject.Find("HealthPointBar").GetComponent<Slider>();
        healthPointCount = GameObject.Find("HealthPointCount").GetComponent<TextMeshProUGUI>();
    }

    // 캐릭터 생성, 부활 등등 활성화 될 때 실행되는 코드
    void OnEnable()
    {   
        hp = maxHp;
        healthPointBar.value = hp;
        healthPointCount.text = hp.ToString();
        isDead = false;
    }

    // [PunRPC]
    // public void ApplyUpdatedHp(float newHp, bool newIsDead)
    // {
    //     hp = newHp;
    //     isDead = newIsDead;
    // }

    public void AddKillCount(string playerId)
    {
        GameObject obj = GameObject.Find(playerId);
        KillManager killer = obj.GetComponent<KillManager>();
        killer.AddKillCount();
    }

    // 데미지 처리하는 함수
    [PunRPC]
    public void RpcOnDamage(float damage, string playerId)
    {
        Debug.Log("RpcOnDamage는 실행됨");
        if (pv.IsMine){
            Debug.Log("pv.isMine은 실행됨");
            Debug.Log("데미지 입음");
            Debug.Log("받은 데미지: " + damage);
            hp -= damage;
            healthPointBar.value = hp;
            healthPointCount.text = hp.ToString();
            Debug.Log("남은 hp: " + hp);

            // pv.RPC("ApplyUpdatedHp", RpcTarget.Others, hp, isDead);

            // 체력이 0 이하이고 살아있으면 사망
            if (hp <= 0 && !isDead)
            {
                Debug.Log("나를 죽인 사람: " + playerId);
                AddKillCount(playerId);
                Die();
            }
        }
    }
    public void OnDamage(float damage, string playerId)
    {
        Debug.Log("OnDamage는 실행됨");
        pv.RPC("RpcOnDamage", RpcTarget.Others, damage, playerId);
    }

    // 체력 회복 함수
    public void OnRecovery(float recovery)
    {
        // 죽었으면 회복 x
        if (!isDead)
        {
            hp += recovery;
            healthPointBar.value = hp;
            healthPointCount.text = hp.ToString();
            if (hp > maxHp)
            {
                hp = maxHp;
            }
        }
    }

    [PunRPC]
    public void RpcDie()
    {
        // 사망 이벤트 있으면 실행
        if (onDeath != null)
        {
            onDeath();
        }
        isDead = true;
        gameObject.SetActive(false);
        if (pv.IsMine)
        {
            Debug.Log("사망");
            uiManager.isGameOver = true;
            uiManager.isUIActivate = true;
        }
    }

    // 사망 함수
    public void Die()
    {
        pv.RPC("RpcDie", RpcTarget.All);
    }
}