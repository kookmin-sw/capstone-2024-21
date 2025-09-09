using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Timer : MonoBehaviour,IGameStateListener
{
    private PhotonView pv;

    public int startTime = 10;
    [SerializeField] private TextMeshProUGUI countDownTMP;

    public delegate void OnZeroEvent();
    public static event OnZeroEvent OnZero;

    public void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Ready:
                StartCountDown();
                break;
        }
    }

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        countDownTMP = GetComponent<TextMeshProUGUI>();
    }

    public void StartCountDown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC(nameof(CountDown), RpcTarget.All, startTime);
        }
    }

    [PunRPC]
    void CountDown(int startTime)
    {
        StartCoroutine(ShowLoop(startTime));
    }
    IEnumerator ShowLoop(int startTime)
    {
        int time = startTime;
        while (time > 0)
        {
            countDownTMP.SetText($"{time}");
            yield return new WaitForSeconds(1f);
            --time;
        }
        gameObject.SetActive(false);
        OnZero?.Invoke();
    }


}
