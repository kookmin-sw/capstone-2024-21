using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color preColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private GameObject playText;
    [SerializeField] private GameObject leaderBoardText;
    [SerializeField] private GameObject statisticText;

    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject leaderBoardPanel;
    [SerializeField] private GameObject statisticPanel;

    [SerializeField] private GameObject canvas;
    [SerializeField] private LobbyPagePlayfab lobbyPlayFab;

    [SerializeField] private bool isPlayClicked;
    [SerializeField] private bool isLeaderBoardClicked;
    [SerializeField] private bool isClicked;


    void Awake()
    {
        preColor = gameObject.GetComponent<TextMeshProUGUI>().color;
        hoverColor = gameObject.GetComponent<TextMeshProUGUI>().color;
        playText = GameObject.Find("PlayText");
        leaderBoardText = GameObject.Find("LeaderBoardText");
        statisticText = GameObject.Find("StatisticText");
        canvas = GameObject.Find("Canvas");

    }
    // 버튼 호버
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverColor.a = 0.7f;
        gameObject.GetComponent<TextMeshProUGUI>().color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = preColor;
    }

    // 버튼 클릭
    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = hoverColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerEnter == playText)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = hoverColor;
            leaderBoardText.GetComponent<TextMeshProUGUI>().color = preColor;
            statisticText.GetComponent<TextMeshProUGUI>().color = preColor;

            playPanel.SetActive(true);
            player.SetActive(true);

            leaderBoardPanel.SetActive(false);
            statisticPanel.SetActive(false);
        }
        else if (eventData.pointerEnter == leaderBoardText)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = hoverColor;
            playText.GetComponent<TextMeshProUGUI>().color = preColor;
            statisticText.GetComponent<TextMeshProUGUI>().color = preColor;

            lobbyPlayFab = canvas.GetComponent<LobbyPagePlayfab>();
            lobbyPlayFab.GetLeaderboard();
            leaderBoardPanel.SetActive(true);

            playPanel.SetActive(false);
            player.SetActive(false);
            statisticPanel.SetActive(false);
        }
        else if (eventData.pointerEnter == statisticText)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = hoverColor;
            leaderBoardText.GetComponent<TextMeshProUGUI>().color = preColor;
            playText.GetComponent<TextMeshProUGUI>().color = preColor;

            statisticPanel.SetActive(true);

            playPanel.SetActive(false);
            player.SetActive(false);
            leaderBoardPanel.SetActive(false);
        }
    }
}