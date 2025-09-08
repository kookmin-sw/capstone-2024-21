using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoardVisual : MonoBehaviour
{
    [SerializeField] private Button scoreLobbyButton;
    [SerializeField] private Button deathcamButton;
    
    private TextMeshProUGUI statePlayerName;
    private TextMeshProUGUI playerName;

    private TextMeshProUGUI currentPlayers;
    private TextMeshProUGUI allPlayers;

    private TextMeshProUGUI kill;
    private TextMeshProUGUI rank;
    private TextMeshProUGUI survivalTime;

    private TextMeshProUGUI rankScore;
    private TextMeshProUGUI killScore;
    public TextMeshProUGUI totalScore;

    void Awake()
    {
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        
    }

    void InitScoreBoard()
    {
        playerName.SetText(GameManager.Instance.UserId);
    }

    void CalculateScore()
    {

    }
    //게임 스코어보드 버튼

    public void OnClickedScoreLobbyButton()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void OnClickedDeathcamButton()
    {
        return;
    }
}
