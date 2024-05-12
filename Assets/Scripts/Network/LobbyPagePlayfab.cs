using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEditor.FilePathAttribute;
using UnityEditor.PackageManager;
using System.Linq;

public class LobbyPagePlayfab : MonoBehaviour
{
    public string playFabId;
    public string userName;
    public string playerLocation;
    public string playerRank;
    public string playerScore;
    public TextMeshProUGUI playerName;
    public GameObject rowPrefab;
    public Transform rowsParent;

    public LobbyUIManager lobbyUIManager;
    void Awake()
    {
        lobbyUIManager = GetComponent<LobbyUIManager>();
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
        GetPlayerCountry();
    }



    public void GetPlayerCountry()
    {
        var request = new GetPlayerProfileRequest
        {
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowLocations = true // 위치 정보(국가)를 요청합니다.
            }
        };
        PlayFabClientAPI.GetPlayerProfile(request, OnPlayerCountryGet, (error) => { });
    }

    public void OnPlayerCountryGet(GetPlayerProfileResult result)
    {
        playerLocation = result.PlayerProfile.Locations[0].CountryCode.Value.ToString();
        lobbyUIManager.locationText.text = playerLocation;
    }
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 100
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, (error) => { });
    }

    public void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach(Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            if(i < 10)
            {
                GameObject row = Instantiate(rowPrefab, rowsParent);
                TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = (result.Leaderboard[i].Position + 1).ToString();
                texts[1].text = result.Leaderboard[i].DisplayName;
                texts[2].text = result.Leaderboard[i].StatValue.ToString();
            }

            // 자신의 정보인 경우 순위와 점수를 업데이트합니다.
            if (result.Leaderboard[i].PlayFabId == playFabId)
            {
                playerRank = (result.Leaderboard[i].Position + 1).ToString();
                playerScore = result.Leaderboard[i].StatValue.ToString();
                lobbyUIManager.rankText.text = playerRank;
                lobbyUIManager.scoreText.text = playerScore;
            }
        }
    }

    // 계정 정보 가져오기 성공 시 호출될 콜백 함수
    public void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        playFabId = result.AccountInfo.PlayFabId;
        userName = result.AccountInfo.Username;
        GameManager.Instance.UserId = userName;
        lobbyUIManager.nameText.text = userName;
        lobbyUIManager.tagText.text = playFabId;
        playerName.text = userName;
    }

    // 계정 정보 가져오기 실패 시 호출될 콜백 함수
    public void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError("GetAccountInfo request failed: " + error.GenerateErrorReport());
    }
}
