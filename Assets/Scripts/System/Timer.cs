using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Timer : MonoBehaviour
{
    int time;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void StartTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            time = 60;

            StartCoroutine(TimerCoroution());
        }
    }

    IEnumerator TimerCoroution()
    {
        if (time > 0)
        {
            time -= 1;
        }
        else
        {
            Debug.Log("타이머 종료");
            yield break;
        }

        pv.RPC("ShowTimer", RpcTarget.All, time); //1초 마다 방 모두에게 전달

        yield return new WaitForSeconds(1);
        StartCoroutine(TimerCoroution());
    }

    [PunRPC]
    void ShowTimer(int time)
    {
        Debug.Log(time);
    }
}
