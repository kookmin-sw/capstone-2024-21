using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class DoorLeft : MonoBehaviour
{
    public bool open = false;
    public float smoot = 0.05f;

    private Vector3 doorOpenVector = new Vector3(0, 90f, 0); //right 와 여기만 다름!!
    private Vector3 CloseDoorAngle; //초기각도
    private Vector3 OpenDoorAngle;

    public PhotonView pv;

    private void Start()
    {
        if (transform.parent.name.Contains("axis"))
        {
            CloseDoorAngle = transform.parent.eulerAngles;
            OpenDoorAngle = CloseDoorAngle + doorOpenVector;
        }
        else
        {
            CloseDoorAngle = transform.eulerAngles;
            OpenDoorAngle = CloseDoorAngle + doorOpenVector;
        }

        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);
    }

    public IEnumerator OpenDoor(Transform obsTransform)
    {
        Debug.Log("OpenDoor() 코루틴 실행됨 ");
        float timecnt = 0.0f;

        while (open && Quaternion.Angle(obsTransform.rotation, Quaternion.Euler(OpenDoorAngle)) > 0)  //문이 열려야하고 두사이각이 0보다 큰 경우 실행
        {
            yield return null;
            //Debug.Log("open while문 실행");
            obsTransform.rotation = Quaternion.Slerp(obsTransform.rotation, Quaternion.Euler(OpenDoorAngle), smoot);
            timecnt += Time.deltaTime;
        }

    }

    public IEnumerator CloseDoor(Transform obsTransform)
    {

        Debug.Log("CloseDoor() 코루틴 실행됨 ");
        float timecnt = 0.0f;

        while (!open && Quaternion.Angle(obsTransform.rotation, Quaternion.Euler(CloseDoorAngle)) > 0) //문이 닫혀야 하고 두사이각이 0보다 큰 경우 실행
        {
            yield return null; //yield return을 만나는 순간마다 다음 구문이 실행되는 프레임으로 나뉘게 됨 
            //Debug.Log("Close while문 실행");
            obsTransform.rotation = Quaternion.Slerp(obsTransform.rotation, Quaternion.Euler(CloseDoorAngle), smoot);
            timecnt += Time.deltaTime;
        }
    }


    //네트워크 계속 오류나서 바꿨는데 되는지 확인 해봐야함 
    [PunRPC]
    public void ChangeDoorState()
    {
        open = !open;

        if (open)
        {
            if (transform.parent.name.Contains("axis")) //축이 잘못돼있는 특수한 문들은 parent의 축에 접근해서 열리도록 
            {
                pv.RPC("OpenDoorParentCoroutineRPC", RpcTarget.All, transform.parent);
            }
            else
            {
                pv.RPC("OpenDoorCoroutineRPC", RpcTarget.All, transform);
            }

        }
        else
        {
            if (transform.parent.name.Contains("axis"))
            {
                StartCoroutine(CloseDoor(transform.parent));
            }
            else
            {
                StartCoroutine(CloseDoor(transform));
            }
        }

    }

    [PunRPC]
    public void OpenParentDoorCoroutineRPC()
    {
        StartCoroutine(OpenDoor(transform.parent));
    }

    [PunRPC]
    public void OpenDoorCoroutineRPC()
    {
        StartCoroutine(OpenDoor(transform));
    }

    public void ChangeDoorStateRPC()
    {
        pv.RPC("ChangeDoorState", RpcTarget.All);
    }


}

