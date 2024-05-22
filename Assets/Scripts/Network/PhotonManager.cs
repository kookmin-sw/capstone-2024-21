using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 버전
    private readonly string version = "1.0f";

    void Awake()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";

        Debug.Log("userID: " + GameManager.Instance.UserId);
        // 같은 룸의 유저들에게 자동으로 씬 로딩 <- 이거 안되도록 수정
        PhotonNetwork.AutomaticallySyncScene = false;
        // 같은 버전의 유저끼리 접속 허용
        PhotonNetwork.GameVersion = version;
        // 유저 아이디 할당
        PhotonNetwork.NickName = GameManager.Instance.UserId;
        // 포톤 서버와 통신 횟수 확인. 초당 30회
        Debug.Log(PhotonNetwork.SendRate);
        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        // 잘 연결됐는지 확인
        Debug.Log("서버와 연결 성공");
        // 로비 입장했는지 확인, False
        Debug.Log($"로비 입장 여부 = {PhotonNetwork.InLobby}");
        // 로비 입장
        PhotonNetwork.JoinLobby();
    }

    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        // 로비 입장했는지 확인, True
        Debug.Log($"로비 입장 여부 = {PhotonNetwork.InLobby}");
        // 생성되어 있는 룸 중에서 랜덤하게 입장
        PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤 룸 입장에 실패했을 떄 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 에러 메세지 출력
        Debug.Log($"랜덤 룸 입장 실패 {returnCode}:{message}");

        // 룸 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 8;      // 최대 동접자 수: 8명
        ro.IsOpen = true;        // 룸의 오픈 여부
        ro.IsVisible = true;     // 로비에서 룸 목록 노출 여부

        // 룸 생성
        PhotonNetwork.CreateRoom("My Room" + PhotonNetwork.NickName, ro);
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("룸 생성");
        Debug.Log($"룸 이름 = {PhotonNetwork.CurrentRoom.Name}");
    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        //맵 구성
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("IsMasterClient 로써 맵 구성");
            MapManager.Instance.EnableBatterySpawner();
            MapManager.Instance.EnableWeaponSpawner();
            


        }
        else
        {
            Debug.Log("넌 클라이언트다 ");
        }



        Debug.Log($"룸 입장 여부 = {PhotonNetwork.InRoom}");
        Debug.Log($"현재 룸의 인원수 = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        // 캐릭터 출현 정보 배열에 저장
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        // 캐릭터 생성
        PhotonNetwork.Instantiate("Prefabs/Player", points[idx].position, points[idx].rotation, 0);

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            Debug.Log("현재 인원수: " + PhotonNetwork.CurrentRoom.PlayerCount);
            Debug.Log("Game Start!");

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            GameManager.Instance.TimerStart();

            Debug.Log("현재 방 오픈 여부: " + PhotonNetwork.CurrentRoom.IsOpen);
        }
    }
}
