using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class MapManager : MonoBehaviour
{
    GameObject[] gameObjs;

    [Header("BatterySpawner")]
    [SerializeField] List<GameObject> BatterySpawnerTargets = new List<GameObject>();//배터리 스포너 후보들
    [SerializeField] int BatterySpawnerCount = 7;

    [Header("WeaponSpawner")]
    [SerializeField] List<GameObject> WeaponSpawnerTargets = new List<GameObject>();//무기 스포너 후보들
    [SerializeField] int WeaponSpawnerCount = 1;

    [Header("Light")]
    [SerializeField] List<GameObject> Lights = new List<GameObject>();//lights

    public PhotonView pv;

    // 모든 오브젝트들을 이름 기준으로 살펴보며 적절한 스크립트를 넣어줌 
    private void Awake()
    {
        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);

        gameObjs = FindObjectsOfType<GameObject>();

        for(int i = 0; i < gameObjs.Length; i++)
        {
            // 문에 door 스크립트 추가 
            if (gameObjs[i].name.Contains("Door") && !gameObjs[i].name.Contains("Frame") && !gameObjs[i].name.Contains("Window"))
            {
                //Debug.Log("gameObjs[i].name.Length : " + gameObjs[i].name.Length + "idx of L : " + gameObjs[i].name.LastIndexOf("L"));
                if (gameObjs[i].name.LastIndexOf("L") == gameObjs[i].name.Length-1)
                {
                    addDoorLeftScript(gameObjs[i]);
                }
                else
                {
                    addDoorRightScript(gameObjs[i]);
                }
                
                gameObjs[i].tag = "door";
                gameObjs[i].layer = LayerMask.NameToLayer("Interact");
                gameObjs[i].isStatic = false; // 이걸 해줘야 회전함!!
            }

            //게임 오브젝트들 중에서 이름에 BatterySpawner가 들어가면 스포너 후보에 추가하고 태그와 레이어 설정 -> 배터리가 추출 당했어도 우선 상호작용은 되야하니까 
            else if (gameObjs[i].name.Contains("BatterySpawner"))
            {
                BatterySpawnerTargets.Add(gameObjs[i]);

                gameObjs[i].tag = "ItemSpawner";
                gameObjs[i].layer = LayerMask.NameToLayer("Interact");
            }

            //게임 오브젝트들 중에서 이름에 WeaponSpawner가 들어가면 스포너 후보에 추가하고 태그와 레이어 설정 -> 배터리가 추출 당했어도 우선 상호작용은 되야하니까 
            else if (gameObjs[i].name.Contains("WeaponSpawner"))
            {
                WeaponSpawnerTargets.Add(gameObjs[i]);

                gameObjs[i].tag = "ItemSpawner";
                gameObjs[i].layer = LayerMask.NameToLayer("Interact");
            }
            else if (gameObjs[i].name.Contains("Lamp") && gameObjs[i].GetComponentInChildren<Light>())
            {
                Lights.Add(gameObjs[i]);

                //빛 초기환
                gameObjs[i].GetComponentInChildren<Light>().range = 5;
                gameObjs[i].GetComponentInChildren<Light>().intensity = 1;

            }
        }

        LocateBatterySpawner();//BatterySpawnerTargets 중 랜덤으로 스포너로 활성화 
        LocateWeaponSpawner();//WeaponSpawnerTargets 중 랜덤으로 스포너로 활성화 

    }

    void addDoorRightScript(GameObject obj)
    {
        //Door 컴포넌트가 있으면 그냥 활성화
        if (obj.gameObject.GetComponent<DoorRight>() != null)
        {
            obj.gameObject.GetComponent<DoorRight>().enabled = true;
        }
        else
        {
            //없으면 찾아서 넣어줌
            DoorRight door = obj.gameObject.AddComponent<DoorRight>();
            door.enabled = true;
        }
    }

    void addDoorLeftScript(GameObject obj)
    {
        //Door 컴포넌트가 있으면 그냥 활성화
        if (obj.gameObject.GetComponent<DoorLeft>() != null)
        {
            obj.gameObject.GetComponent<DoorLeft>().enabled = true;
        }
        else
        {
            //없으면 찾아서 넣어줌
            DoorLeft door = obj.gameObject.AddComponent<DoorLeft>();
            door.enabled = true;
        }
    }

    void LocateBatterySpawner()
    {
        int cnt = 0;
        while (cnt != BatterySpawnerCount)
        {
            int i = Random.Range(0, BatterySpawnerTargets.Count); //랜덤으로 인덱스 뽑아서
            //BatterySpawner 없으면 찾아서 넣어주고 활성화.
            if (BatterySpawnerTargets[i].gameObject.GetComponent<BatterySpawner>() == null)
            {
                BatterySpawner batterySpawner = BatterySpawnerTargets[i].AddComponent<BatterySpawner>();
                batterySpawner.enabled = true;
                cnt++;
            }
        }
    }

    void LocateWeaponSpawner()
    {
        int cnt = 0;
        while (cnt != WeaponSpawnerCount)
        {
            int i = Random.Range(0, WeaponSpawnerTargets.Count); //랜덤으로 인덱스 뽑아서
            //WeaponSpawner 없으면 찾아서 넣어주고 활성화
            if (WeaponSpawnerTargets[i].gameObject.GetComponent<WeaponSpawner>() == null)
            {
                WeaponSpawner weaponSpawner = WeaponSpawnerTargets[i].AddComponent<WeaponSpawner>();
                weaponSpawner.enabled = true;
                cnt++;
            }
        }
    }

    [PunRPC]
    public void SetLight(int range, int intensity)
    {
        for(int i=0;i< Lights.Count; i++) {
            Lights[i].GetComponentInChildren<Light>().range = range;
            Lights[i].GetComponentInChildren<Light>().intensity = intensity;
        }

    }

    public void SetLightRPC(int range, int intensity=1)
    {
        pv.RPC("SetLight", RpcTarget.AllBuffered, range, intensity);
    }
}
