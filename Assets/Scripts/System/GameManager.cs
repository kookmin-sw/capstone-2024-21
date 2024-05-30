using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<GameManager>();
                if (!_instance)
                {
                    GameObject obj = new GameObject();
                    obj.name = "GameManager";
                    _instance = obj.AddComponent(typeof(GameManager)) as GameManager;
                }
            }
            return _instance;
        }
    }

    public string UserId { get; set; } = "soldier";
    public bool isPlaying { get; set; } = false;
    public bool isEscape { get; set; } = false;

    public UIManager uiManager;

    public Interact interact;

    public Timer timer;
    public GameObject[] playerObjects;
    Player[] players;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void TimerStart()
    {
        timer.StartTimer(10);
    }

    public void GameStart()
    {
        isPlaying = true;

        uiManager = GameObject.FindObjectOfType<UIManager>();
        uiManager.isGameStart = true;
        uiManager.totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        uiManager.curPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        interact = GameObject.FindObjectOfType<Interact>();
        interact.lastExitBatteryTime = Time.time;

        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Player[playerObjects.Length];

        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].GetComponent<Player>();
        }

        Debug.Log("일단 GameStart 함수는 실행");
        Debug.Log("내가 마스터 클라이언트의 상인가? :" + PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient)
        {
            MapManager.Instance.SpawndItemInMapRPC();//일단 여기는 잘 실행됨 ! 
            Go2Map();
        }
    }

    public void Escape()
    {
        Debug.Log("Escape 실행");
        isEscape = true;
    }

    public void GameOver()
    {
        Debug.Log("GameOver 실행");
        uiManager = GameObject.FindObjectOfType<UIManager>();
        if (uiManager.isGameOver == false)
        {
            isPlaying = false;
            uiManager.isGameOver = true;
        }
    }

    public void Go2Map()
    {
        Transform[] points = GameObject.Find("WarpPointGroup").GetComponentsInChildren<Transform>();

        int[] idx = new int[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            idx[i] = i;
        }

        Shuffle(idx);

        for (int i = 0; i < playerObjects.Length; i++)
        {
            Vector3 pos = points[idx[i]].position;
            players[i].Go2Map(pos);
        }

    }

    public void Shuffle(int[] deck)
    {
        for (int i = 0; i < deck.Length; i++)
        {
            int temp = deck[i];
            int randomIndex = Random.Range(0, deck.Length);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
}
