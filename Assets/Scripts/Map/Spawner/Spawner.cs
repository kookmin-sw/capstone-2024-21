using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    //스포너 오브젝트 근처로 포물선 운동을 하며 스폰됨

    [SerializeField] protected List<Item> items; //상속받은 스포너들에서 객체 생성해서 할당해줘야함 
    [SerializeField] protected bool isSpawned = false; // 아이템이 하나 이상 나오지 않도록 하기위해

    GameObject ItemPrefab; //items중 생성될 아이템 
    float maxDistance = 1f; // 아이템이 스폰될 최대 반경
    Vector3 offset_ = new Vector3(0, 1f, 0); //얼마나 위에서 item 복제본 생성할지 

    //포물선 운동을 위한 변수들
    float m_InitialAngle = 70f; // 처음 날라가는 각도
    Rigidbody itemRigidbody;

    //네트워크
    public PhotonView pv;

    //private void Start()
    //{
    //    pv = gameObject.AddComponent<PhotonView>();
    //    pv.ViewID = PhotonNetwork.AllocateViewID(0);
    //}

    //interact 스크립트에서 호출됨. 아이템 리스트중 랜덤으로 하나를 뽑아서 스폰
    [PunRPC]
    public void SpawnItem()
    {
        //아이템이 전에 스폰되지 않았을 경우에만 스폰. 
        if (!isSpawned)
        {
            //int randomItemNumber = Random.Range(0, items.Count); //items중 랜덤 인덱스 추출 
            //ItemPrefab = items[randomItemNumber].itemPrefab;

            // 스포너 근처의랜덤 위치를 가져옵니다.
            //Vector3 spawnPosition = transform.position + (Random.insideUnitSphere * maxDistance); //현재 위치에서 maxDistance 반경 랜덤으로 원형자리에 Vector3를 구함

            Debug.Log("transform.name " + transform.name);
            Debug.Log("transform.position " + transform.position);

            //GameObject item = Instantiate(ItemPrefab, transform.position + offset_, transform.rotation); //item 복제본 생성
            //itemRigidbody = item.GetComponent<Rigidbody>();
            //Vector3 velocity = GetVelocity(transform.position, spawnPosition, m_InitialAngle);
            //itemRigidbody.velocity = velocity;

            Debug.Log("item is spawned");
            isSpawned = true;
        }
        else
        {
            Debug.Log("이미 전에 아이템이 스폰됐습니다.");
        }
    }

    public void SpawnItemRPC()
    {
        pv.RPC("SpawnItem", RpcTarget.AllBuffered);
    }

    //포물선을 그리며 스폰되도록 하는 함수. 이건 그냥 가져옴..ㅋ 
    private Vector3 GetVelocity(Vector3 start_pos, Vector3 target_pos, float initialAngle)
    {
        float gravity = Physics.gravity.magnitude;
        float angle = initialAngle * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(target_pos.x, 0, target_pos.z);
        Vector3 planarPosition = new Vector3(start_pos.x, 0, start_pos.z);

        float distance = Vector3.Distance(planarTarget, planarPosition);
        float yOffset = start_pos.y - target_pos.y;

        float initialVelocity
            = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity
            = new Vector3(0f, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        float angleBetweenObjects
            = Vector3.Angle(Vector3.forward, planarTarget - planarPosition) * (target_pos.x > start_pos.x ? 1 : -1);
        Vector3 finalVelocity
            = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        return finalVelocity;
    }
}
