using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Timer : MonoBehaviour
{
    public int time;
    private PhotonView pv;
    UIManager uiManager;

    private void Awake()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        pv = GetComponent<PhotonView>();
        GameManager.Instance.timer = this;
    }

    public void StartTimer(int timerTime)
    {
        time = timerTime;
        StartCoroutine(TimerCoroution());
    }

    IEnumerator TimerCoroution()
    {
        if (time > 0)
        {
            time -= 1;
        }
        else
        {
            pv.RPC("GameStart", RpcTarget.All);
            yield break;
        }

        pv.RPC("ShowTimer", RpcTarget.All, time); //1초 마다 방 모두에게 전달

        yield return new WaitForSeconds(1);
        StartCoroutine(TimerCoroution());
    }

    [PunRPC]
    void ShowTimer(int time)
    {

        if (time != 0)
        {
            uiManager.countDownNum.text = time.ToString();
        }
        
        Debug.Log(time);
    }

    [PunRPC]
    void GameStart()
    {
        Debug.Log("타이머 종료");
        GameManager.Instance.GameStart();
    }
}
