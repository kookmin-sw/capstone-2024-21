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

    private void Awake()
    {
        gameObjs = FindObjectsOfType<GameObject>();

        for(int i = 0; i < gameObjs.Length; i++)
        {
            // 문에 door 스크립트 추
            if (gameObjs[i].name.Contains("Door") && !gameObjs[i].name.Contains("Frame") && !gameObjs[i].name.Contains("Window"))
            {
                //Debug.Log("gameObjs[i].name.Length : " + gameObjs[i].name.Length + "idx of L : " + gameObjs[i].name.LastIndexOf("L"));
                if (gameObjs[i].name.LastIndexOf("L") == gameObjs[i].name.Length-1)
                {
                    //Debug.Log("hit!");
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
                //gameObjs[i].isStatic = false; // 이거 해줘야 하나? 
            }

            //게임 오브젝트들 중에서 이름에 WeaponSpawner가 들어가면 스포너 후보에 추가하고 태그와 레이어 설정 -> 배터리가 추출 당했어도 우선 상호작용은 되야하니까 
            else if (gameObjs[i].name.Contains("WeaponSpawner"))
            {
                WeaponSpawnerTargets.Add(gameObjs[i]);

                gameObjs[i].tag = "ItemSpawner";
                gameObjs[i].layer = LayerMask.NameToLayer("Interact");
                //gameObjs[i].isStatic = false; // 이거 해줘야 하나? 
            }


        }

        //BatterySpawnerTargets 중 랜덤으로 스포너로 활성화 
        int cnt = 0;
        while (true)
        {
            int i = Random.Range(0, BatterySpawnerTargets.Count); //랜덤으로 인덱스 뽑아서
            //BatterySpawner 없으면 찾아서 넣어주고 활성화
            if (BatterySpawnerTargets[i].gameObject.GetComponent<BatterySpawner>() == null)
            {
                BatterySpawner batterySpawner = BatterySpawnerTargets[i].AddComponent<BatterySpawner>();
                batterySpawner.enabled = true;
                cnt++;
            }

            if (cnt == BatterySpawnerCount) break;
        }

        //WeaponSpawnerTargets 중 랜덤으로 스포너로 활성화 
        cnt = 0;
        while (true)
        {
            int i = Random.Range(0, WeaponSpawnerTargets.Count); //랜덤으로 인덱스 뽑아서
            //WeaponSpawner 없으면 찾아서 넣어주고 활성화
            if (WeaponSpawnerTargets[i].gameObject.GetComponent<WeaponSpawner>() == null)
            {
                WeaponSpawner weaponSpawner = WeaponSpawnerTargets[i].AddComponent<WeaponSpawner>();
                weaponSpawner.enabled = true;
                cnt++;
            }

            if (cnt == WeaponSpawnerCount) break;
        }

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
}
