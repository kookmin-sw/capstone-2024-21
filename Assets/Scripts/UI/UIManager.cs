using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SelectedSlot[] slots;
    [SerializeField] private GameObject combinationSlots;
    [SerializeField] private GameObject systemEnvironment;
    [SerializeField] private GameObject gameOverBoard;

    [SerializeField] private TextMeshProUGUI statePlayerName;
    [SerializeField] private TextMeshProUGUI gameOverPlayerName;
    [SerializeField] private TextMeshProUGUI rankScore;
    [SerializeField] private TextMeshProUGUI killScore;
    [SerializeField] private TextMeshProUGUI survivalTime;


    [SerializeField] private TextMeshProUGUI killPoint;
    [SerializeField] private TextMeshProUGUI rankPoint;
    public TextMeshProUGUI totalScore;

    private float gameTime;
    private int selectSlot;

    public int killCount;
    public int totalPlayers;
    public int curPlayers;

    [HideInInspector] public bool isGameStart;
    [HideInInspector] public bool isGameOver;
    [HideInInspector] public bool isUIActivate;
    [HideInInspector] public bool isComActivate;


    // Start is called before the first frame update
    void Awake()
    {
        combinationSlots.SetActive(false);
        systemEnvironment.SetActive(false);
        gameOverBoard.SetActive(false);
        isGameStart = false;
        isGameOver = false;
        isUIActivate = false;
        gameTime = 0;
        selectSlot = 0;
        ChangeSlot(0);

        statePlayerName.text = GameManager.Instance.UserId;
        gameOverPlayerName.text = GameManager.Instance.UserId;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {
        if(isGameStart == true)
        {
            gameTime += Time.deltaTime;
        }
        if(isGameOver == false)
        {
            ManageCombinationSlot();
            ManageSetting();
        }
        else
        {
            ManageGameOverBoard();
        }
    }

    void ManageCombinationSlot()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            combinationSlots.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            isComActivate = true;

        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            combinationSlots.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isComActivate = false;
        }
    }
    void ManageSetting()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!systemEnvironment.activeSelf)
            {
                systemEnvironment.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                isUIActivate = true;
            }
            else
            {
                systemEnvironment.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                isUIActivate = false;
            }
        }
    }

    void ManageGameOverBoard()
    {
        if(isGameOver)
        {
            isGameStart = false;
            gameTime = Mathf.FloorToInt(gameTime);
            survivalTime.text = (gameTime / 60).ToString("00") + ":" + (gameTime % 60).ToString("00");
            killScore.text = killCount.ToString(); //킬매니저에 killCount넣어줘야 한다!!!
            rankScore.text = curPlayers.ToString() + "/" + totalPlayers.ToString();

            killPoint.text = "+" + (killCount * 5).ToString();
            rankPoint.text = "+" + Mathf.FloorToInt(20 / curPlayers).ToString();
            totalScore.text = ((killCount * 5) + Mathf.FloorToInt(20 / curPlayers)).ToString();

            gameOverBoard.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    //퀵슬롯 1,2,3,4,5로 선택
    public void SelectQuickSlot()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeSlot(4);
    }
    //이전 선택 슬롯 비활성화, 현재 선택 슬롯 활성화
    void ChangeSlot(int pressValue)
    {
        slots[selectSlot].Deselected();
        slots[pressValue].Selected();
        selectSlot = pressValue;
    }


}
