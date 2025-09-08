using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Timer : MonoBehaviour, IUIStateListener
{
    private PhotonView pv;

    public int time;
    TextMeshProUGUI countDownTMP;

    public delegate void OnZeroEvent();
    public static event OnZeroEvent OnZero;

    public void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Ready:
                StartCoroutine(CountDown());
                break;
        }
    }

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        countDownTMP = GetComponent<TextMeshProUGUI>();
    }

    IEnumerator CountDown()
    {
        time = 10;
        while(time > 0)
        {
            pv.RPC("ShowTimer", RpcTarget.All, time);
            yield return new WaitForSeconds(1f);
            --time;
        }
        OnZero?.Invoke();
    }

    [PunRPC]
    void ShowTimer(int time)
    {
        if (time != 0)
        {
            countDownTMP.SetText($"{time}");
        }
    }
}
