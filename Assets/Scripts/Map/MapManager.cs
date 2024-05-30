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
    [SerializeField] int BatterySpawnerCount = 5;
    [SerializeField] List<GameObject> WorkingBatterySpawners = new List<GameObject>();//배터리 스포너들


    //일시적으로 웨폰 스포너에서 아이템도 나오도록 함. 우선 아이템 스포너는 없다고 생각해도 무관 
    [Header("WeaponSpawner")]
    [SerializeField] List<GameObject> WeaponSpawnerTargets = new List<GameObject>();//스포너 후보들
    [SerializeField] int WeaponSpawnerCount = 5;
    [SerializeField] List<GameObject> WorkingWeaponSpawners = new List<GameObject>();//스포너들 

    //[Header("ItemSpawner")]
    //[SerializeField] List<GameObject> ItemSpawnerTargets = new List<GameObject>();//스포너 후보들
    //[SerializeField] int ItemSpawnerCount = 1;
    //[SerializeField] List<GameObject> ItemSpawners = new List<GameObject>();//스포너 후보들


    [Header("hiddenItem")] // 맵에 미리 스폰돼있는 아이템
    //Spawner spawner;
    GameObject posPrefeb; //hiddenItem이 스폰될 pos
    [Header("hiddenItemAll")] // 맵에 미리 스폰돼있는 아이템
    [SerializeField] List<GameObject> hiddenItemTargetObjAll = new List<GameObject>();
    [SerializeField] public List<Item> itemsAll = new List<Item>();//스폰될 아이템 후보들. 아이템 버릴때에도 가져다 쓰신댔음 
                     List<GameObject> hiddenItemPosAll = new List<GameObject>();
    [SerializeField] int hiddenItemCntAll = 5;

    [Header("hiddenItemSmall")] // 맵에 미리 스폰돼있는 아이템
    [SerializeField] List<GameObject> hiddenItemTargetObjSmall = new List<GameObject>();
    [SerializeField] List<Item> itemsSmall = new List<Item>();//스폰될 아이템 후보들
                     List<GameObject> hiddenItemPosSmall = new List<GameObject>();
    [SerializeField] int hiddenItemCntSmall = 5;

    Vector3 Case_Door_offset = new Vector3(-0.2f, -1.0f, 0.2f);
    Vector3 ToiletDoor_offset = new Vector3(0.6f, -1.0f, 1.0f);
    Vector3 MirrorShelf_offset = new Vector3(0f,0.35f, -0.1f);
    Vector3 Fridge_offset = new Vector3(0f, 0.3f, 0f);
    Vector3 MedRack_offset = new Vector3(0f, 1.5f, 0f);
    Vector3 TableWhite_offset = new Vector3(0f, 1.5f, 0f);


    [Header("Light")]
    float brightSpeed = 1f;
    [SerializeField] List<GameObject> Lights = new List<GameObject>();//lights

    public PhotonView pv;

    // 모든 오브젝트들을 이름 기준으로 살펴보며 적절한 스크립트를 넣어줌 
    private void Awake()
    {
        //spawner = GetComponent<Spawner>();

        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);

        posPrefeb = (GameObject)Resources.Load("Prefabs/Point");

        //itemsAll
        itemsAll.Add((Item)Resources.Load("Item/Axe"));
        itemsAll.Add((Item)Resources.Load("Item/BaseballBat"));
        itemsAll.Add((Item)Resources.Load("Item/Butcher Knife"));
        itemsAll.Add((Item)Resources.Load("Item/Crowbar"));
        itemsAll.Add((Item)Resources.Load("Item/Hammer"));
        itemsAll.Add((Item)Resources.Load("Item/HeavyWrench"));
        itemsAll.Add((Item)Resources.Load("Item/Machete"));
        itemsAll.Add((Item)Resources.Load("Item/Shovel"));
        itemsAll.Add((Item)Resources.Load("Item/TacticalKnife"));

        itemsAll.Add((Item)Resources.Load("Item/Painkiller"));


        //itemsSmall
        itemsSmall.Add((Item)Resources.Load("Item/Painkiller"));


        gameObjs = FindObjectsOfType<GameObject>();

        for(int i = 0; i < gameObjs.Length; i++)
        {
            GameObject tmpObj = gameObjs[i];

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

                //for hiddenItem
                if (tmpObj.name.Contains("Case_Door"))
                {
                    InstantiatePosPrefeb_All(tmpObj, Case_Door_offset);
                }
                else if (tmpObj.name.Contains("ToiletDoor"))
                {
                    InstantiatePosPrefeb_All(tmpObj, ToiletDoor_offset);
                }

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
            else if (tmpObj.name.Contains("WeaponSpawner")
                || tmpObj.name.Contains("SheetRackCase")
                || tmpObj.name.Contains("SmallMetalicCase")
                || tmpObj.name.Contains("TableOffice")
                || tmpObj.name.Contains("Table_Bed"))
            {
                WeaponSpawnerTargets.Add(tmpObj);

                if (tmpObj.GetComponent<WeaponSpawner>() == null)
                {
                    tmpObj.gameObject.AddComponent<WeaponSpawner>();
                }

                tmpObj.tag = "ItemSpawner";
                tmpObj.layer = LayerMask.NameToLayer("Interact");
            }

            ////ItemSpawner
            //else if (tmpObj.name.Contains("ItemSpawner"))
            //{
            //    ItemSpawnerTargets.Add(tmpObj);

            //    tmpObj.tag = "ItemSpawner";
            //    tmpObj.layer = LayerMask.NameToLayer("Interact");
            //}

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


            //hiddenItem
            else if (tmpObj.name.Contains("MirrorShelf_Case"))
            {
                InstantiatePosPrefeb_Small(tmpObj, MirrorShelf_offset);
            }
            else if (tmpObj.name.Contains("Fridge_Case"))
            {
                InstantiatePosPrefeb_All(tmpObj, Fridge_offset);

            }
            else if (tmpObj.name.Contains("MedRack_case"))
            {
                InstantiatePosPrefeb_All(tmpObj, MedRack_offset);

            }
            else if (tmpObj.name.Contains("TableWhite"))
            {
                InstantiatePosPrefeb_All(tmpObj, TableWhite_offset);
            }
        }

        //이게 룸이 구성되기 전에 맵을 구성하라고 하니까 안됐음!! -> 포톤매니져에서 룸 생성이 되면 그 이후에 호출하도록 함 
        //LocateBatterySpawner();//BatterySpawnerTargets 중 랜덤으로 스포너로 활성화 
        //LocateWeaponSpawner();//WeaponSpawnerTargets 중 랜덤으로 스포너로 활성화
        //LocateItemSpawner();

        //SpawndItemInMap(); -> Instantiate는 딱 같은 시간에 동시에 진행해야 view ID가 같게 할당됨. 그래서 모든 플레이어가 존재하는 시점에서 실행돼야 함 !
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

    void InstantiatePosPrefeb_All(GameObject tmpObj, Vector3 offset)
    {
        hiddenItemTargetObjAll.Add(tmpObj);

        Vector3 pos = tmpObj.transform.position;
        GameObject tmp = Instantiate(posPrefeb, pos, tmpObj.transform.rotation);
        tmp.transform.parent = tmpObj.transform;//자식으로 넣어줌
        tmp.transform.Translate(offset);

        hiddenItemPosAll.Add(tmp);
    }

    void InstantiatePosPrefeb_Small(GameObject tmpObj, Vector3 offset)
    {
        hiddenItemTargetObjSmall.Add(tmpObj);

        Vector3 pos = tmpObj.transform.position;
        GameObject tmp = Instantiate(posPrefeb, pos, tmpObj.transform.rotation);
        tmp.transform.parent = tmpObj.transform;//자식으로 넣어줌
        tmp.transform.Translate(offset);

        hiddenItemPosSmall.Add(tmp);
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

            WorkingBatterySpawners.Add(BatterySpawnerTargets[i]); //마스터만 보임 

            check[i] = true;
            cnt++;
        }
    }

    public void EnableWeaponSpawner()
    {
        bool[] check = new bool[WeaponSpawnerTargets.Count]; // false로 초기화됨 

        int cnt = 0;

        while (cnt != WeaponSpawnerCount)
        {
            int i = Random.Range(0, WeaponSpawnerTargets.Count); //랜덤으로 인덱스 뽑아서

            if (check[i]) continue; //이미 스포너로 지정한 오브젝트라면 continue

            WeaponSpawner weaponSpawner = WeaponSpawnerTargets[i].GetComponent<WeaponSpawner>();
            weaponSpawner.EnableSpawnerWorking();

            WorkingWeaponSpawners.Add(WeaponSpawnerTargets[i]);

            check[i] = true;
            cnt++;
        }
    }

    public void SpawndItemInMapRPC()
    {
        if (PhotonNetwork.IsMasterClient)
            {
            //All
            int[] idx_all = new int[hiddenItemPosAll.Count];
            for (int i = 0; i < hiddenItemPosAll.Count; i++) idx_all[i] = i;
            GameManager.Instance.Shuffle(idx_all);

            for (int i = 0; i < hiddenItemCntAll; i++)
            {
                int itemNum = Random.Range(0, itemsAll.Count);
                int idx = idx_all[i];

                // pv.RPC("SpawnItemAll", RpcTarget.AllBuffered, itemNum, idx);

                Transform idxTransform = hiddenItemPosAll[idx].transform;
                PhotonNetwork.Instantiate("Prefabs/" + itemsAll[itemNum].itemName, idxTransform.position, idxTransform.rotation);
            }


            //Small
            int[] idx_small = new int[hiddenItemPosSmall.Count];
            for (int i = 0; i < hiddenItemPosSmall.Count; i++) idx_small[i] = i;
            GameManager.Instance.Shuffle(idx_small);

            for (int i = 0; i < hiddenItemCntSmall; i++)
            {
                int itemNum = Random.Range(0, itemsSmall.Count);
                int idx = idx_small[i];

                // pv.RPC("SpawnItemSmall", RpcTarget.AllBuffered, itemNum, idx);

                Transform idxTransform = hiddenItemPosSmall[idx].transform;
                PhotonNetwork.Instantiate("Prefabs/" + itemsSmall[itemNum].itemName, idxTransform.position, idxTransform.rotation);
            }
        }
    }


    //[PunRPC]
    //public void SpawnItemAll(int itemNum, int idx)
    //{
    //    //Debug.Log("SpawnItem 실행 " + "view ID : " + GetComponent<PhotonView>().ViewID);

    //    // GameObject ItemPrefab = itemsAll[itemNum].itemPrefab;
    //    Transform idxTransform = hiddenItemPosAll[idx].transform;

    //    // Instantiate(ItemPrefab, idxTransform.position, idxTransform.rotation);
    //    PhotonNetwork.Instantiate("Prefabs/" + itemsAll[itemNum].itemName, idxTransform.position, idxTransform.rotation);
    //}

    //[PunRPC]
    //public void SpawnItemSmall(int itemNum, int idx)
    //{
    //    //Debug.Log("SpawnItem 실행 " + "view ID : " + GetComponent<PhotonView>().ViewID);

    //    // GameObject ItemPrefab = itemsSmall[itemNum].itemPrefab;
    //    Transform idxTransform = hiddenItemPosSmall[idx].transform;

    //    // Instantiate(ItemPrefab, idxTransform.position, idxTransform.rotation);
    //    PhotonNetwork.Instantiate("Prefabs/" + itemsAll[itemNum].itemName, idxTransform.position, idxTransform.rotation);
    //}




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
