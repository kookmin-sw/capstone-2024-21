using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyPagePlayfab : MonoBehaviour
{
    public string playFabId;
    public string username;
    public TextMeshProUGUI playerName;
    public GameObject rowPrefab;
    public Transform rowsParent;
    void Awake()
    {

        // 로그인된 상태에서만 PlayFab ID를 가져옵니다.
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            // GetAccountInfo 요청을 보냅니다.
            var request = new GetAccountInfoRequest();

            PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
        }
        else
        {
            Debug.LogError("User is not logged in.");
        }

        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, (error) => { });
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach(Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach(var item in result.Leaderboard)
        {
            GameObject row = Instantiate(rowPrefab, rowsParent);
            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = (item.Position+1).ToString();
            texts[1].text = item.DisplayName.ToString();
            texts[2].text = item.StatValue.ToString();
        }
    }

    // 계정 정보 가져오기 성공 시 호출될 콜백 함수
    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        playFabId = result.AccountInfo.PlayFabId;
        username = result.AccountInfo.Username;
        GameManager.Instance.UserId = username;
        playerName.text = username;
        Debug.Log("PlayFab ID: " + playFabId);
        Debug.Log("PlayFab ID: " + username);
    }

    // 계정 정보 가져오기 실패 시 호출될 콜백 함수
    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError("GetAccountInfo request failed: " + error.GenerateErrorReport());
    }
}
