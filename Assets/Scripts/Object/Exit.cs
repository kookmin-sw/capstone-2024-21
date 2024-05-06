using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Exit : MonoBehaviour
{
    // 문을 열고 닫기위해 필요한 변수
    [SerializeField] private bool open = false;
    private Vector3 localPositoin;
    private float offset = 2f;
    private float speed = 1.3f;

    public PhotonView pv;


    private void Start()
    {
        localPositoin = transform.position;

        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);
    }


    // 아 이거 위치를 이상하게 받아오는데 이유를 몰라서 우선 그냥 돌아가게 해놨습니다 
    private IEnumerator ExitOpenDoor(Transform obsTransform)
    {
        Debug.Log("Exit OpenDoor() 코루틴 실행됨 ");

        //Debug.Log("localPositoin :"+ localPositoin);
        while (transform.position.y < localPositoin.y + offset)
        {
            //Debug.Log(transform.position.y);
            yield return null;
            obsTransform.Translate( Vector3.forward * speed * Time.deltaTime);
        }
        //Debug.Log("localPositoin :" + transform.position);
    }


    [PunRPC]
    public void ChangeExitDoorState()
    {
        open = !open;

        if (open)
        {
            StartCoroutine(ExitOpenDoor(transform));
        }

    }

    public void ChangeExitDoorStateRPC()
    {
        pv.RPC("ChangeExitDoorState", RpcTarget.AllBuffered);
    }

}
