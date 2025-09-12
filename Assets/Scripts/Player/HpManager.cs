using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

public class HpManager : MonoBehaviour
{
    public float maxHp { get; set; } = 100;

    private float _hp;
    public float hp
    {
        get { return _hp; }
        set
        {
            _hp = Mathf.Min(value, maxHp);
            if (_hp <= 0)
            {
                _hp = Mathf.Max(_hp, 0);
                Die();
            }
            else OnHpChanged(_hp, maxHp);
        }
    }

    public AttackManager attackManager;
    public GameObject DroppedItem;

    [SerializeField] private UIManager uiManager;
    private MovementStateManager movementStateManager;

    public event Action OnDeath;
    public event Action OnDamaged;
    public event Action OnRecoverd;

    public delegate void OnHpChangedEvent(float hp, float maxhp);
    public event OnHpChangedEvent OnHpChanged;

    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        hp = maxHp;
        movementStateManager = GetComponent<MovementStateManager>();
    }

    void Start()
    {
        if(pv.IsMine) //local이 UI와 상호작용하기 위한 리스너 등록
        {
            var HpListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IHpListener>();
            foreach (var listener in HpListeners) OnHpChanged += listener.OnHpChanged;

            var DeathListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDeathListener>();
            foreach (var listener in DeathListeners) OnDeath += listener.OnDeath;
        }
        OnDamaged += movementStateManager.OnDamaged; //애니메이션이랑 사운드는 나를 포함한 모두
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (GameManager.Instance.isEscape == true)
            {
                Escape();
                Debug.Log("탈출 성공공");
            }
        }
    }

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
        if (pv.IsMine && GameManager.Instance.UserId != playerId)
        {
            
            Debug.Log("데미지 입음");
            Debug.Log("내 이름: " + GameManager.Instance.UserId);
            Debug.Log("나를 때린 사람 이름: " + playerId);

            Debug.Log("받은 데미지: " + damage);
            OnDamaged.Invoke();
            hp -= damage;
            Debug.Log("남은 hp: " + hp);

            // 체력이 0 이하이고 살아있으면 사망
            if (hp <= 0)
            {
                hp = 0;
                Debug.Log("나를 죽인 사람: " + playerId);
                AddKillCount(playerId);
            }
        }
    }
    public void OnDamage(float damage, string playerId)
    {
        //Debug.Log("OnDamage는 실행됨");
        pv.RPC("RpcOnDamage", RpcTarget.Others, damage, playerId);
    }


    [PunRPC]
    public void RpcRecover(float recovery)
    {
        if (hp > 0) hp += recovery;
    }
    /// <summary>
    /// 회복함수
    /// </summary>
    public void Recover(float recovery) => pv.RPC("RpcRecover", RpcTarget.All, recovery);


    [PunRPC]
    public void RpcDie()
    {
        if (pv.IsMine) OnDeath?.Invoke(); //local은 ui실행
        else //객체는 플레이어 수 줄게하고 꺼주기
        {
            GameManager.Instance.curPlayers -= 1;
            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 사망 함수
    /// </summary>
    public void Die() => pv.RPC("RpcDie", RpcTarget.All);

    [PunRPC]
    public void RpcEscape()
    {
        if (pv.IsMine) OnDeath?.Invoke(); //내가 죽진 않고 게임종료 UI만 사용할 것

        for (int i = 0; i < GameManager.Instance.playerObjects.Length; i++)
        {
            HpManager hpManager = GameManager.Instance.playerObjects[i].GetComponent<HpManager>();
            hpManager.hp = 0;
        }
    }
    /// <summary>
    /// 탈출 시 나 빼고 다죽는 함수
    /// </summary>
    public void Escape() => pv.RPC("RpcAllDie", RpcTarget.All);
}
