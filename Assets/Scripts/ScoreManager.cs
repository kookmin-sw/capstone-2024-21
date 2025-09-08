using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour,IDeathListener
{
    [SerializeField] private int rankScoreAmount = 20;

    private int totalScore;

    private int updateValue;

    private KillManager killManager;

    public delegate void OnCalculatedEvent();
    public static event OnCalculatedEvent OnCalculated;
    public void OnDeath()
    {
        CalculateScore(); //점수 계산 후
        GetPlayerStatistics(); // db에 반영
        OnCalculated?.Invoke(); // UI에 반영
    }

    private void CalculateScore()
    {
        killManager = GetComponent<KillManager>();

        int killScore = killManager.killCount;
        int rankScore = rankScoreAmount / GameManager.Instance.curPlayers;
        totalScore = killScore + rankScore;
    }

    /// <summary>
    /// playfab에서 플레이어 통계 데이터를 가져와 계산된 점수를 더하고 저장하는 함수
    /// </summary>
    public void GetPlayerStatistics()
    {
        var request = new GetPlayerStatisticsRequest();
        PlayFabClientAPI.GetPlayerStatistics(request, OnGetStatistics, (error) => { });
    }

    public void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        updateValue = result.Statistics[0].Value + totalScore;

        UpdatePlayerStatistics();
    }

    public void UpdatePlayerStatistics()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "Score", Value = updateValue } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => { });
    }
}

