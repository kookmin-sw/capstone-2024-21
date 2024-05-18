using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class MapManager : MonoBehaviour
{
    //싱글턴
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MapManager>();
                if (!_instance)
                {
                    GameObject obj = new GameObject();
                    obj.name = "MapManager";
                    _instance = obj.AddComponent(typeof(MapManager)) as MapManager;
                }
            }
            return _instance;
        }
    }

    [Header("About Exit")]
    [SerializeField] public int ExitNeedBattery = 2;
    [SerializeField] public int ExitChargedBattery = 0;

    GameObject[] gameObjs;

    [Header("BatterySpawner")]
    [SerializeField] List<GameObject> BatterySpawnerTargets = new List<GameObject>();//배터리 스포너 후보들
    [SerializeField] int BatterySpawnerCount = 7;
    [SerializeField] List<GameObject> WorkingBatterySpawners = new List<GameObject>();//배터리 스포너들


    //일시적으로 웨폰 스포너에서 아이템도 나오도록 함. 우선 아이템 스포너는 없다고 생각해도 무관 
    [Header("WeaponSpawner")]
    [SerializeField] List<GameObject> WeaponSpawnerTargets = new List<GameObject>();//스포너 후보들
    [SerializeField] int WeaponSpawnerCount = 50;
    [SerializeField] List<GameObject> WeaponSpawners = new List<GameObject>();//스포너 후보들

    [Header("ItemSpawner")]
    [SerializeField] List<GameObject> ItemSpawnerTargets = new List<GameObject>();//스포너 후보들
    [SerializeField] int ItemSpawnerCount = 1;
    [SerializeField] List<GameObject> ItemSpawners = new List<GameObject>();//스포너 후보들

    [Header("Light")]
    float brightSpeed = 1f;
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
            GameObject tmpObj = gameObjs[i];

            //if (tmpObj.GetComponent<PhotonView>())
            //{
            //    Debug.Log("name : " + tmpObj.gameObject.name + " || id : " + tmpObj.GetComponent<PhotonView>().ViewID);
            //}

            // door 
            if (tmpObj.name.Contains("Door") && !tmpObj.name.Contains("Frame") && !tmpObj.name.Contains("Window"))
            {
                //Debug.Log("tmpObj.name.Length : " + tmpObj.name.Length + "idx of L : " + tmpObj.name.LastIndexOf("L"));
                if (tmpObj.name.LastIndexOf("L") == tmpObj.name.Length-1)
                {
                    addDoorLeftScript(tmpObj);
                }
                else
                {
                    addDoorRightScript(tmpObj); 
                }

                tmpObj.tag = "door";
                tmpObj.layer = LayerMask.NameToLayer("Interact");
                tmpObj.isStatic = false; // 이걸 해줘야 회전함!!
            }

            //BatterySpawner
            else if (tmpObj.name.Contains("BatterySpawner"))
            {
                BatterySpawnerTargets.Add(tmpObj);

                if (tmpObj.GetComponent<BatterySpawner>()==null)
                {
                    tmpObj.gameObject.AddComponent<BatterySpawner>();
                }

                //AddPv(tmpObj); // pv 스크립트로 넣으면 뷰 아이디가 동일하게 안들어간다.... 하나하나 넣어줘야 일치함... 

                tmpObj.tag = "ItemSpawner";
                tmpObj.layer = LayerMask.NameToLayer("Interact");
            }

            // WeaponSpawner
            else if (tmpObj.name.Contains("WeaponSpawner") || tmpObj.name.Contains("SheetRackCase") || tmpObj.name.Contains("SmallMetalicCase"))
            {
                WeaponSpawnerTargets.Add(tmpObj);

                tmpObj.tag = "ItemSpawner";
                tmpObj.layer = LayerMask.NameToLayer("Interact");
            }

            //ItemSpawner
            else if (tmpObj.name.Contains("ItemSpawner"))
            {
                ItemSpawnerTargets.Add(tmpObj);

                tmpObj.tag = "ItemSpawner";
                tmpObj.layer = LayerMask.NameToLayer("Interact");
            }

            // 빛
            else if (tmpObj.name.Contains("Lamp") && tmpObj.GetComponentInChildren<Light>())
            {
                Lights.Add(tmpObj);

                //빛 초기환
                Light light = tmpObj.GetComponentInChildren<Light>();
                SetLight(light, 5, 1); //range=5, intensity =1로 초기화 

            }

            // ground 레이어 달기
            else if (tmpObj.name.Contains("floor") || tmpObj.name.Contains("Stair"))
            {
                tmpObj.layer = LayerMask.NameToLayer("Ground");

            }
        }

        //이게 룸이 구성되기 전에 맵을 구성하라고 하니까 안됐음!! -> 포톤매니져에서 룸 생성이 되면 그 이후에 호출하도록 함 
        //LocateBatterySpawner();//BatterySpawnerTargets 중 랜덤으로 스포너로 활성화 
        //LocateWeaponSpawner();//WeaponSpawnerTargets 중 랜덤으로 스포너로 활성화
        //LocateItemSpawner();
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



    public void EnableBatterySpawner()
    {
        bool[] check = new bool[BatterySpawnerTargets.Count]; // false로 초기화됨 

        int cnt = 0;

        while (cnt != BatterySpawnerCount)
        {
            int i = Random.Range(0, BatterySpawnerTargets.Count); //랜덤으로 인덱스 뽑아서

            if (check[i]) continue; //이미 스포너로 지정한 오브젝트라면 continue

            BatterySpawner batterySpawner = BatterySpawnerTargets[i].GetComponent<BatterySpawner>();
            batterySpawner.EnableSpawnerWorking();

            WorkingBatterySpawners.Add(BatterySpawnerTargets[i]);

            check[i] = true;
            cnt++;
        }
    }






    public void LocateBatterySpawner()
    {
        bool[] check = new bool[BatterySpawnerTargets.Count]; // false로 초기화됨 

        int cnt = 0;

        while (cnt != BatterySpawnerCount)
        {
            int i = Random.Range(0, BatterySpawnerTargets.Count); //랜덤으로 인덱스 뽑아서
            if (check[i]) continue; //이미 스포너로 지정한 오브젝트라면 continue

            pv.RPC("AddBatterySpawnerScriptRPC", RpcTarget.AllBuffered, i);
            check[i]=true;
            cnt++;
        }
    }

    //public void DisableSpawners_tmp()
    //{
    //    bool[] check = new bool[BatterySpawnerTargets.Count]; // false로 초기화됨 

    //    int cnt = 0;

    //    while (cnt != BatterySpawnerCount)
    //    {
    //        int i = Random.Range(0, BatterySpawnerTargets.Count); //랜덤으로 인덱스 뽑아서
    //        if (check[i]) continue; //이미 스포너로 지정한 오브젝트라면 continue

    //        BatterySpawnerTargets[i].GetComponent<BatterySpawner>().DisableSpawner();
    //        BatterySpawners.Add(BatterySpawnerTargets[i]);
    //        check[i] = true;
    //        cnt++;
    //    }
    //}

    void AddPv(GameObject obj)
    {
        if (obj.gameObject.GetComponent<PhotonView>() == null)
        {
            PhotonView targetPV = obj.AddComponent<PhotonView>();
            targetPV.ViewID = PhotonNetwork.AllocateViewID(0);
        }
    }

    //[PunRPC]
    //void AddBatterySpawnerScriptRPC(int i)
    //{
    //    // 포톤 없으면 포톤 달아주고 
    //    if (BatterySpawnerTargets[i].gameObject.GetComponent<PhotonView>() == null)
    //    {
    //        PhotonView targetPV = BatterySpawnerTargets[i].AddComponent<PhotonView>();
    //        targetPV.ViewID = PhotonNetwork.AllocateViewID(0);
    //    }

    //    // 배터리 스포너 스크립트 달아주고 
    //    if (BatterySpawnerTargets[i].gameObject.GetComponent<BatterySpawner>() == null)
    //    {
    //        BatterySpawner batterySpawner = BatterySpawnerTargets[i].AddComponent<BatterySpawner>();
    //        batterySpawner.enabled = true;
    //    }

    //    BatterySpawners.Add(BatterySpawnerTargets[i]);
    //}
    
    void LocateWeaponSpawner()
    {
        int cnt = 0;
        while (cnt != WeaponSpawnerCount)
        {
            int i = Random.Range(0, WeaponSpawnerTargets.Count); //랜덤으로 인덱스 뽑아서

            if (WeaponSpawnerTargets[i].gameObject.GetComponent<WeaponSpawner>() == null)
            {
                if (WeaponSpawnerTargets[i].gameObject.GetComponent<PhotonView>() == null)
                {
                    PhotonView targetPV = WeaponSpawnerTargets[i].AddComponent<PhotonView>();
                    targetPV.ViewID = PhotonNetwork.AllocateViewID(0);
                }

                WeaponSpawner weaponSpawner = WeaponSpawnerTargets[i].AddComponent<WeaponSpawner>();
                weaponSpawner.enabled = true;
                cnt++;

                WeaponSpawners.Add(WeaponSpawnerTargets[i]);
            }
        }
    }

    void LocateItemSpawner()
    {
        int cnt = 0;
        while (cnt != ItemSpawnerCount)
        {
            int i = Random.Range(0, ItemSpawnerTargets.Count); //랜덤으로 인덱스 뽑아서
            if (ItemSpawnerTargets[i].gameObject.GetComponent<ItemSpawner>() == null)
            {
                if (ItemSpawnerTargets[i].gameObject.GetComponent<PhotonView>() == null)
                {
                    PhotonView targetPV = ItemSpawnerTargets[i].AddComponent<PhotonView>();
                    targetPV.ViewID = PhotonNetwork.AllocateViewID(0);
                }

                ItemSpawner itemSpawner = ItemSpawnerTargets[i].AddComponent<ItemSpawner>();
                itemSpawner.enabled = true;
                cnt++;

                ItemSpawners.Add(ItemSpawnerTargets[i]);
            }
        }
    }

    //여기부터 살펴보기 !!!
    public IEnumerator BightenLight(Light light, int range)
    {
        while (light.range < range)
        {
            yield return null; //yield return을 만나는 순간마다 다음 구문이 실행되는 프레임으로 나뉘게 됨 
            light.range += brightSpeed * Time.deltaTime;
        }
    }

    [PunRPC]
    public void StartCoroutineBightenLight(int range)
    {
        for (int i = 0; i < Lights.Count; i++)
        {
            Light light = Lights[i].GetComponentInChildren<Light>();
            StartCoroutine(BightenLight(light, range));
        }
    }

    public void BightenLightRPC(int range)
    {
        pv.RPC("StartCoroutineBightenLight", RpcTarget.AllBuffered, range);
    }

    public void SetLight(Light light, int range, int intensity = 1)
    {
        light.range = range;
        light.intensity = intensity;
    }

    [PunRPC]
    public void AddChargeBattery()
    {
        Debug.Log("AddChargeBattery 실행! 추가 전 : " + ExitChargedBattery);
        ExitChargedBattery++;
        Debug.Log("  추가 후  : " + ExitChargedBattery);
    }

    public void AddChargeBatteryRPC()
    {
        pv.RPC("AddChargeBattery", RpcTarget.AllBuffered);
    }

}
