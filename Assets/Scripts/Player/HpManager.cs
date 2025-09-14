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
            _hp = Mathf.Clamp(value, 0f, maxHp);
            if (_hp <= 0) Die();
            else OnHpChanged?.Invoke(_hp, maxHp);
        }
    }

    private MovementStateManager movementStateManager;
    private KillManager killManager;

    public event Action OnDeath;
    public event Action OnDamaged;

    public delegate void OnHpChangedEvent(float hp, float maxhp);
    public event OnHpChangedEvent OnHpChanged;

    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        movementStateManager = GetComponent<MovementStateManager>();
        killManager = GetComponent<KillManager>();
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
        hp = maxHp;
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

    [PunRPC]
    public void RPCAddKillCount()
    {
        if (pv.IsMine)
        {
            killManager.killCount++;
        }
    }

    // 데미지 처리하는 함수
    [PunRPC]
    public void RpcOnDamage(float damage, int attakActorNum)
    {

        if (pv.IsMine)
        {
            hp -= damage;
            OnDamaged.Invoke();

            //날 죽인 사람의 킬카운트 높이기
            if (hp <= 0)
            {
                KillManager attakerKM = GameManager.Instance.ActorDict[attakActorNum].GetComponent<KillManager>();
                attakerKM.AddKillCount();
            }
        }
    }
    /// <summary>
    /// 때린 쪽에서 가져가서 데미지와 자기 정보를 넘겨주는 함수
    /// </summary>
    /// <param name="damage"></param>
    public void OnDamage(float damage, int attakActorNum)
    {
        pv.RPC(nameof(RpcOnDamage), RpcTarget.All, damage, attakActorNum);
    }


    [PunRPC]
    public void RpcRecover(float recovery)
    {
        if (hp > 0) hp += recovery;
    }
    /// <summary>
    /// 회복함수
    /// </summary>
    public void Recover(float recovery) => pv.RPC(nameof(RpcRecover), RpcTarget.AllViaServer, recovery);


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
    public void Die() => pv.RPC(nameof(RpcDie), RpcTarget.AllViaServer);

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
    public void Escape() => pv.RPC(nameof(RpcEscape), RpcTarget.AllViaServer);
}
