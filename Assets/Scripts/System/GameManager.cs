using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public enum GameState
{
    Lobby,
    Ready,
    InGame,
    GameOver
}
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

    public int totalPlayers { get; private set; }
    private int _curPlayers;

    PhotonView pv;

    public int curPlayers
    {
        get { return _curPlayers; }
        set
        {
            _curPlayers = value;
            OnPlayerCountChanged?.Invoke(_curPlayers, totalPlayers);

            if (_curPlayers == 1) SetState(GameState.GameOver);
        }
    }

    public delegate void GameStateChanged(GameState newState);
    public event GameStateChanged OnStateChanged;

    public delegate void PlayerCountChanged(int curPlayer, int totalPlayer);
    public event PlayerCountChanged OnPlayerCountChanged;

    public GameObject[] playerObjects;
    Player[] players;

    public GameState CurrentState { get; private set; } = GameState.Lobby;


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
        pv = GetComponent<PhotonView>();
        Init();
    }
    public void SetState(GameState state)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        pv.RPC(nameof(ApplyStateRPC), RpcTarget.All, state);
    }

    [PunRPC]
    void ApplyStateRPC(GameState state)
    {
        CurrentState = state;
        OnStateChanged?.Invoke(CurrentState);
    }
    void Init()
    {
        //게임 상태 구독자 구독
        IEnumerable<IGameStateListener> gsListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                                                      .OfType<IGameStateListener>();
        foreach (IGameStateListener listener in gsListeners) OnStateChanged += listener.OnStateChanged;

        //플레이어 카운트 구독자 구독
        IEnumerable<IPlayerCountListener> pcListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                                                        .OfType<IPlayerCountListener>();
        foreach (IPlayerCountListener listener in pcListeners) OnPlayerCountChanged += listener.OnPlayerCountChanged;

        //타이머 구독
        Timer.OnZero += GameStart;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetState(GameState.Ready);
        }
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
        Debug.Log("Escape 실행");
        isEscape = true;
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
