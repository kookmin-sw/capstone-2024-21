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

    public Timer timer;
    GameObject[] playerObjects;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TimerStart();
        }
    }

    public void TimerStart()
    {
        timer.StartTimer(10);
    }

    public void GameStart()
    {
        isPlaying = true;

        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Player[playerObjects.Length];

        uiManager = GameObject.FindObjectOfType<UIManager>();
        uiManager.isGameStart = true;
        uiManager.totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        uiManager.curPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].GetComponent<Player>();
        }
        Go2Map();
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
        isPlaying = false;
        uiManager.isGameOver = true;
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

    void Shuffle(int[] deck)
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
