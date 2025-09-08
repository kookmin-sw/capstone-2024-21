using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public enum GameState
{
    Lobby,
    Ready,
    InGame,
    GameOver
}
public class GameManager : MonoBehaviourPun
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
    public GameState CurrentState { get; private set; }

    public string UserId { get; set; } = "soldier";

    public GameObject[] playerObjects;
    Player[] players;

    public int totalPlayers { get; private set; }

    private int _curPlayers;
    public int curPlayers 
    {
        get { return _curPlayers; } 
        set 
        { 
            _curPlayers = value;
            OnPlayerCountChanged?.Invoke();

            if (_curPlayers == 1) SetState(GameState.GameOver);
        }
    }

    public delegate void GameStateChanged(GameState newState);
    public event GameStateChanged OnStateChanged;

    public delegate void PlayerCountChanged();
    public event PlayerCountChanged OnPlayerCountChanged;

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

    void Start()
    {
        Timer.OnZero += GameStart;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetState(GameState.Ready);
        }
    }

    public void SetState(GameState state)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(ApplyStateRPC), RpcTarget.All, state);
    }

    [PunRPC]
    void ApplyStateRPC(GameState state)
    {
        CurrentState = state;
        OnStateChanged.Invoke(CurrentState);
    }

    public void GameStart()
    {
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        curPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Player[playerObjects.Length];

        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].GetComponent<Player>();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            MapManager.Instance.SpawndItemInMapRPC();//일단 여기는 잘 실행됨 ! 
            Go2Map();
        }

        SetState(GameState.InGame);
    }

    public void Escape()
    {
        SetState(GameState.GameOver);
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
            int randomIndex = Random.Range(i, deck.Length);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
}
