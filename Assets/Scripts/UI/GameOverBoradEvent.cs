using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;
using Unity.VisualScripting;
using System;

public class GameOverBoradEvent : MonoBehaviour
{
    //게임 스코어보드 버튼
    [SerializeField] private Button scoreLobbyButton;
    [SerializeField] private Button deathcamButton;
    UIManager uiManager;
    private int updateValue;
    private void Awake()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    public void OnClickedScoreLobbyButton()
    {
        GetPlayerStatistics();
        SceneManager.LoadScene("Lobby");
    }

    public void OnClickedDeathcamButton()
    {
        return;
    }
    public void GetPlayerStatistics()
    {
        var request = new GetPlayerStatisticsRequest();
        PlayFabClientAPI.GetPlayerStatistics(request, OnGetStatistics, (error) => { });
    }

    public void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        updateValue = result.Statistics[0].Value + Int32.Parse(uiManager.totalScore.text);
        Debug.Log(updateValue);

        UpdatePlayerStatistics();
    }

    public void UpdatePlayerStatistics()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "Score", Value = updateValue } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => { });
    }
}
