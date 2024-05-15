using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Player : MonoBehaviour
{
    [SerializeField]
    Transform[] points;
    [SerializeField] UIManager uiManager;
    private PhotonView pv;

    int time;

    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Go2Map();
        }
    }

    void GameStart()
    {
        Go2Map();
    }

    void Go2Map()
    {
        uiManager.isGameStart = true;
        uiManager.totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        uiManager.curPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Transform[] points = GameObject.Find("WarpPointGroup").GetComponentsInChildren<Transform>();
        if (points != null)
        {
            int idx = Random.Range(1, points.Length);
            this.transform.position = points[idx].position;
        }
        else
        {
            Debug.Log("points가 왜 null임??");
        }
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
