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
    // Start is called before the first frame update
    void Awake()
    {
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
}
